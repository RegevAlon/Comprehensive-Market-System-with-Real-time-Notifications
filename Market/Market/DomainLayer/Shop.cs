using Market.DomainLayer.Rules;
using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using Market.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using Microsoft.EntityFrameworkCore;
using Market.DataLayer.DTOs.Rules;
using System.Security.Cryptography;
using Microsoft.SqlServer.Management.Smo;

namespace Market.DomainLayer
{
    public class Shop
    {
        private int _id;
        private string _name;
        private bool _active;
        private SynchronizedCollection<Product> _products;
        //private string _bankAccount;
        private ConcurrentDictionary<int, Appointment> _appointments;           //<userId, Appointment>
        private ConcurrentDictionary<int, IRule> _rules;
        private SynchronizedCollection<Purchase> _purchases;
        private EventManager _eventManager;
        private DiscountPolicyManager _discountPolicyManager;
        private PurchasePolicyManager _purchasePolicyManager;
        private long _productIdFactory;
        private int _purchaseIdFactory;
        private int _rulesIdFactory;
        private int _policyIdFactory;
        private double _rating;
        private object _lockobject;
        private ShopDirector _shopDirector;
        private ConcurrentDictionary<string, PendingAgreement> _pendingAgreements;       //<userName, PendingAgreements>

        public Shop(int shopId, string name, Member member)
        {
            _id = shopId;
            _name = name;
            _active = true;
            _products = ProductRepo.GetInstance().GetShopProducts(_id);
            _appointments = AppointmentRepo.GetInstance().GetShopAppointments(_id);
            _rules = RuleRepo.GetInstance().GetShopRules(_id);
            _purchases = PurchaseRepo.GetInstance().GetShopPurchaseHistory(_id);
            _eventManager = new EventManager(_id);
            _discountPolicyManager = new DiscountPolicyManager(shopId);
            _purchasePolicyManager = new PurchasePolicyManager(shopId);
            _rules = new ConcurrentDictionary<int, IRule>();
            _productIdFactory = 1;
            _purchaseIdFactory = 1;
            _rulesIdFactory = 1;
            _policyIdFactory = 1;
            _rating = 0;
            _lockobject = new object();
            _shopDirector = new ShopDirector(_id);
            _pendingAgreements = new ConcurrentDictionary<string, PendingAgreement>();
        }
        public Shop(ShopDTO shopDTO)
        {
            _id = shopDTO.Id;
            _name = shopDTO.Name;
            _active = shopDTO.Active;
            _products = new SynchronizedCollection<Product>();
            _appointments = new ConcurrentDictionary<int, Appointment>();
            _rules = new ConcurrentDictionary<int, IRule>();
            _purchases = new SynchronizedCollection<Purchase>();
            _discountPolicyManager = new DiscountPolicyManager(shopDTO.Id);
            _purchasePolicyManager = new PurchasePolicyManager(shopDTO.Id);
            _rules = new ConcurrentDictionary<int, IRule>();
            _rating = shopDTO.Rating;
            _lockobject = new object();
            _pendingAgreements = new ConcurrentDictionary<string, PendingAgreement>();
        }
        public void InitializeComplexfeilds(ShopDTO shopDTO)
        {
            List<AppointmentDTO> appDtos = MarketContext.GetInstance().Appointments.Where((app) => app.ShopId == _id).ToList();
            _products = ProductRepo.GetInstance().GetShopProducts(_id);
            _purchases = PurchaseRepo.GetInstance().GetShopPurchaseHistory(_id);
            _rules = RuleRepo.GetInstance().GetShopRules(_id);
            _shopDirector = new ShopDirector(shopDTO.Id, _rulesIdFactory);
            _eventManager = new EventManager(_id);
            _rulesIdFactory = 1;
            _purchaseIdFactory = 1;
            _productIdFactory = 1;
            _policyIdFactory = 1;
            foreach (AppointmentDTO appdto in appDtos)
                _appointments.TryAdd(appdto.MemberId, AppointmentRepo.GetInstance()
                    .GetById(appdto.MemberId, appdto.ShopId));

            if (_products.Count > 0)
                _productIdFactory = shopDTO.Products.Max((p) => p.Id) + 1;
            if (_purchases.Count > 0)
                _purchaseIdFactory = shopDTO.Purchases.Max((p) => p.Id) + 1;
            if (_rules.Count() > 0)
                _rulesIdFactory = shopDTO.Rules.Max((r) => r.Id) + 1;
            if (_discountPolicyManager.Policies.Count() > 0)
                _policyIdFactory = shopDTO.Policies.Max((p) => p.Id) + 1;
            _pendingAgreements = PendingAgreementsRepo.GetInstance().GetShopPendingAgreements(_id);
        }

