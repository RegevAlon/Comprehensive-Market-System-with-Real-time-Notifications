using Market.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Market.RepoLayer;
using Market.DataLayer;
using Market.DataLayer.DTOs;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;

namespace Market.DomainLayer
{
    public class Member : User
    {
        private string _userName;
        private string _password;
        private ConcurrentDictionary<int, Appointment> _appointments;
        private SynchronizedCollection<Message> _messages;
        private SynchronizedCollection<ShoppingCartPurchase> _userPurchases;
        private bool _notification;
        private bool _isSystemAdmin;
        private bool _loggedIn;
        private NotificationManager _notificationManager = NotificationManager.GetInstance();

        public Member(int id, string userName, string password) : base(id)
        {
            _userName = userName;
            _password = password;
            _appointments = new ConcurrentDictionary<int, Appointment>();
            _messages = new SynchronizedCollection<Message>();
            _userPurchases = new SynchronizedCollection<ShoppingCartPurchase>();
            _notification = true;
            _isSystemAdmin = false;
            _loggedIn = false;
        }

        public Member(MemberDTO member) : base(member.Id)
        {
            _userName = member.UserName;
            _password = member.Password;
            _notification = member.Notification;
            _password = member.Password;
            _messages = new SynchronizedCollection<Message>();
            _appointments = new ConcurrentDictionary<int, Appointment>();
            _userPurchases = new SynchronizedCollection<ShoppingCartPurchase>();
            _isSystemAdmin = member.IsSystemAdmin;
            _loggedIn = false;
        }
        public void InitializeComplexFeilds(MemberDTO member)
        {
            if (member.Messages != null)
                foreach (MessageDTO message in member.Messages) _messages.Add(new Message(message.MessageContent, message.Seen));
            List<AppointmentDTO> appointmentDTOs = MarketContext.GetInstance().Appointments.
               Where((app) => app.MemberId == Id).ToList();
            foreach (AppointmentDTO appointmentDTO in appointmentDTOs)
                if (appointmentDTO != null)
                {
                    _appointments.TryAdd(appointmentDTO.ShopId,
                        AppointmentRepo.GetInstance().GetById(appointmentDTO.MemberId, appointmentDTO.ShopId));
                }
            List<ShoppingCartPurchaseDTO> shoppingCartPurchaseDTOs = member.ShoppingCartPurchases;
            if (shoppingCartPurchaseDTOs == null) shoppingCartPurchaseDTOs = new List<ShoppingCartPurchaseDTO>();
            foreach (ShoppingCartPurchaseDTO spDTO in shoppingCartPurchaseDTOs)
                _userPurchases.Add(new ShoppingCartPurchase(spDTO));
            ShoppingCartDTO scpDto = member.ShoppingCart;
            if (scpDto == null)
                scpDto = MarketContext.GetInstance().ShoppingCarts.Include(s => s.Baskets).FirstOrDefault(s => s.Id == Id);
            ShoppingCart = new ShoppingCart(scpDto);
        }

        public string UserName { get => _userName; }
        public string Password { get => _password; set => _password = value; }
        public ConcurrentDictionary<int, Appointment> Appointments { get => _appointments; set => _appointments = value; }
        public SynchronizedCollection<Message> Messages { get => _messages; }
        public bool Notification { get => _notification; set => _notification = value; }
        public SynchronizedCollection<ShoppingCartPurchase> UserPurchases { get => _userPurchases; set => _userPurchases = value; }
        public bool IsSystemAdmin { get => _isSystemAdmin; set => _isSystemAdmin = value; }
        public bool LoggedIn { get => _loggedIn; set => _loggedIn = value; }
        public NotificationManager NotificationManager { get => _notificationManager; set => _notificationManager = value; }

        /// <summary>
        /// Checks if there are circular appoints.
        /// Checks recursively that the appointee is not an appointer of the appointing member.
        /// </summary>
        /// <param name="appointeeID">The appointee identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <returns></returns>
        public Boolean CheckCircularAppoints(int appointeeID, int shopID)
        {
            if (!_appointments.ContainsKey(shopID) || (_appointments[shopID].Appointer != null && _appointments[shopID].Appointer.Id == appointeeID))
                return true;
            if (_appointments[shopID].Appointer == null)
                return false;
            return _appointments[shopID].Appointer.CheckCircularAppoints(appointeeID, shopID);
        }

