using Market.DataLayer;
using Market.RepoLayer;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Market.DomainLayer
{
    public class UserManager
    {
        private static UserManager _userManager = null;
        private int _userCounter;
        private MemberRepo _memberRepo;
        private static ConcurrentDictionary<string, Member> _memberBySession;
        private static ConcurrentDictionary<string, Guest> _activeGuests;
        private Security _paswwordSecurity;
        private object _lock = new object();


        private UserManager()
        {
            _userCounter = 1;
            _activeGuests = new ConcurrentDictionary<string, Guest>();
            _memberBySession = new ConcurrentDictionary<string, Member>();
            _memberRepo = MemberRepo.GetInstance();
            _paswwordSecurity = new Security();
        }

        public static UserManager GetInstance()
        {
            if (_userManager == null)
                _userManager = new UserManager();
            return _userManager;
        }

        public void Dispose()
        {
            _userManager = new UserManager();

        }

        /// <summary>
        /// Checks the credentials- if user name and password matches a user's credentials
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private Boolean CheckCredentials(string userName, string password)
        {
            if (_memberRepo.ContainsUserName(userName))
            {
                string hashPassword = _memberRepo.GetByUserName(userName).Password;
                bool res = _paswwordSecurity.VerifyPassword(password, hashPassword);
                return res;
            }
            return false;
        }

        /// <summary>
        /// Checks the user name availability.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        private Boolean CheckUserNameAvailability(string userName)
        {
            return !_memberRepo.ContainsUserName(userName);
        }

        /// <summary>
        /// Enters as guest.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public int EnterAsGuest(string sessionID)
        {
            Guest guest = new Guest(_userCounter);
            _activeGuests.TryAdd(sessionID, guest);
            _userCounter++;
            return _userCounter - 1;
        }

        /// <summary>
        /// Logins the specified session identifier- maps the sessionID to the matched member instance
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ArgumentException">Invalid user name or password.</exception>
        public void Login(string sessionID, string username, string password)
        {
            if (CheckCredentials(username, password))
            {
                Member member = _memberRepo.GetByUserName(username);
                Monitor.Enter(_lock);
                try
                {
                    if (IsLoggedIn(member)) throw new Exception("User already logged in");
                    member.LoggedIn = true;
                    _memberBySession.TryAdd(sessionID, member);
                    _activeGuests.TryRemove(sessionID, out Guest _); // return false if he hasn't entered as guest- not exception
                }
                finally { Monitor.Exit(_lock); }
            }
            else
            {
                throw new ArgumentException("Invalid user name or password.");
            }
        }
        public bool IsLoggedIn(Member user)
        {
            return _memberBySession.Values.Contains(user);
        }

        /// <summary>
        /// Logs out the specified member according the session identifier.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        public void Logout(string sessionID)
        {
            if (CheckMemberSessionID(sessionID))
            {
                _memberBySession.TryRemove(sessionID, out Member member);
                if (member != null) member.LoggedIn = false;
            }
        }

        /// <summary>
        /// Registers the guest according to the user name and password.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ArgumentException">User name not available. Please choose another.</exception>
        public void Register(string username, string password)
        {
            if (CheckUserNameAvailability(username) && IsValidPassword(password))
            {
                string hashPassword = _paswwordSecurity.HashPassword(password);
                _memberRepo.Add(new Member(_userCounter, username, hashPassword));
                _userCounter++;
            }
            else
            {
                throw new ArgumentException("User name not available. Please choose another.");
            }
        }
        private bool IsValidPassword(string password)
        {
            if (password.IsNullOrEmpty()) throw new Exception("Password can be empty");
            if (password.Contains(" ")) throw new Exception("password must not contain whitspaces");
            return true;
        }

        /// <summary>
        /// Adds to cart.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shop">The shop.</param>
        /// <param name="productID">The product identifier.</param>
        /// <param name="quantity">The quantity.</param>
        public void AddToCart(string sessionID, Shop shop, int productID, int quantity)
        {
            GetUser(sessionID).AddToCart(shop, productID, quantity);
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopID">The shop identifier.</param>
        /// <param name="productID">The product identifier.</param>
        public void RemoveFromCart(string sessionID, int shopID, int productID)
        {
            GetUser(sessionID).RemoveFromCart(shopID, productID);
        }

        /// <summary>
        /// Gets the shopping cart information.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public ShoppingCart GetShoppingCartInfo(string sessionID)
        {
            return GetUser(sessionID).ShoppingCart;
        }

        /// <summary>
        /// Purchases the specified session identifier.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <param name="shopId">The shop identifier.</param>
        /// <returns></returns>
        public ShoppingCartPurchase Purchase(string sessionID, int shopId)
        {
            return GetUser(sessionID).Purchase(shopId);
        }

        /// <summary>
        /// Checks if the appointee is appointed by the client to this shop
        /// Or the client is founder or owner of the shop
        /// </summary>
        /// <param name="client"></param>
        /// <param name="appointee"></param>
        /// <param name="shopID"></param>
        /// <returns></returns>
        private Boolean CanChangePermissions(Member client, Member appointee, int shopID)
        {
            if (!client.Appointments.ContainsKey(shopID) || !appointee.Appointments.ContainsKey(shopID))
                throw new ArgumentException("This user or the appointee given are not appointed to the given shop");
            if (appointee.Appointments[shopID].Role == Role.Founder)
                throw new ArgumentException("Cannot change permission to shop founder");
            Role clientRole = client.Appointments[shopID].Role;
            Boolean founderOrOwner = clientRole == Role.Owner || clientRole == Role.Founder;
            Boolean appointedByClient = appointee.Appointments[shopID].Appointer.Id == client.Id;
            return founderOrOwner | appointedByClient;

        }

        /// <summary>
        /// Changes the permissions of a member identified by the appointeeUserName.
        /// The change is made by a member identified by the sessionID if he is the appointee's appointer
        /// to the new permissions
        /// to the shopID identified by the shopID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="appointeeUserName"></param>
        /// <param name="shopID"></param>
        /// <param name="permissions"></param>
        public void ChangePermissions(string sessionID, string appointeeUserName, int shopID, Permission permissions)
        {
            Member appointee = _memberRepo.GetByUserName(appointeeUserName);
            Member client = _memberBySession[sessionID];
            if (CanChangePermissions(client, appointee, shopID))
                client.ChangePermissions(appointee, shopID, permissions);
            else throw new ArgumentException($"Failed to change permission to {appointeeUserName}");

        }

        /// <summary>
        /// Adds the permissions of a member identified by the appointeeUserName.
        /// The change is made by a member identified by the sessionID if he is the appointee's appointer
        /// to the new permissions
        /// to the shopID identified by the shopID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="appointeeUserName"></param>
        /// <param name="shopID"></param>
        /// <param name="permissions"></param>
        public void AddPermissions(string sessionID, string appointeeUserName, int shopID, Permission permissions)
        {
            Member appointee = _memberRepo.GetByUserName(appointeeUserName);
            Member client = _memberBySession[sessionID];
            if (CanChangePermissions(client, appointee, shopID))
                client.AddPermissions(appointee, shopID, permissions);
        }

        /// <summary>
        /// Deletes the permissions of a member identified by the appointeeUserName.
        /// The change is made by a member identified by the sessionID if he is the appointee's appointer
        /// to the new permissions
        /// to the shopID identified by the shopID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="appointeeUserName"></param>
        /// <param name="shopID"></param>
        /// <param name="permissions"></param>
        public void DeletePermissions(string sessionID, string appointeeUserName, int shopID, Permission permissions)
        {
            Member appointee = _memberRepo.GetByUserName(appointeeUserName);
            Member client = _memberBySession[sessionID];
            if (CanChangePermissions(client, appointee, shopID))
                client.DeletePermissions(appointee, shopID, permissions);
        }

        /// <summary>
        /// Appoints a member identified by the appointeeID
        /// by a member identified by the sessionID
        /// to the role of the param role
        /// to the shopID identified by the shopID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="appointeeUserName"></param>
        /// <param name="shopID"></param>
        /// <param name="role"></param>
        public void Appoint(string sessionID, string appointeeUserName, Shop shop, Role role, Permission permissions)
        {
            Member appointee = _memberRepo.GetByUserName(appointeeUserName);
            _memberBySession[sessionID].Appoint(appointee, shop, role, permissions);
        }

        public void RemoveAppoint(string sessionID, string appointeeUserName, Shop shop)
        {
            Member appointee = _memberRepo.GetByUserName(appointeeUserName);
            _memberBySession[sessionID].RemoveAppoint(appointee, shop);
        }

        public void CancelMembership(string sessionID, string memberName)
        {
            Member member = _memberRepo.GetByUserName(memberName);

            if (member.Appointments.Count > 0)
                throw new ArgumentException("The member is assigned to one or more shops in the system and therefore the membership can't be canceled");

            if (PendingAgreementsRepo.GetInstance().AppointeeHasPendings(member.Id))
                throw new ArgumentException("The member has a pending appointment agreement in one or more shops in the system and therefore the membership can't be canceled");
            Member outMember;
            string sessionIdToRemove = GetSessionIdByMember(member);
            if (sessionIdToRemove != null)
                _memberBySession.Remove(sessionIdToRemove, out outMember);
            _memberRepo.Delete(member.Id);
        }

        public string GetSessionIdByMember(Member memberValue)
        {
            string sessionId = null;
            foreach (var pair in _memberBySession)
            {
                if (pair.Value.Equals(memberValue))
                {
                    sessionId = pair.Key;
                    break;
                }
            }
            return sessionId;
        }

        public Member GetByUserName(string name)
        {
            return _memberRepo.GetByUserName(name);
        }
        public void NotificationOn(string sessionID)
        {
            _memberBySession[sessionID].NotificationOn();
        }
        public void NotificationOff(string sessionID)
        {
            _memberBySession[sessionID].NotificationOff();
        }
        public List<Message> GetMessages(string sessionID)
        {
            return _memberBySession[sessionID].GetMessages();
        }

        /// <summary>
        /// Checks the member session identifier- throws exception if the user is not logged in.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">In order to execute that action, the user must first be logged in.</exception>
        public Boolean CheckMemberSessionID(string sessionID)
        {
            if (!_memberBySession.ContainsKey(sessionID))
                throw new ArgumentException("In order to execute that action, the user must first be logged in.");
            else
                return true;
        }

        /// <summary>
        /// Checks the session identifier- throws exception if the user is either not logged in or an active guest in the system.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">In order to execute that action, the user must be a visitor in the system.</exception>
        public Boolean CheckSessionID(string sessionID)
        {
            if (!_memberBySession.ContainsKey(sessionID) && !_activeGuests.ContainsKey(sessionID))
                throw new ArgumentException("In order to execute that action, the user must be a visitor in the system.");
            else
                return true;
        }

        public Member GetMember(string sessionID)
        {
            if (_memberBySession.ContainsKey(sessionID))
                return _memberBySession[sessionID];
            else throw new Exception("No Member Found");
        }

        public User GetUser(string sessionID)
        {
            User user = null;
            if (_memberBySession.ContainsKey(sessionID))
                user = _memberBySession[sessionID];
            else if (_activeGuests.ContainsKey(sessionID))
                user = _activeGuests[sessionID];
            return user;
        }
        public void RemoveBasketFromCart(string sessionID, int shopId)
        {
            GetUser(sessionID).RemoveBasketFromCart(shopId);
        }

        public List<Shop> GetUserShops(string sessionID)
        {
            return GetMember(sessionID).GetUserShops();
        }

        public List<Member> GetActiveMembers()
        {
            return _memberBySession.Values.ToList();
        }

        public Member GetMemberBySessionId(string sessionID)
        {
            CheckMemberSessionID(sessionID);
            return _memberBySession[sessionID];
        }

        public void UpdateBasketItemQuantity(User user, int shopId, int productID, int quantity)
        {
            user.UpdateBasketItemQuantity(shopId, productID, quantity);
        }
        internal void ResetDomainData()
        {
            _memberBySession.Clear();
        }
        public void AppointOwner(Shop shop, Member appointee)
        {
            if (!shop.PendingAgreements.ContainsKey(appointee.UserName))
                throw new Exception("Can't appoint an owner without an appointement agreement!");
            Member appointer = shop.PendingAgreements[appointee.UserName].Appointer;
            appointer.AddOwnerAppointment(shop, new Appointment(appointee, shop, appointer, Role.Owner, Permission.All));
        }
    }
}