        public int Id { get => _id; }
        public ConcurrentDictionary<int, Appointment> Appointments { get => _appointments; set => _appointments = value; }
        public SynchronizedCollection<Product> Products { get => _products; set => _products = value; }
        public bool Active { get => _active; set => _active = value; }
        public double Rating { get => _rating; set => _rating = value; }
        public string Name { get => _name; set => _name = value; }
        public SynchronizedCollection<Purchase> Purchases { get => _purchases; set => _purchases = value; }
        public ConcurrentDictionary<int, IRule> Rules { get => _rules; set => _rules = value; }
        public DiscountPolicyManager DiscountPolicyManager { get => _discountPolicyManager; set => _discountPolicyManager = value; }
        public PurchasePolicyManager PurchasePolicyManager { get => _purchasePolicyManager; set => _purchasePolicyManager = value; }
        public EventManager EventManager { get => _eventManager; set => _eventManager = value; }
        public ConcurrentDictionary<string, PendingAgreement> PendingAgreements { get => _pendingAgreements; set => _pendingAgreements = value; }


        /// <summary>
        /// collect shop details: id and products details
        /// </summary>
        /// <returns></returns> return a String contains the shop Id and all of its products info
        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Permission exception for userId: {_id}");
            foreach (Product product in _products)
            {
                sb.AppendLine(product.GetInfo());
            }
            return sb.ToString();
        }
        
        public void RemoveAppointment(Member member, Member memberToRemove)
        {
            if (_appointments.Count <= 1)
                throw new Exception("A shop must have at least one manager! The appointment removal can't be done.");
            Appointment outAppointment;
            _appointments.TryRemove(memberToRemove.Id, out outAppointment);
            if (IsOwnerOrFounder(outAppointment))
                RemoveOwnerFromBidProducts(outAppointment.Member.UserName);
            RemoveAppointmentEvent e = new RemoveAppointmentEvent(this, member, memberToRemove);
            _eventManager.NotifySubscribers(e);
            _eventManager.UnsubscribeToAll(member);
        }

        private void RemoveOwnerFromBidProducts(string username)
        {
            foreach(Product product in _products)
            {
                if (product.SellMethod.GetType().Name.Equals("BidSell"))
                {
                    List<Bid> newApprovedProductBids = ((BidSell)product.SellMethod).RemoveOwnerFromBid(username);
                    foreach (Bid bid in newApprovedProductBids) HandelAllApproveBid(bid, product);
                }
            }
        }
        private void AddOwnerToBidProducts(Member owner)
        {
            foreach (Product product in _products)
            {
                if (product.SellMethod.GetType().Name.Equals("BidSell"))
                    ((BidSell)product.SellMethod).AddOwnerToBid(owner);
            }
        }
        private bool IsOwnerOrFounder(Appointment app)
        {
            return app.Role == Role.Owner || app.Role == Role.Founder;
        }