        /// <summary>
        ///The function appoint an appointee to the role on the shop and give permissions
        /// according to parameters given.
        /// The functions checks if there are no circular appointments!
        /// The function allows only appointments of managers and owners!
        /// </summary>
        /// <param name="appointee"></param>
        /// <param name="shop"></param>
        /// <param name="role"></param>
        /// <param name="permissions"></param>
        public void Appoint(Member appointee, Shop shop, Role role, Permission permissions)
        {
            if (!_appointments.ContainsKey(shop.Id) || !_appointments[shop.Id].HasPermission(Permission.Appoint))
                throw new ArgumentException("In order to execute that action, the user must have permissions to appoint.");
            if (CheckCircularAppoints(appointee.Id, shop.Id))
                throw new ArgumentException("Circular appointments are NOT allowed!");
            if (role == Role.Founder)
                throw new ArgumentException("Only the user who opened the shop can be the shop founder");

            Appointment newAppointment = new Appointment(appointee, shop, this, role, permissions);
            if (role == Role.Manager)
            {
                appointee.AddManagerAppointment(shop.Id, newAppointment);
                shop.AddAppointment(appointee, newAppointment);
                //==================Save into Database====================
                AppointmentRepo.GetInstance().Add(newAppointment);
                //========================================================
                AddApointee(shop.Id, appointee);
            }
            else if (role == Role.Owner)
            {
                if (appointee.Appointments.ContainsKey(shop.Id) && (appointee.Appointments[shop.Id].Role == Role.Owner || appointee.Appointments[shop.Id].Role == Role.Founder))
                    throw new ArgumentException(appointee.UserName + " already is a owner or founder in the shop: " + shop.Id + ", and can't be appointed again");
                List<Member> pendings = MemberRepo.GetInstance().GetOwners(shop.Id);
                pendings.Remove(this);
                if (pendings.Count > 0)
                {
                    PendingAgreement pendingAgreement = new PendingAgreement(shop.Id, this, appointee, pendings);
                    shop.AddPendingAgreement(pendingAgreement);
                    PendingAgreementsRepo.GetInstance().AddPendingAgreement(pendingAgreement, shop);

                }
                else
                {
                    AddOwnerAppointment(shop, newAppointment);
                }
            }

        }

        public void RemoveAppoint(Member appointee, Shop shop)
        {
            if (!appointee.Appointments.ContainsKey(shop.Id))
                throw new ArgumentException("The appointee provided does not have an appointment to the shop given.");
            if (appointee.Appointments[shop.Id].Appointer == null)
                throw new ArgumentException("Removing the appointment of the shop founder is not allowed!.");
            if (appointee.Appointments[shop.Id].Appointer.Id != this.Id)
                throw new ArgumentException("In order to Remove appointment of an appointee the user must be the appointee's appointer in this shop.");

            foreach (Member recAppointee in appointee.Appointments[shop.Id].Apointees.ToList<Member>())
            {
                appointee.RemoveAppoint(recAppointee, shop);
            }
            Appointment outAppointment;
            shop.RemoveAppointment(this, appointee);
            appointee.Appointments.TryRemove(shop.Id, out outAppointment);
            AppointmentRepo.GetInstance().Delete(appointee.Id, shop.Id);
            Appointments[shop.Id].Apointees.Remove(appointee);
            if (outAppointment.Role == Role.Owner)
                PendingAgreementsRepo.GetInstance().DeleteOwnerAndCheck(appointee, shop);
        }

