using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using Market.ServiceLayer;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.XEvent;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Market.DomainLayer
{
    public class MarketManager
    {
        private ShopManager _shopManager;
        private UserManager _userManager;
        private IDeliverySystem _deliverySystem;
        private IPaymentSystem _paymentSystem;
        private static MarketManager _marketManager = null;
        private static object _lock = new object();

        private MarketManager()
        {
            _shopManager = ShopManager.GetInstance();
            _userManager = UserManager.GetInstance();
            _deliverySystem = new RealDeliverSystem();
            _paymentSystem = new RealPaymentSystem();
            ConnectExternalSystems();
            ConfigreCorrectnessConstrains();
        }
        private void ConnectExternalSystems()
        {
            if (_deliverySystem.Connect() && _paymentSystem.Connect())
                return;
            throw new Exception("Invalid Connection To External Services");
        }

        public static MarketManager GetInstance()
        {
            if (_marketManager != null)
                return _marketManager;
            lock(_lock){ 
                if (_marketManager == null)
                    _marketManager = new MarketManager();
            }
            return _marketManager;
        }
        public void Dispose()
        {
            _shopManager.Dispose();
            _userManager.Dispose();
            RuleRepo.GetInstance().Clear();
            AppointmentRepo.GetInstance().Clear();
            MemberRepo.GetInstance().Clear();
            ProductRepo.GetInstance().Clear();
            ReviewRepo.GetInstance().Clear();
            ShopRepo.GetInstance().Clear();
            MarketContext.GetInstance().Dispose();
            PurchaseRepo.GetInstance().Clear();
            _marketManager = new MarketManager();
        }

        public void ResetDomainData()
        {
            RuleRepo.GetInstance().ResetDomainData();
            AppointmentRepo.GetInstance().ResetDomainData();
            MemberRepo.GetInstance().ResetDomainData();
            ProductRepo.GetInstance().ResetDomainData();
            ReviewRepo.GetInstance().ResetDomainData();
            ShopRepo.GetInstance().ResetDomainData();
            PurchaseRepo.GetInstance().ResetDomainData();
            UserManager.GetInstance().ResetDomainData();
        }
        //public List<Member> SystemAdmins { get => _systemAdmins.ToList(); }
        public IDeliverySystem DeliverySystem { get => _deliverySystem; set => _deliverySystem = value; }
        public IPaymentSystem PaymentSystem { get => _paymentSystem; set => _paymentSystem = value; }

        /// <summary>
        /// Checks the session identifier- throws exception if the user is either not logged in or an active guest in the system.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">In order to execute that action, the user must be a visitor in the system.</exception>
        private Boolean CheckSessionID(string sessionID)
        {
            return _userManager.CheckSessionID(sessionID);
        }

        /// <summary>
        /// Checks the member session identifier- throws exception if the user is not logged in.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">In order to execute that action, the user must first be logged in.</exception>
        private Boolean CheckMemberSessionID(string sessionID)
        {
            return _userManager.CheckMemberSessionID(sessionID);
        }

        private void ConfigreCorrectnessConstrains()
        {
            string admin_card = "MasterAdmin";
            bool exist = MemberRepo.GetInstance().ContainsUserName(admin_card);
            if (!exist)
                _userManager.Register(admin_card, admin_card);
            Member member = _userManager.GetByUserName(admin_card);
            member.IsSystemAdmin = true;
            MemberRepo.GetInstance().SetAsSystemAdmin(member);
        }

        public void AppointSystemAdmin(string membersUserName)
        {
            Member member = _userManager.GetByUserName(membersUserName);
            if (member == null)
                throw new Exception("The member you wish to appoint to system admin must be regisered to the system first!");
            member.IsSystemAdmin = true;
            MemberRepo.GetInstance().SetAsSystemAdmin(member);
        }
        //------------------------ Guest Use Cases-------------------------------------------

        public int EnterAsGuest(string sessionID)
        {
            return _userManager.EnterAsGuest(sessionID);
        }

        /// <summary>
        /// Adds to cart.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="quantity">The quantity.</param>
        public void AddToCart(string sessionID, int shopId, int productID, int quantity)
        {
            CheckSessionID(sessionID);
            Shop shop = _shopManager.GetShop(shopId);
            using var transaction = MarketContext.GetInstance().Database.BeginTransaction();
            try
            {
                _userManager.AddToCart(sessionID, shop, productID, quantity);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets the market information.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public List<Shop> GetMarketInfo(string sessionID)
        {
            CheckSessionID(sessionID);
            return _shopManager.GetAll().ToList();
        }

        /// <summary>
        /// Gets the shop information.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <returns></returns>
        public Shop GetShopInfo(string sessionID, int shopID)
        {
            CheckSessionID(sessionID);
            return _shopManager.GetShop(shopID);
        }

        /// <summary>
        /// Gets the shopping cart information.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public ShoppingCart GetShoppingCartInfo(string sessionID)
        {
            CheckSessionID(sessionID);
            return _userManager.GetShoppingCartInfo(sessionID);
        }


        /// <summary>
        /// Purchases the shopping cart of the user identified by the specified session ID.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <exception cref="System.Exception">
        /// Problem with Payment System
        /// or
        /// Problem with delivery system
        /// </exception>
        public void PurchaseBasket(string sessionID, int shopId, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            using var transaction = MarketContext.GetInstance().Database.BeginTransaction();
            try
            {
                CheckSessionID(sessionID);
                User user = _userManager.GetUser(sessionID);
                Shop shop = _shopManager.GetShop(shopId);
                ShoppingCartPurchase shoppingCartPurchase = _userManager.Purchase(sessionID, shopId);
                SendPurchaseToExternalServices(user,shoppingCartPurchase,paymentDetails,deliveryDetails);
                shop.PurchaseSuccessHandler(shoppingCartPurchase.ShopPurchaseObjects[0]);
                _userManager.RemoveBasketFromCart(sessionID, shop.Id);
                user.PurchaseSuccessHandler(shoppingCartPurchase);
                transaction.Commit();
            }
            catch(Exception e) 
            {
                transaction.Rollback();
                throw new Exception(e.Message); 
            }
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        public void RemoveFromCart(string sessionID, int shopID, int productID)
        {
            CheckSessionID(sessionID);
            _userManager.RemoveFromCart(sessionID, shopID, productID);
        }

        /// <summary>
        /// Searches product according to the specified search type.
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public HashSet<Product> Search(string sessionID, string wordToSearch, List<int> searchType, List<FilterSearchType> filterType)
        {
            CheckSessionID(sessionID);
            SearchType st = ParseSearchType(searchType);
            return _shopManager.Search(wordToSearch, st, filterType);
        }

        /// <summary>
        /// Registers the specified session identifier with the user name and password given.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        public void Register(string sessionID, string username, string password)
        {
            _userManager.Register(username, password);
        }

        /// <summary>
        /// Logins according to the specified session identifier.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        public void Login(string sessionID, string username, string password)
        {
            _userManager.Login(sessionID, username, password);
        }
        /// <summary>
        /// Logs out according to the specified session identifier.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        public void Logout(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            _userManager.Logout(sessionID);
        }
        //------------------------------ Member Use Cases------------------------------------        
        /// <summary>
        /// Appoints the user identified by the appointeeID, to a role in the shop identified by the shopID.
        /// The functions also set the permissions of the appointee in the store to the given permissions.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="appointeeUserName">The appointee identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="permissions">The permissions.</param>
        public void Appoint(string sessionID, string appointeeUserName, int shopID, int role, int permissions)
        {
            CheckMemberSessionID(sessionID);
            Shop shop = _shopManager.GetShop(shopID);
            Permission newPermission = (Permission)permissions;
            Role newRole = (Role)role;
            _userManager.Appoint(sessionID, appointeeUserName, shop, newRole, newPermission);
        }
        /// <summary>
        /// Removes the appoint if the client has permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="appointeeUserName">Name of the appointee user.</param>
        /// <param name="shopID">The shop identifier.</param>
        public void RemoveAppoint(string sessionID, string appointeeUserName, int shopID)
        {
            CheckMemberSessionID(sessionID);
            Shop shop = _shopManager.GetShop(shopID);
            _userManager.RemoveAppoint(sessionID, appointeeUserName, shop);
        }

        public void CancelMembership(string sessionID, string memberName)
        {
            CheckMemberSessionID(sessionID);
            Member client = UserManager.GetInstance().GetMember(sessionID);
            if (!client.IsSystemAdmin)
                throw new Exception("Only a system admin can cancel a membership in the system!");
            Member member = MemberRepo.GetInstance().GetByUserName(memberName);
            if (member.IsSystemAdmin)
                throw new Exception("Can't cancel membership of a system admin!");
            _userManager.CancelMembership(sessionID, memberName);

        }
        /// <summary>
        /// Changes the permissions of the appointee if the client has permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="appointeeUserName">The appointee identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="permissions">The permissions.</param>
        public void ChangePermissions(string sessionID, string appointeeUserName, int shopID, int permissions)
        {
            CheckMemberSessionID(sessionID);
            Permission newPermission = (Permission)permissions;
            _userManager.ChangePermissions(sessionID, appointeeUserName, shopID, newPermission);
        }

        /// <summary>
        /// Adds the permissions of the appointee if the client has permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="appointeeUserName">The appointee identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="permissions">The permissions.</param>
        public void AddPermissions(string sessionID, string appointeeID, int shopID, int permissions)
        {
            CheckMemberSessionID(sessionID);
            Permission newPermission = (Permission)permissions;
            _userManager.AddPermissions(sessionID, appointeeID, shopID, newPermission);
        }

        /// <summary>
        /// Deletes the permissions of the appointee if the client has permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="appointeeID">The appointee identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="permissions">The permissions.</param>
        public void DeletePermissions(string sessionID, string appointeeUserName, int shopID, int permissions)
        {
            CheckMemberSessionID(sessionID);
            Permission newPermission = (Permission)permissions;
            _userManager.DeletePermissions(sessionID, appointeeUserName, shopID, newPermission);
        }
        /// <summary>
        /// Closes the shop if the client has the right permissions to do so.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        public void CloseShop(string sessionID, int shopID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.CloseShop(member.Id, shopID);
        }

        /// <summary>
        /// Opens a shop if the client has the right permissions to do so.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OpenShop(string sessionID, int shopID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.OpenShop(member.Id, shopID);
        }

        /// <summary>
        /// Creates a new shop and make the client the founder of the shop.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopName">Name of the shop.</param>
        public Shop CreateShop(string sessionID, string shopName)
        {
            lock (_lock)
            {
                CheckMemberSessionID(sessionID);
                Member owner = _userManager.GetMember(sessionID);
                Shop newShop = _shopManager.CreateShop(owner, shopName);
                owner.AppointFounder(newShop);
                return newShop;
            }
        }
        /// <summary>
        /// Returns information about the shop positions if the client has the right permissions.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <returns></returns>
        public List<Appointment> GetShopPositions(string sessionID, int shopID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            return _shopManager.getShopPositions(member.Id, shopID);
        }

        /// <summary>
        /// Adds new product to the shop if the client has the right permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="description">The description.</param>
        /// <param name="price">The price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="category">The category.</param>
        /// <param name="keyWords">The key words.</param>
        public Product AddProduct(string sessionID, int shopId, string productName,int sellType, string description, double price, int quantity, string category, List<string> keyWords)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            SynchronizedCollection<string> keys = new SynchronizedCollection<string>();
            foreach (string keyword in keyWords) keys.Add(keyword);
            return _shopManager.AddProduct(member.Id, shopId, productName,GenerateSellMethod(sellType), description, price, quantity, category, keys);
        }

        private ISell GenerateSellMethod(int sellType)
        {
            if (sellType == 0) return new RegularSell();
            else if (sellType == 1) return new BidSell();
            else throw new Exception("Unrecognize sell type");
        }

        /// <summary>
        /// Removes the product.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        public void RemoveProduct(string sessionID, int shopId, int productID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.RemoveProduct(member.Id, shopId, productID);
        }

        /// <summary>
        /// Updates the name of the product.
        /// If the client has the right permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="name">The name.</param>
        public void UpdateProductName(string sessionID, int shopId, int productID, string name)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateProductName(member.Id, shopId, productID, name);
        }

        /// <summary>
        /// Updates the product price.
        /// If the client has the right permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="price">The price.</param>
        public void UpdateProductPrice(string sessionID, int shopId, int productID, double price)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateProductPrice(member.Id, shopId, productID, price);
        }

        /// <summary>
        /// Updates the product quantity.
        /// If the client has the right permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="quantity">The quantity.</param>
        public void UpdateProductQuantity(string sessionID, int shopId, int productID, int quantity)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateProductQuantity(member.Id, shopId, productID, quantity);
        }

        /// <summary>
        /// Adds a review.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="rate">The rate.</param>
        /// <param name="comment">The comment.</param>
        public void AddReview(string sessionID, int shopId, string userName, int productID, double rate, string comment)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddReview(shopId, member.Id, userName, productID, rate, comment);
        }

        /// <summary>
        /// Parses the type of the search.
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        private SearchType ParseSearchType(List<int> searchType)
        {
            //-------default search type-------
            SearchType st = SearchType.Name;
            //---------------------------------
            if (searchType.Contains(1))
                st |= SearchType.Keywords;
            if (searchType.Contains(2))
                st |= SearchType.Category;
            //check if search type Name is requested//
            if (!searchType.Contains(0))
                st &= ~SearchType.Name;
            return st;
        }

        /// <summary>
        /// Shows the member's purchase history.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public List<Purchase> ShowPurchaseHistory(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            return _shopManager.ShowPurchaseHistory(member.Id);
        }

        /// <summary>
        /// Shows the shop's purchases history.
        /// If the client has the right permissions to execute this action.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <returns></returns>
        public List<Purchase> ShowShopHistory(string sessionID, int shopID)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            return _shopManager.ShowShopHistory(member.Id, shopID);
        }
        public User GetUser(string sessionID)
        {
            return _userManager.GetUser(sessionID);
        }

        public void PurchaseShoppingCart(string sessionID, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            using var transaction = MarketContext.GetInstance().Database.BeginTransaction();
            try
            {
                CheckSessionID(sessionID);
                User user = _userManager.GetUser(sessionID);
                ShoppingCartPurchase shoppingCartPurchaseObject = user.PurchaseShoppingCart();
                SendPurchaseToExternalServices(user,shoppingCartPurchaseObject,paymentDetails,deliveryDetails);
                NotifyShopsPurchaseSuccess(user, shoppingCartPurchaseObject.ShopPurchaseObjects);
                user.PurchaseSuccessHandler(shoppingCartPurchaseObject);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        private void SendPurchaseToExternalServices(User user, ShoppingCartPurchase shoppingCartPurchase, PaymentDetails paymentDetails, DeliveryDetails deliveryDetails)
        {
            int orderNum = -1;
            try
            {
                orderNum = _deliverySystem.OrderDelivery(shoppingCartPurchase, deliveryDetails);
                bool successOrder = orderNum > -1;
                bool successPayment;
                int receiptNum;
                if (!successOrder)
                {
                    NotifyShopsPurchaseFailed(shoppingCartPurchase.ShopPurchaseObjects);
                    user.PurchaseFailHandler(shoppingCartPurchase);
                    throw new Exception("Problem with delivery system");
                }
                receiptNum = _paymentSystem.Pay(shoppingCartPurchase, paymentDetails);
                successPayment = receiptNum > -1;
                if (!successPayment)
                {
                    NotifyShopsPurchaseFailed(shoppingCartPurchase.ShopPurchaseObjects);
                    user.PurchaseFailHandler(shoppingCartPurchase);
                    _deliverySystem.CancelOrder(orderNum);
                    throw new Exception("Problem with Payment System");
                }
                shoppingCartPurchase.PaymentId = receiptNum;
                shoppingCartPurchase.DeliveryId = orderNum;
            }
            catch (TimeoutException ex)
            {
                NotifyShopsPurchaseFailed(shoppingCartPurchase.ShopPurchaseObjects);
                user.PurchaseFailHandler(shoppingCartPurchase);
                throw new Exception(ex.Message);
            }
        }
        private void NotifyShopsPurchaseSuccess(User user, SynchronizedCollection<Purchase> purchases)
        {
            foreach (Purchase purchase in purchases)
            {
                Shop shop = _shopManager.GetShop(purchase.ShopId);
                shop.PurchaseSuccessHandler(purchase);
            }
        }
        private void NotifyShopsPurchaseFailed(SynchronizedCollection<Purchase> purchases)
        {
            foreach (Purchase purchase in purchases)
            {
                Shop shop = _shopManager.GetShop(purchase.ShopId);
                shop.PurchaseFailHandler(purchase);
            }
        }
        public Shop GetShopByName(string name)
        {
            return _shopManager.GetShopByName(name);
        }

        public List<Member> GetAllMembers(string sessionID)
        {
            CheckSessionID(sessionID);
            Member client = _userManager.GetMember(sessionID);
            if (!client.IsSystemAdmin)
                throw new Exception("Only a system admin can get informations about members in the system!");
            return MemberRepo.GetInstance().GetAll();

        }
        public List<Member> GetActiveMembers(string sessionID)
        {
            CheckSessionID(sessionID);
            Member client = _userManager.GetMember(sessionID);
            if (!client.IsSystemAdmin)
                throw new Exception("Only a system admin can get informations about members in the system!");
            return _userManager.GetActiveMembers();
        }
        //--------------------------------TODO: Implement---------------------------------------------------------
        public void SendMessage(string sessionID, int shopID, string comment)
        {
            CheckMemberSessionID(sessionID);
            Member user = _userManager.GetMember(sessionID);
            _shopManager.SendMessageToShop(shopID, user, comment);
        }

        public void SendReport(string sessionID, int shopId, string comment)
        {
            CheckMemberSessionID(sessionID);
            Member user = _userManager.GetMember(sessionID);
            _shopManager.SendReportToShop(shopId, user, comment);
        }

        //---------------------------Doesn't need to be implemented yet!!---------------------------------------
        public void RemovePolicy(string sessionID, int shopID, int policyID,string type)
        {
            CheckMemberSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            Shop shop = _shopManager.GetShop(shopID);
            shop.RemovePolicy(member.Id, policyID, type);
        }
        public void AddSimpleRule(string sessionID, int shopID,string subject)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddSimpleRule(shopID, member.Id, subject);
        }
        public void AddQuantityRule(string sessionID, int shopID, string subject, int minQuantity, int maxQuantity)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddQuantityRule(shopID, member.Id, subject, minQuantity, maxQuantity);
        }
        public void AddTotalPriceRule(string sessionID, int shopID, string subject, int targetPrice)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddTotalPriceRule(shopID, member.Id, subject, targetPrice);
        }
        public void AddCompositeRule(string sessionID, int shopID, int Operator, List<int> rules)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddCompositeRule(shopID, member.Id, Operator, rules);
        }
        public void UpdateRuleSubject(string sessionID, int shopID, int ruleId, string subject)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateRuleSubject(shopID, member.Id, ruleId, subject);
        }
        public void UpdateRuleQuantity(string sessionID, int shopID, int ruleId, int minQuantity, int maxQuantity)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateRuleQuantity(shopID, member.Id, ruleId, minQuantity, maxQuantity);
        }
        public void UpdateRuleTargetPrice(string sessionID, int shopID, int ruleId, int targetPrice)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateRuleTargetPrice(shopID, member.Id, ruleId, targetPrice);
        }
        public void UpdateCompositeOperator(string sessionID, int shopID, int ruleId, int Operator)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateCompositeOperator(shopID, member.Id, ruleId, Operator);
        }
        public void UpdateCompositeRules(string sessionID, int shopID, int ruleId, List<int> rules)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.UpdateCompositeRules(shopID, member.Id, ruleId, rules);
        }

        public void AddPurchasePolicy(string sessionID, int shopID, string expirationDate, string subject, int ruleId)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddPurchasePolicy(shopID, member.Id, expirationDate, subject, ruleId);
        }
        public void AddDiscountPolicy(string sessionID, int shopID, string expirationDate, string subject, int ruleId, double precentage)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddDiscountPolicy(shopID, member.Id, expirationDate, subject, ruleId , precentage);
        }
        public void AddCompositePolicy(string sessionID, int shopID, string expirationDate, string subject, int Opreator, List<int> policies)
        {
            CheckSessionID(sessionID);
            Member member = _userManager.GetMember(sessionID);
            _shopManager.AddCompositePolicy(shopID, member.Id, expirationDate, subject, Opreator, policies);
        }

        public List<Shop> GetUserShops(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            return _userManager.GetUserShops(sessionID);
        }
        public List<Message> GetMessages(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            return _userManager.GetMessages(sessionID);
        }
        public void NotificationOn(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            _userManager.NotificationOn(sessionID);
        }
        public void NotificationOff(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            _userManager.NotificationOff(sessionID);
        }
        public bool IsSystemAdmin(string sessionID)
        {
            Member client = _userManager.GetMemberBySessionId(sessionID);
            return client.IsSystemAdmin;
        }
        public void UpdateBasketItemQuantity(string sessionID, int shopId, int productID, int quantity)
        {
            CheckSessionID(sessionID);
            User user = _userManager.GetUser(sessionID);
            _userManager.UpdateBasketItemQuantity(user, shopId, productID, quantity);
        }

        public void BidOnProduct(string sessionId, int shopID, int productId, int quantity, double suggestedPriceForOne)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            _shopManager.BidOnProduct(member,shopID,productId,quantity,suggestedPriceForOne);

        }
        public void ApproveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            Member biddingMember = MemberRepo.GetInstance().GetByUserName(bidUsername);
            _shopManager.ApproveBid(member, shopID ,biddingMember.Id, productId);
        }
        public void OfferCounterBid(string sessionId, int shopID, string bidUsername, int productId, double counterPrice)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            Member biddingMember = MemberRepo.GetInstance().GetByUserName(bidUsername);
            _shopManager.OfferCounterBid(member, shopID, biddingMember.Id, productId, counterPrice);
        }
        public void ApproveCounterBid(string sessionId, int shopId, int productId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            _shopManager.ApproveCounterBid(member.Id, shopId,productId);
        }
        public void DissapproveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            Member biddingMember = MemberRepo.GetInstance().GetByUserName(bidUsername);
            _shopManager.DissapproveBid(member, shopID, biddingMember.Id, productId);
        }

        public void RemoveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            Member biddingMember = MemberRepo.GetInstance().GetByUserName(bidUsername);
            if (biddingMember.ShoppingCart.HasBasketItem(shopID, productId))
            {
                throw new Exception("Cannot remove already approve bid");
            }
            _shopManager.RemoveBid(member, shopID, biddingMember.Id, productId);
        }

        public void AddApproval(string sessionID, int shopId, string appointeeUserName)
        {
            CheckMemberSessionID(sessionID);
            Member client = UserManager.GetInstance().GetMember(sessionID);
            Shop shop = ShopManager.GetInstance().GetShop(shopId);
            bool agreementIsApproved = shop.AddApproval(client, appointeeUserName);
            if (agreementIsApproved)
            {
                Member appointee = UserManager.GetInstance().GetByUserName(appointeeUserName);
                UserManager.GetInstance().AppointOwner(shop, appointee);
            }
        }

        public void AddDecline(string sessionID, int shopId, string appointeeUserName)
        {
            CheckMemberSessionID(sessionID);
            Member client = UserManager.GetInstance().GetMember(sessionID);
            Shop shop = ShopManager.GetInstance().GetShop(shopId);
            shop.AddDecline(client, appointeeUserName);
        }

        public void DeclineCounterBid(string sessionId, int shopID, int productId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            _shopManager.DeclineCounterBid(member.Id, shopID, productId);
        }

        public int GetMessagesNumberRequest(string sessionId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            int notifications = member.GetMessagesNumber();
            return notifications;

        }

        public int GetShoppingCartAmount(string sessionId)
        {
            CheckMemberSessionID(sessionId);
            Member member = _userManager.GetMember(sessionId);
            int productAmount = member.GetShoppingCartAmount();
            return productAmount;
        }
    }
}