        /// <summary>
        /// Add new appointment to appointments dictionary
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newAppointment"></param>
        public void AddAppointment(Member user, Appointment newAppointment)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (user.Id != newAppointment.Member.Id || this.Id != newAppointment.Shop.Id)
                    throw new Exception($"The Appointment added doesn't match the user parameter or this shop");
                if (_appointments.TryAdd(user.Id, newAppointment))
                    SignApointeeToEvents(user);
                if (newAppointment.Appointer != null)
                {
                    AddAppointmentEvent e = new AddAppointmentEvent(this, newAppointment.Appointer, user, newAppointment);
                    _eventManager.NotifySubscribers(e);
                }
                if (IsOwnerOrFounder(newAppointment))
                {
                    AddOwnerToBidProducts(newAppointment.Member);
                    SignOwnerToEvents(user);
                }

            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void UpdtaeAppointment(Member user, Appointment newAppointment)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (user.Id != newAppointment.Member.Id || this.Id != newAppointment.Shop.Id)
                    throw new Exception($"The Appointment added doesn't match the user parameter or this shop");
                if (!_appointments.ContainsKey(user.Id) || !_appointments.TryUpdate(user.Id, newAppointment, _appointments[user.Id]))
                    return;
                if (newAppointment.Appointer != null)
                {
                    AddAppointmentEvent e = new AddAppointmentEvent(this, newAppointment.Appointer, user, newAppointment);
                    _eventManager.NotifySubscribers(e);
                }
                if (IsOwnerOrFounder(newAppointment))
                {
                    AddOwnerToBidProducts(newAppointment.Member);
                    SignOwnerToEvents(user);
                }

            }
            finally { Monitor.Exit(_lockobject); }
        }

        /// <summary>
        /// find all product in the shop that contain at least one of the keywords from the parameter list
        /// </summary>
        /// <param name="keywords"></param> keyword we want to look for
        /// <returns></returns> list of the products in the shopID which satisfy the condition.
        public List<Product> SearchByKeywords(string keywords)
        {
            return _products.ToList().FindAll((p) => p.ContainKeyword(keywords));
        }
        /// <summary>
        /// find all product in the shop that there name contains the name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns> list of the products in the shopID which satisfy the condition.
        public List<Product> SearchByName(string name)
        {
            string lowerName = name.ToLower();
            return _products.ToList().FindAll((p) => p.Name.ToLower().Contains(lowerName) || lowerName.Contains(p.Name.ToLower()));
        }
        /// <summary>
        /// find all product in the shop that there category fits the category parameter
        /// </summary>
        /// <param name="catagory"></param>
        /// <returns></returns> list of the products in the shop which satisfy the condition.
        public List<Product> SearchByCategory(Category category)
        {
            return _products.ToList().FindAll((p) => ((category & p.Category) == category));
        }

        /// <summary>
        /// checks if the user has the permission and add a new product to product list.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param> name of the product
        /// <param name="description"></param> description of the product
        /// <param name="price"></param> price of the product
        /// <param name="quantity"></param> quantity of the product
        /// <exception cref="Exception"></exception> if user does not have the right permissions.
        public Product AddProduct(int userId, string name,ISell sellMethod, string description, double price, Category category, int quantity, SynchronizedCollection<string> keywords)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.ManageSupply) && IsUniqueProductName(name)
                    && ValidProductName(name) && ValidProductPrice(price) && ValidProductQuantity(quantity))
                {
                    int productId = GenerateUniqueProductId();
                    Product newProduct = new Product(productId, this.Id, name, sellMethod, description, price, category, quantity, keywords);
                    AddProduct(newProduct);
                    return newProduct;
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }

        /// <summary>
        /// add product to _products and to product repo
        /// </summary>
        /// <param name="p"></param>
        private void AddProduct(Product p)
        {
            _products.Add(p);
            ProductRepo.GetInstance().Add(p);
        }
        private void AddRule(IRule rule)
        {
            _rules.TryAdd(rule.Id, rule);
            RuleRepo.GetInstance().Add(rule);
        }
        /// <summary>
        /// checks if the user has the permission and remove a product from product list.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <exception cref="Exception"></exception> if user does not have the right permissions or pId does not exist
        public void RemoveProduct(int userId, int productId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.ManageSupply))
                {
                    Product productToRemove = GetProduct(productId);
                    IsNotNullProduct(productToRemove);
                    
                    RemoveProduct(productToRemove);  
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        private void RemoveProduct(Product p)
        {
            _products.Remove(p);
            ProductRepo.GetInstance().Delete(p.Id);
        }

        public bool CheckInSupply(int productId, int quantity)
        {
            Product p = FindProduct(productId);
            if (p.Quantity >= quantity)
                return true;
            throw new Exception("Not enough quantity in shop supply");
        }
        /// <summary>
        /// get product by ID
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private Product GetProduct(int productId)
        {
            return _products.ToList().Find((p) => p.Id == productId);
        }
        private IRule GetRule(int ruleId)
        {

            if (_rules.TryGetValue(ruleId, out IRule rule))
            {
                return rule;
            }
            else
                throw new Exception($"No Rule matches ruleId: {ruleId}");

        }
        /// <summary>
        /// Add a discount/purchase rule to discount/purchase policyManager if the user has the permissions
        /// </summary>
        /// <param name="userId"></param> userId the want to add the rule
        /// <param name="rule"></param>
        /// <exception cref="Exception"></exception> No permission exception.
        public void AddPurchasePolicy(int userId, DateTime expirationDate, string subject, int ruledId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruledId);
                    _purchasePolicyManager.AddPolicy(_policyIdFactory++, expirationDate, CastProductOrCategory(subject), rule);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void AddDiscountPolicy(int userId, DateTime expirationDate, string subject, int ruledId, double precantage)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruledId);
                    _discountPolicyManager.AddPolicy(_policyIdFactory++, expirationDate, CastProductOrCategory(subject), rule, precantage);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }

        public void AddCompositePolicy(int userId, DateTime expirationDate, string subject, NumericOperator Operator, List<int> policies)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _discountPolicyManager.AddCompositePolicy(_policyIdFactory, expirationDate, CastProductOrCategory(subject), Operator, policies);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }

        public void RemovePolicy(int userId, int policyId, string type)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    switch (type)
                    {
                        case "DiscountPolicy": _discountPolicyManager.RemovePolicy(policyId); break;
                        case "PurchasePolicy": _purchasePolicyManager.RemovePolicy(policyId); break;
                    }
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// Add a discount/purchase rule to discount/purchase policyManager if the user has the permissions
        /// </summary>
        /// <param name="userId"></param> userId the want to add the rule
        /// <param name="rule"></param>
        /// <exception cref="Exception"></exception> No permission exception.
        public int AddSimpleRule(int userId, string subject)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _shopDirector.setFeatures(CastProductOrCategory(subject));
                    IRule newRule = _shopDirector.makeRule(typeof(SimpleRule));
                    AddRule(newRule);
                    return newRule.Id;

                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public int AddQuantityRule(int userId, string subject, int minQuantity, int maxQuantity)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _shopDirector.setFeatures(CastProductOrCategory(subject), minQuantity, maxQuantity);
                    IRule newRule = _shopDirector.makeRule(typeof(QuantityRule));
                    AddRule(newRule);
                    return newRule.Id;

                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public int AddTotalPriceRule(int userId, string subject, int targetPrice)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _shopDirector.setFeatures(CastProductOrCategory(subject), targetPrice);
                    IRule newRule = _shopDirector.makeRule(typeof(TotalPriceRule));
                    AddRule(newRule);
                    return newRule.Id;

                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public int AddCompositeRule(int userId, LogicalOperator Operator, List<int> rules)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    List<IRule> rulesToAdd = new List<IRule>();
                    foreach (int id in rules)
                    {
                        rulesToAdd.Add(GetRule(id));
                    }

                    _shopDirector.setFeatures(Operator, rulesToAdd);
                    IRule newRule = _shopDirector.makeRule(typeof(CompositeRule));
                    AddRule(newRule);
                    return newRule.Id;

                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void RemoveRule(int userId, int ruleId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule RuletToRemove = GetRule(ruleId);
                    foreach (IRule rule in _rules.Values)
                    {
                        if (typeof(IRule).Name.Equals("CompositeRule"))
                        {
                            removeRuleFromComposite((CompositeRule)rule, RuletToRemove);
                        }
                    }
                    RemoveRule(ruleId);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void removeRuleFromComposite(CompositeRule rule, IRule RuletToRemove)
        {
            foreach (IRule checkedRule in rule.Rules)
            {
                if (checkedRule.Id.Equals(RuletToRemove.Id))
                {
                    rule.Rules.Remove(RuletToRemove);
                    RuleRepo.GetInstance().Update(rule);
                }
            }
        }
        private void RemoveRule(int ruleId)
        {
            if (_rules.ContainsKey(ruleId))
            {
                _rules.TryRemove(ruleId, out IRule rule);
                RuleRepo.GetInstance().Delete(rule.Id);
            }
        }
        public void UpdateRuleSubject(int userId, int ruleId, string subject)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruleId);
                    rule.Subject = CastProductOrCategory(subject);
                    RuleRepo.GetInstance().Update(rule);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void UpdateRuleQuantity(int userId, int ruleId, int minQuantity, int maxQuantity)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruleId);
                    ((QuantityRule)rule).MinQuantity = minQuantity;
                    ((QuantityRule)rule).MaxQuantity = maxQuantity;
                    RuleRepo.GetInstance().Update(rule);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void UpdateRuleTargetPrice(int userId, int ruleId, int targetPrice)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruleId);
                    ((TotalPriceRule)rule).TotalPrice = targetPrice;
                    RuleRepo.GetInstance().Update(rule);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void UpdateCompositeOperator(int userId, int ruleId, LogicalOperator Operator)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruleId);
                    ((CompositeRule)rule).Operator = Operator;
                    RuleRepo.GetInstance().Update(rule);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        public void UpdateCompositeRules(int userId, int ruleId, List<int> rules)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    IRule rule = GetRule(ruleId);
                    ((CompositeRule)rule).Rules.Clear();
                    foreach (int id in rules)
                    {
                        ((CompositeRule)rule).AddRule(GetRule(ruleId));
                    }
                    RuleRepo.GetInstance().Update(rule);

                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// remove a discount rule from discountPolicyManager if the user has the permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="policyId"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveDiscountPolicy(int userId, int policyId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _discountPolicyManager.RemovePolicy(policyId);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// remove a purchase rule from purchasePolicyManager if the user has the permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="policyId"></param>
        /// <exception cref="Exception"></exception>
        public void RemovePurchasePolicy(int userId, int policyId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (HasPermission(userId, Permission.Policy))
                {
                    _purchasePolicyManager.RemovePolicy(policyId);
                }
                else throw new Exception($"Permission exception for userId: {userId}");
            }
            finally { Monitor.Exit(_lockobject); }
        }

        public void AddReview(string username, int userId, int productId, string comment, double rate)
        {
            Monitor.Enter(_lockobject);
            try
            {
                Product productToAddReview = GetProduct(productId);
                if (productToAddReview != null)
                    productToAddReview.AddReview(userId, username, comment, rate);
                else throw new Exception("No product with this Id");
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// check if userId has the permission to see the shop history and if so, gets all purchase history of the shop
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns> string contain all the info of the purchases that was in the shop
        /// <exception cref="Exception"></exception>
        public List<Purchase> ShowShopHistory(int userId)
        {
            if (HasPermission(userId, Permission.ShopPurchaseHistory))
            {
                return _purchases.ToList();
            }
            else throw new Exception($"Permission exception for userId: {userId}");
        }
        /// <summary>
        /// get all user purchases in the shop
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Purchase> ShowUserPurchaseHistory(int userId)
        {
            List<Purchase> result = new List<Purchase>();
            foreach (Purchase purchase in _purchases)
            {
                if (purchase.ShopId.Equals(_id)) result.Add(purchase);
            }
            return result;
        }
        /// <summary>
        /// gets all of the appointments of the shop
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Appointment> GetShopPositions(int userId)
        {
            if (HasPermission(userId, Permission.ShopAppointmentsInfo))
            {
                return new List<Appointment>(_appointments.Values.ToList());
            }
            else throw new Exception(String.Format("Permission exception for userId: %d", userId));
        }

        /// <summary>
        /// check if userId exist in shop appointments and if he has the specific permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns> return true if appointment exist and has the specific permission.
        private bool HasPermission(int userId, Permission permission)
        {
            return (_appointments.ContainsKey(userId) && (_appointments[userId].HasPermission(permission)));
        }
        /// <summary>
        /// check if basket standing with purchasePolicy demands. 
        /// update basket price by discount policy, create a new pending purchase and
        /// adding it to purchase repo 
        /// ********* SHOULD BE LOCKED WHEN APPLIED ********
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="basket"></param>
        /// <returns></returns> a new pending purchase related to the basket.
        public Purchase PurchaseBasket(int userId, Basket basket)
        {
            if (!_active)
                throw new Exception($"Shop: {_name} is not active anymore");
            Monitor.Enter(_lockobject);
            try
            {
                basket.resetDiscount();
                _purchasePolicyManager.Apply(basket);
                _discountPolicyManager.Apply(basket);
                if (BasketInSupply(basket))
                {
                    RemoveBasketProductsFromSupply(basket);
                }
                Purchase pendingPurchase = new Purchase(GenerateUniquePurchaseId(), _id, userId, basket.Clone());
                AddPurchase(pendingPurchase);
                return pendingPurchase;
            }
            finally { Monitor.Exit(_lockobject); }
        }
        private int GenerateUniqueProductId()
        {
            return int.Parse($"{_id}{_productIdFactory++}");
        }
        private int GenerateUniquePurchaseId()
        {
            return int.Parse($"{_id}{_purchaseIdFactory++}");
        }
        private void AddPurchase(Purchase p)
        {
            Monitor.Enter(_lockobject);
            try
            {
                _purchases.Add(p);
                PurchaseRepo.GetInstance().Add(p);
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// update the purchase status to success and notify the shop managers about the event
        /// </summary>
        /// <param name="purchase"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void PurchaseSuccessHandler(Purchase purchase)
        {
            purchase.PurchaseStatus = PurchaseStatus.Success;
            PurchaseRepo.GetInstance().Update(purchase);
            foreach (BasketItem basketItem in purchase.Basket.BasketItems)
            {
                _eventManager.NotifySubscribers(
                    new ProductSellEvent(this, basketItem));
                if (basketItem.Product.SellMethod is BidSell)
                {
                    BidSell bidSell = (BidSell)basketItem.Product.SellMethod;
                    bidSell.RemoveBid(purchase.BuyerId);
                }

            }
        }
        /// <summary>
        /// update the purchase status to failed and update the quantity of the shop's products
        /// </summary>
        /// <param name="purchase"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void PurchaseFailHandler(Purchase purchase)
        {
            Monitor.Enter(_lockobject);
            try
            {
                purchase.PurchaseStatus = PurchaseStatus.Failed;
                AddBasketProductsToSupply(purchase.Basket);
                PurchaseRepo.GetInstance().Update(purchase);
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// get the product by Id, checks if the is enough in the supply and add to basket and check discounts if so.
        /// </summary>
        /// <param name="basket"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <exception cref="Exception"></exception>
        public void AddProductToBasket(Basket basket, int productId, int quantity)
        {
            Product productToAdd = GetProduct(productId);
            if (productToAdd.CanAddToCart() && ProductInSupply(productToAdd, quantity))
            {
                basket.AddProduct(productToAdd, quantity);
            }
            else throw new Exception($"Product {productToAdd.Name}: In supply: {productToAdd.Quantity}, You required: {quantity}");
        }

        public void AddProductToBasket(Basket basket, Product productToAdd, int quantity)
        {
            if (ProductInSupply(productToAdd, quantity))
            {
                basket.AddProduct(productToAdd, quantity);
            }
            else throw new Exception($"Product {productToAdd.Name}: In supply: {productToAdd.Quantity}, You required: {quantity}");
        }
        /// <summary>
        /// checks if all the product and required quantity in supply
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool BasketInSupply(Basket basket)
        {
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                Product productToBuy = basketItem.Product;
                int quantity = basketItem.Quantity;
                if (!ProductInSupply(productToBuy, quantity))
                {
                    throw new ArgumentException($"Product {productToBuy.Name}: In supply: {productToBuy.Quantity}, You required: {quantity}");
                }
            }
            return true;
        }
        /// <summary>
        /// remove all the required product quantity of the user from the supply
        /// </summary>
        /// <param name="basket"></param>
        /// <exception cref="Exception"></exception>
        private void RemoveBasketProductsFromSupply(Basket basket)
        {
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                Product productToBuy = basketItem.Product;
                int quantity = basketItem.Quantity;
                if (ProductInSupply(productToBuy, quantity))
                {
                    productToBuy.Quantity -= quantity;
                    ProductRepo.GetInstance().Update(productToBuy);
                }
                else throw new Exception("This should not happened");
            }
        }
        /// <summary>
        /// Adds the to product quantity all of the product amount in the basket.
        /// </summary>
        /// <param name="basket"></param>
        private void AddBasketProductsToSupply(Basket basket)
        {
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                Product productToBuy = FindProduct(basketItem.Product.Id);
                if (productToBuy != null)
                {
                    int quantity = basketItem.Quantity;
                    productToBuy.Quantity += quantity;
                    ProductRepo.GetInstance().Update(productToBuy);
                }
            }
        }
        private Product FindProduct(int productId)
        {
            foreach (Product p in Products)
            {
                if (p.Id.Equals(productId))
                    return p;
            }
            return null;
        }
        /// <summary>
        /// check if product as at least <paramref name="quantity"/> amount in the supply
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private bool ProductInSupply(Product product, int quantity)
        {
            if (_products.Contains(product))
                return quantity <= product.Quantity;
            throw new Exception($"Product Name: \'{product.Name}\' Id: {product.Id} not exist in shop.");
        }
        /// <summary>
        /// check if <paramref name="userId"/> has the permission to close the shop and if so close the shop and notify subscribers.
        /// </summary>
        /// <param name="userId"></param>
        public void CloseShop(int userId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (!HasPermission(userId, Permission.OpenCloseShop))
                    throw new Exception("No Permission");
                if (!_active)
                    throw new Exception("Shop already closed.");
                Active = false;
                ShopClosedEvent e = new ShopClosedEvent(_appointments[userId].Member);
                _eventManager.NotifySubscribers(e);

                //=====================Save To DB===================
                MarketContext context = MarketContext.GetInstance();
                ShopDTO shopDto = context.Shops.Find(_id);
                if (shopDto != null)
                {
                    shopDto.Active = _active;
                    context.SaveChanges();
                }
                //==================================================
            }
            finally { Monitor.Exit(_lockobject); }
        }
        /// <summary>
        /// check if <paramref name="userId"/> has the permission to open the shop and if so open the shop and notify subscribers.
        /// </summary>
        /// <param name="userId"></param>
        public void OpenShop(int userId)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (!HasPermission(userId, Permission.OpenCloseShop))
                    throw new Exception("No Permission");
                if (_appointments.Count < 1)
                    throw new Exception("To open the store there must be at least one store manager/ owner/ founder");
                if (_active)
                    throw new Exception("Shop already open.");
                Active = true;
                ShopOpenEvent e = new ShopOpenEvent(_appointments[userId].Member);
                _eventManager.NotifySubscribers(e);

                //=====================Save To DB===================
                MarketContext context = MarketContext.GetInstance();
                ShopDTO shopDto = context.Shops.Find(_id);
                if (shopDto != null)
                {
                    shopDto.Active = _active;
                    context.SaveChanges();
                }
                //==================================================
            }
            finally { Monitor.Exit(_lockobject); }


        }

        public void UpdateProductName(int userId, int productID, string name)
        {
            if (HasPermission(userId, Permission.ManageSupply))
            {
                Product productToUpdate = GetProduct(productID);
                if (productToUpdate != null && ValidProductName(name) && IsUniqueProductName(name))
                {
                    productToUpdate.Name = name;
                    ProductRepo.GetInstance().Update(productToUpdate);
                }
                else throw new Exception("Invalid product Id");
            }
            else throw new Exception($"Permission exception for userId: {userId}");
        }

        public void UpdateProductPrice(int userId, int productID, double price)
        {
            if (HasPermission(userId, Permission.ManageSupply))
            {
                Product productToUpdate = GetProduct(productID);
                if (productToUpdate != null && ValidProductPrice(price))
                {
                    productToUpdate.Price = price;
                    ProductRepo.GetInstance().Update(productToUpdate);
                }
                else throw new Exception("Invalid product Id");
            }
            else throw new Exception($"Permission exception for userId: {userId}");
        }

        public void ApplyDiscountsOnBasket(Basket basket)
        {
            _discountPolicyManager.Apply(basket);
        }

        public void UpdateProductQuantity(int userId, int productID, int qauntity)
        {
            if (HasPermission(userId, Permission.ManageSupply))
            {
                Product productToUpdate = GetProduct(productID);
                if (productToUpdate != null && ValidProductQuantity(qauntity))
                {
                    productToUpdate.Quantity = qauntity;
                    ProductRepo.GetInstance().Update(productToUpdate);
                }
                else throw new Exception("Invalid product Id");
            }
            else throw new Exception(String.Format("Permission exception for userId: %d", userId));
        }
        private void SignApointeeToEvents(Member user)
        {
            Monitor.Enter(_lockobject);
            try
            {
                _eventManager.SubscribeToAll(user);
            }
            finally { Monitor.Exit(_lockobject); }
        }

        public void RecieveMessage(string userName, string comment)
        {
            ValidCommentLength(comment);
            _eventManager.NotifySubscribers(new MessageEvent(userName, comment));
        }

        public void RecieveReport(string userName, string comment)
        {
            Monitor.Enter(_lockobject);
            try
            {
                ValidCommentLength(comment);
                _eventManager.NotifySubscribers(new ReportEvent(userName, comment));
            }
            finally { Monitor.Exit(_lockobject); }
        }
        private bool ValidProductQuantity(int quantity)
        {
            if (quantity < 0) throw new Exception("Invalid Product quantity");
            return true;
        }
        private bool ValidProductPrice(double price)
        {
            if (price > 0) return true;
            throw new Exception("invalid product price");
        }
        private bool ValidProductName(string name)
        {
            if (name.Length == 0) throw new Exception("Invalid Product Name");
            
            return true;
        }
        private bool IsUniqueProductName(string name)
        {
            foreach (Product p in _products)
                if (p.Name.ToLower().Equals(name.ToLower()))
                    throw new Exception("This shop already has product woth this name.");
            return true;
        }
        private bool ValidCommentLength(string msg)
        {
            if (msg.Length == 0) throw new Exception("Invalid Product Name");
            return true;
        }
        private RuleSubject CastProductOrCategory(string subject)
        {

            Category category;
            if (Enum.TryParse(subject, out category) && Enum.IsDefined(typeof(Category), category))
                return new RuleSubject(category);
            else
            {
                Product product = null;
                foreach (Product p in _products)
                {
                    if (p.Name.ToLower().Equals(subject.ToLower()))
                    {
                        product = p;
                        break;
                    }
                }
                if (product != null)
                    return new RuleSubject(product);
                else throw new Exception("could not find subject");
            }
        }

        public void BidOnProduct(Member member, int productId, int quantity, double suggestedPriceForOne)
        {
            Product productToBid = FindProduct(productId);
            if (productToBid == null) throw new Exception($"Cannot find product with Id {productId}");
            if(productToBid.CanBid() && CheckInSupply(productId, quantity) && ValidProductPrice(suggestedPriceForOne))
            {
                Bid newBid = new Bid(productToBid.Id,member, getOwnersList(), quantity, suggestedPriceForOne);
                ((BidSell)productToBid.SellMethod).AddBid(newBid);
                _eventManager.NotifySubscribers(new ProductBidEvent(this, productToBid, newBid));

                MarketContext.GetInstance().Products.Find(productId).Bids.Add(new BidDTO(newBid));
                MarketContext.GetInstance().SaveChanges();

            }
        }
        private List<Member> getOwnersList()
        {
            List<Member> ownersUsername = new List<Member>();
            foreach(Appointment app in _appointments.Values)
            {
                if (app.Role == Role.Owner || app.Role == Role.Founder)
                    ownersUsername.Add(app.Member);
            }
            return ownersUsername;
        }

        private bool IsOwnerOrFounder(Member member)
        {
            return (HasRole(member, Role.Owner) || HasRole(member, Role.Founder)); 
        }

        public void ApproveBid(Member member, int userId, int productId)
        {
            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (!IsOwnerOrFounder(member))
                throw new Exception("Permission Exception: only owners and founders allowed to approve bids.");

            if(IsBidSellProduct(product))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                bidsell.ApproveBid(userId,member.UserName);
                Bid bid = bidsell.GetBid(userId);
                if (bid.AllApproved())
                {
                    HandelAllApproveBid(bid, product);
                }
            }
        }
        private void HandelAllApproveBid(Bid bid, Product product)
        {
            BidSell bidsell = ((BidSell)product.SellMethod);
            bid.BiddingMember.AddBidProductToCart(this, bid, product);
            bid.BiddingMember.Notify($"Bid approved. Shop: {_name}, Product: \'{product.Name}\' added to your cart");
        }
        private bool IsNotNullProduct(Product product)
        {
            if (product == null)
                throw new Exception($"Cannot find product with Id {product.Id}");
            return true;

        }
        public bool IsBidSellProduct(Product product)
        {
            return product.SellMethod is BidSell;
        }
        private bool HasRole(Member m, Role role)
        {
            foreach(Appointment app in _appointments.Values)
            {
                if (app.Member.Id == m.Id)
                    return app.Role.Equals(role);
            }
            return false;
        }

        public void OfferCounterBid(Member member, int userId, int productId, double counterPrice)
        {
            if (!HasPermission(member.Id, Permission.BidsPermissions) && !IsOwnerOrFounder(member))
                throw new Exception("Permission Exception: Do not have Bids Permissions");

            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (IsBidSellProduct(product) && ValidProductPrice(counterPrice))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                double oldPrice = bidsell.OfferCounterBid(userId, member.UserName, counterPrice);
                Bid bid = bidsell.GetBid(userId);
                Event e = new ProductCounterBidEvent(this, product, bid, member.UserName, oldPrice);
                _eventManager.NotifySubscribers(e);
                bid.BiddingMember.Notify(e.GenerateMsg());                
            }
        }

        public void ApproveCounterBid(int memberId, int productId)
        {
            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (IsBidSellProduct(product))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                bidsell.ApproveCounterBid(memberId);
                Bid bid = bidsell.GetBid(memberId);
                if (bid.AllApproved())
                {
                    HandelAllApproveBid(bid, product);
                }
            }
        }

        public void DissapproveBid(Member member, int userId, int productId)
        {
            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (!IsOwnerOrFounder(member))
                throw new Exception("Permission Exception: only owners and founders allowed to approve bids.");

            if (IsBidSellProduct(product))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                bidsell.DissapproveBid(userId, member.UserName);
            }
        }

        public void RemoveBid(Member member, int userId, int productId)
        {
            bool isBidCreator = member.Id == userId;
            if (HasPermission(member.Id, Permission.BidsPermissions) || isBidCreator || IsOwnerOrFounder(member))
            {
                Product product = FindProduct(productId);

                IsNotNullProduct(product);

                if (IsBidSellProduct(product))
                {
                    BidSell bidsell = ((BidSell)product.SellMethod);
                    Bid removedBid = bidsell.RemoveBid(userId);
                    removedBid.BiddingMember.Notify($"Bid dissapproved. Your bid in Shop: {_name}, on Product: \'{product.Name}\' dissapproved and removed");
                }
            }
            else throw new Exception("Permission Exception: member does not have permission to remove bid.");
        }

        public void RemoveBid(int userId, int productId)
        {
            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (IsBidSellProduct(product))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                bidsell.RemoveBid(userId);
            }

        }
        public void AddPendingAgreement(PendingAgreement pendingAgreement)
        {
            Monitor.Enter(_lockobject);
            try
            {
                if (!Appointments.ContainsKey(pendingAgreement.Appointer.Id))
                    throw new Exception($"The Appointer is not an owner in the shop and therefor can't add an appointmetn agreement!");
                if (!_pendingAgreements.TryAdd(pendingAgreement.Appointee.UserName, pendingAgreement))
                    throw new Exception($"There is already a pending appointment agreement for this user in this shop");
                PendingAgreementEvent e = new PendingAgreementEvent(this, pendingAgreement.Appointer, pendingAgreement.Appointee);
                _eventManager.NotifySubscribers(e);
            }
            finally { Monitor.Exit(_lockobject); }
        }

        public double CalculateShopRating()
        {
            double totalRatings = 0;
            int numReviews = 0;

            foreach (Product product in Products)
            {
                foreach (Review review in product.Reviews)
                {
                    if(review.Rate > 0)
                    {
                        totalRatings += review.Rate;
                        numReviews++;
                    }
                }
            }

            if (numReviews > 0)
            {
                double averageRating = (double)totalRatings / numReviews;
                return averageRating;
            }

            return 0.0; // Default rating if there are no reviews
        }

        private void SignOwnerToEvents(Member user)
        {
            Monitor.Enter(_lockobject);
            try
            {
                //_eventManager.SubscribeToAll(user);
                _eventManager.SubscribePendingAgreementEvent(user);

            }
            finally { Monitor.Exit(_lockobject); }
        }
        public bool AddApproval(Member client, string appointeeUserName)
        {
            if (!Appointments.ContainsKey(client.Id) || (Appointments[client.Id].Role != Role.Owner && Appointments[client.Id].Role != Role.Founder))
                throw new ArgumentNullException("To execute this action te client must be a owner in the shop");
            if (!PendingAgreements.ContainsKey(appointeeUserName))
                throw new ArgumentNullException("No pending appointment agreement exist for the apointee: " + appointeeUserName + " , int the shop: " + this.Name);
            PendingAgreements[appointeeUserName].AddApproval(client);
            PendingAgreementsRepo.GetInstance().AddApproval(client, PendingAgreements[appointeeUserName], this);
            return PendingAgreements[appointeeUserName].CheckIfApproved();
        }

        public void AddDecline(Member client, string appointeeUserName)
        {
            if (!Appointments.ContainsKey(client.Id) || (Appointments[client.Id].Role != Role.Owner && Appointments[client.Id].Role != Role.Founder))
                throw new ArgumentNullException("To execute this action te client must be a owner in the shop");
            if (!PendingAgreements.ContainsKey(appointeeUserName))
                throw new ArgumentNullException("No pending appointment agreement exist for the apointee: " + appointeeUserName + " , int the shop: " + this.Name);
            PendingAgreements[appointeeUserName].AddDecline(client);
            PendingAgreementsRepo.GetInstance().AddDeclined(client, PendingAgreements[appointeeUserName], this);
        }

        public void DeletePendingAgreement(string appointeeUserName)
        {
            PendingAgreement outPending;
            _pendingAgreements.Remove(appointeeUserName, out outPending);
        }

        public void DeclineCounterBid(int memberId, int productId)
        {
            Product product = FindProduct(productId);

            IsNotNullProduct(product);

            if (IsBidSellProduct(product))
            {
                BidSell bidsell = ((BidSell)product.SellMethod);
                Bid bid = bidsell.GetBid(memberId);
                bidsell.RemoveBid(memberId);
                _eventManager.NotifySubscribers(new CounterBidDeclinedEvent(this, product, bid));
            }
        }
    }
}