        private void AddApointee(int shopId, Member appointee)
        {
            _appointments[shopId].Apointees.Add(appointee);

            //===================Save to DB====================
            MarketContext context = MarketContext.GetInstance();
            AppointmentDTO appointmentDTO = context.Appointments.Find(Id, shopId);
            MemberDTO appointeeDTO = context.Members.Find(appointee.Id);
            appointmentDTO.Appointees.Add(new AppointeesDTO(appointeeDTO));
            context.SaveChanges();
            //=================================================
        }
        private void RemoveApointee(int shopId, Member appointee)
        {
            _appointments[shopId].Apointees.Remove(appointee);

            //===================Save to DB====================
            MarketContext context = MarketContext.GetInstance();
            AppointmentDTO appointmentDTO = context.Appointments.Find(Id, shopId);
            AppointeesDTO appointeeDTO = appointmentDTO.Appointees.Where((a) => a.Appointee.Id == appointee.Id).FirstOrDefault();
            appointmentDTO.Appointees.Remove(appointeeDTO);
            context.SaveChanges();
            //=================================================
        }
        public void ChangePermissions(Member appointee, int shopID, Permission permissions)
        {
            if (_appointments.ContainsKey(shopID) && IsMyApointee(shopID, appointee) && HasPermission(shopID, Permission.Appoint))
            {
                appointee.Appointments[shopID].Permissions = permissions;

                //===================Save To Database====================
                MarketContext context = MarketContext.GetInstance();
                AppointmentDTO appDto = context.Appointments.Find(appointee.Id, shopID);
                if (appDto != null)
                {
                    appDto.Permissions = Convert.ToInt32(Appointments[shopID].Permissions);
                    context.SaveChanges();
                }
                //========================================================
            }
            else throw new Exception("Only the appointer of this member can change it's permissions");
        }
        public void AddPermissions(Member appointee, int shopID, Permission permissions)
        {
            if (HasPermission(shopID, Permission.Appoint) && IsMyApointee(shopID, appointee))
            {
                Permission currPermission = appointee.Appointments[shopID].Permissions;
                appointee.Appointments[shopID].Permissions = (currPermission | permissions);

                //===================Save To Database====================
                MarketContext context = MarketContext.GetInstance();
                AppointmentDTO appDto = context.Appointments.Find(appointee.Id, shopID);
                if (appDto != null)
                {
                    appDto.Permissions = Convert.ToInt32(Appointments[shopID].Permissions);
                    context.SaveChanges();
                }
                //========================================================
            }
            else throw new Exception("No permission to appoint");
        }
        private bool IsMyApointee(int shopId, Member member)
        {
            if (_appointments[shopId] != null)
                return _appointments[shopId].Apointees.Contains(member);
            else throw new Exception("No appointment for that shop");
        }
        public void DeletePermissions(Member appointee, int shopID, Permission permissions)
        {
            if (IsMyApointee(shopID, appointee))
            {
                appointee.Appointments[shopID].Permissions &= ~permissions;

                //===================Save To Database====================
                MarketContext context = MarketContext.GetInstance();
                AppointmentDTO appDto = context.Appointments.Find(appointee.Id, shopID);
                if (appDto != null)
                {
                    appDto.Permissions = Convert.ToInt32(Appointments[shopID].Permissions);
                    context.SaveChanges();
                }
                //========================================================
            }
            else throw new Exception("Only the appointer of this user can change is permissions.");
        }
        /// <summary>
        /// When a member is getting appointed to a manager this function is called by the appointer.
        /// If the member already has a role in the given shop an exception will be thrown
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="appointment"></param>
        public bool AddManagerAppointment(int shopID, Appointment appointment)
        {
            if (_appointments.ContainsKey(shopID))
                throw new ArgumentException(this.UserName + " already has a role in the shop: " + shopID + ", and can't be appointed to another role");
            return _appointments.TryAdd(shopID, appointment);


        }
        /// <summary>
        /// When a member is getting appointed to a manager this function is called by the appointer.
        /// If the member already has a role as an owner or founder in the given shop an exception will be thrown
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="appointment"></param>
        public bool AddOwnerAppointment(Shop shop, Appointment appointment)
        {
            Member appointee = appointment.Member;
            Member appointer = appointment.Member;
            if (!appointee._appointments.ContainsKey(shop.Id))
            {
                bool wasAdded = appointee._appointments.TryAdd(shop.Id, appointment);
                shop.AddAppointment(appointee, appointment);
                //==================Save into Database====================
                if (wasAdded)
                    AppointmentRepo.GetInstance().Add(appointment);
                //========================================================
            }
            else
            {
                Appointment oldAppointment = appointee._appointments[shop.Id];
                bool wasUpdated = appointee._appointments.TryUpdate(shop.Id, appointment, oldAppointment);
                if (wasUpdated)
                {
                    AppointmentRepo.GetInstance().Update(appointment);
                    oldAppointment.Appointer.RemoveApointee(shop.Id, appointee);
                    shop.UpdtaeAppointment(appointee, appointment);
                }
            }
            if (shop.PendingAgreements.ContainsKey(appointee.UserName))
            {
                PendingAgreementsRepo.GetInstance().DeletePendingAgreement(shop.PendingAgreements[appointee.UserName], shop.Id);
                shop.DeletePendingAgreement(appointee.UserName);
            }
            PendingAgreementsRepo.GetInstance().AddOwner(shop, appointment);
            AddApointee(shop.Id, appointee);
            return true;
        }

        /// <summary>
        /// When a member creates a shop and becomes it's founder this function is called by the user manager.
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="appointment"></param>
        public void AppointFounder(Shop shop)
        {
            Appointment appointment = new Appointment(this, shop, null, Role.Founder, Permission.All);
            bool wasAdded = _appointments.TryAdd(shop.Id, appointment);
            shop.AddAppointment(this, appointment);

            //==================Save into Database====================
            MarketContext context = MarketContext.GetInstance();
            if (wasAdded)
                AppointmentRepo.GetInstance().Add(appointment);
            context.SaveChanges();
            //========================================================
        }
        public override void AddToCart(Shop shop, int productId, int quantity)
        {
            MarketContext context = MarketContext.GetInstance();
            base.AddToCart(shop, productId, quantity);

            //===================Update DB======================
            MemberDTO memberDTO = context.Members.Find(Id);
            if (memberDTO != null)
            {
                BasketDTO basketDTO = memberDTO.ShoppingCart.Baskets.Find((s) => s.ShopId == shop.Id);
                if (basketDTO == null)
                {
                    memberDTO.ShoppingCart.Baskets.Add(new BasketDTO(ShoppingCart.BasketbyShop[shop.Id]));
                }
                else
                {
                    basketDTO.BasketItems.Add(new BasketItemDTO(ShoppingCart.FindBasketItem(shop.Id, productId)));
                }
            }
            context.SaveChanges();
            //==================================================
        }

        public override void RemoveFromCart(int shopId, int productId)
        {
            MarketContext context = MarketContext.GetInstance();
            base.RemoveFromCart(shopId, productId);

            //========================Update DB==========================
            MemberDTO memberDTO = context.Members.Find(Id);
            if (memberDTO != null)
            {
                BasketDTO basketDTO = memberDTO.ShoppingCart.Baskets.Find((s) => s.ShopId == shopId);
                if (basketDTO != null)
                {
                    BasketItemDTO basketItemDTO = basketDTO.BasketItems.
                        Where((b) => b.Product.Id == productId).ToList().First();
                    basketDTO.BasketItems.Remove(basketItemDTO);
                    context.BasketItems.Remove(basketItemDTO);
                    if (basketDTO.BasketItems.Count == 0)
                    {
                        context.ShoppingCarts.Find(Id).Baskets.Remove(basketDTO);
                        context.Baskets.Remove(basketDTO);
                    }

                }
            }
            context.SaveChanges();
            //===========================================================
        }
        public void Notify(string msg)
        {
            Message message = new Message(msg);


            if (_notification && LoggedIn)
            {
                NotificationManager.SendNotification(msg, UserName);
                message.Seen = true;
            }
            _messages.Add(message);
            MarketContext.GetInstance().Members.Find(Id).Messages.Add(new MessageDTO(message));
            MarketContext.GetInstance().SaveChanges();

        }
        private bool HasPermission(int shopId, Permission permission)
        {
            string name = this.UserName;
            bool hasAppointment = _appointments.ContainsKey(shopId);
            bool hasPerm = hasAppointment && (_appointments[shopId].HasPermission(permission));
            return (hasPerm);
        }

        public List<Shop> GetUserShops()
        {
            List<Shop> shops = new List<Shop>();
            foreach (Appointment appointment in _appointments.Values)
            {
                shops.Add(appointment.Shop);
            }
            return shops;
        }
        public override ShoppingCartPurchase PurchaseShoppingCart()
        {
            ShoppingCartPurchase pendingPurchase = base.PurchaseShoppingCart();
            _userPurchases.Add(pendingPurchase);
            MarketContext context = MarketContext.GetInstance();
            context.Members.Find(Id).ShoppingCartPurchases.Add(new ShoppingCartPurchaseDTO(pendingPurchase));
            context.SaveChanges();
            return pendingPurchase;
        }

        public override bool Equals(object obj)
        {
            return (obj is Member member &&
                   Id == member.Id &&
                   UserName == member.UserName);
        }
        public void NotificationOff()
        {
            if (_notification)
            {
                _notification = false;

                //==========================Update Db=========================
                MarketContext context = MarketContext.GetInstance();
                MemberDTO memberDTO = context.Members.Find(Id);
                if (memberDTO != null)
                {
                    memberDTO.Notification = _notification;
                    context.SaveChanges();
                }
                //============================================================
            }
            else throw new Exception("Notification already Off");
        }

        public override void PurchaseFailHandler(ShoppingCartPurchase pendingPurchase)
        {
            pendingPurchase.PurchaseStatus = PurchaseStatus.Failed;
            ShoppingCartPurchaseDTO shoppingCartPurchaseDTO = MarketContext.GetInstance().ShoppingCartPurchases.Find(pendingPurchase.Id);
            shoppingCartPurchaseDTO.PurchaseStatus = pendingPurchase.PurchaseStatus.ToString();
            MarketContext.GetInstance().SaveChanges();
        }
        public override void PurchaseSuccessHandler(ShoppingCartPurchase pendingPurchase)
        {
            pendingPurchase.PurchaseStatus = PurchaseStatus.Success;
            ShoppingCart.PurchaseSuccessHandler();
            MarketContext context = MarketContext.GetInstance();
            ShoppingCartPurchaseDTO shoppingCartPurchaseDTO = context.ShoppingCartPurchases.Find(pendingPurchase.Id);
            shoppingCartPurchaseDTO.PurchaseStatus = pendingPurchase.PurchaseStatus.ToString();
            shoppingCartPurchaseDTO.DeliveryId = pendingPurchase.DeliveryId;
            shoppingCartPurchaseDTO.PaymentId = pendingPurchase.PaymentId;
            context.ShoppingCarts.Find(Id).Baskets.RemoveRange(0, context.ShoppingCarts.Find(Id).Baskets.Count());
            context.SaveChanges();
        }
        public override void RemoveBasketFromCart(int shopId)
        {
            MarketContext context = MarketContext.GetInstance();
            ShoppingCart.RemoveBasket(shopId);

            //==========================Update Db=========================
            ShoppingCartDTO shoppingCartDTO = context.ShoppingCarts.Find(Id);
            BasketDTO basketDto = shoppingCartDTO.Baskets.FirstOrDefault(bs => bs.ShopId == shopId);
            basketDto.BasketItems.Clear();
            shoppingCartDTO.Baskets.Remove(basketDto);
            context.SaveChanges();
            //============================================================
        }
        public void NotificationOn()
        {
            if (!_notification)
            {
                _notification = true;

                //==========================Update Db=========================
                MarketContext context = MarketContext.GetInstance();
                MemberDTO memberDTO = context.Members.Find(Id);
                if (memberDTO != null)
                {
                    memberDTO.Notification = _notification;
                    context.SaveChanges();
                }
                //============================================================
            }
            else throw new Exception("Notification already On");
        }
        public override void UpdateBasketItemQuantity(int shopId, int productID, int quantity)
        {
            MarketContext context = MarketContext.GetInstance();
            base.UpdateBasketItemQuantity(shopId, productID, quantity);

            //==========================Update Db=========================
            ShoppingCartDTO shoppingCartDTO = context.ShoppingCarts.Find(Id);
            BasketDTO basketDto = shoppingCartDTO.Baskets.FirstOrDefault(bs => bs.ShopId == shopId);
            BasketItemDTO basketItemDTO = basketDto.BasketItems.FirstOrDefault(bi => bi.Product.Id == productID);
            if (quantity > 0)
            {
                basketItemDTO.Quantity = quantity;
            }
            else
            {
                basketDto.BasketItems.Remove(basketItemDTO);
                context.BasketItems.Remove(basketItemDTO);
                if (basketDto.BasketItems.Count() == 0)
                {
                    shoppingCartDTO.Baskets.Remove(basketDto);
                    context.Baskets.Remove(basketDto);
                }
            }
            context.SaveChanges();
            //============================================================
        }

        public void AddBidProductToCart(Shop shop, Bid bid, Product product)
        {
            ShoppingCart.AddBidProduct(shop, bid, product);

            //===================Update DB======================
            MarketContext context = MarketContext.GetInstance();
            MemberDTO memberDTO = context.Members.Find(Id);
            if (memberDTO != null)
            {
                BasketDTO basketDTO = memberDTO.ShoppingCart.Baskets.Find((s) => s.ShopId == shop.Id);
                if (basketDTO == null)
                {
                    memberDTO.ShoppingCart.Baskets.Add(new BasketDTO(ShoppingCart.BasketbyShop[shop.Id]));
                }
                else
                {
                    basketDTO.BasketItems.Add(new BasketItemDTO(ShoppingCart.FindBasketItem(shop.Id, bid.ProductId)));
                }
            }
            context.SaveChanges();
            //==================================================
        }
        object _lock = new Object();
        public List<Message> GetMessages()
        {
            lock (_lock)
            {
                MarketContext context = MarketContext.GetInstance();
                MemberDTO mem = context.Members.Find(Id);
                if (mem.Messages != null)
                {
                    _messages.Clear();
                    List<MessageDTO> msgDTO = mem.Messages.ToList();
                    foreach (MessageDTO msg in msgDTO)
                    {
                        if (!msg.Seen & _notification)
                        {
                            NotificationManager.SendNotification(msg.MessageContent, UserName);
                            msg.Seen = true;
                        }
                        _messages.Add(new Message(msg));
                        context.SaveChanges();
                    }
                }
                return _messages.ToList();
            }


        }

        public int GetMessagesNumber()
        {
            return _messages.Count;
        }

        public int GetShoppingCartAmount()
        {
            return ShoppingCart.getShoppingCartProductAmount();
        }
    }
}
