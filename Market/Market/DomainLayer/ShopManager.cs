using Market.RepoLayer;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Market.DomainLayer
{
    public class ShopManager
    {
        private static ShopManager _shopManager = null;
        private ShopRepo _shops;
        private int _shopIdFactory;
        private Search _searcher;

        public ShopRepo Shops { get => _shops; set => _shops = value; }

        private ShopManager()
        {
            _shops = ShopRepo.GetInstance();
            _shopIdFactory = 1;
            _searcher = new Search();
        }
        public static ShopManager GetInstance()
        {
            if (_shopManager == null)
                _shopManager = new ShopManager();
            return _shopManager;
        }

        public void Dispose()
        {
            _shopManager = new ShopManager();
        }

        public Shop GetShop(int shopId)
        {
            return _shops.GetById(shopId);
        }
        public void AddDiscountPolicy(int shopID, int userId, string expirationDate, string subject, int ruleId, double precentage)
        {
            Shop shop = _shops.GetById(shopID);
            shop.AddDiscountPolicy(userId, CastDateTime(expirationDate), subject, ruleId, precentage);
        }
        public void AddPurchasePolicy(int shopID, int userId, string expirationDate, string subject, int ruleId)
        {
            Shop shop = _shops.GetById(shopID);
            shop.AddPurchasePolicy(userId, CastDateTime(expirationDate), subject, ruleId);
        }
        public void AddCompositePolicy(int shopID, int userId, string expirationDate, string subject, int Operator, List<int> policies)
        {
            Shop shop = _shops.GetById(shopID);
            shop.AddCompositePolicy(userId, CastDateTime(expirationDate), subject, CastNumericalOperator(Operator), policies);
        }

        public int AddSimpleRule(int shopID, int userId, string subject)
        {
            Shop shop = _shops.GetById(shopID);
            return shop.AddSimpleRule(userId, subject);
        }
        public int AddQuantityRule(int shopID, int userId, string subject, int minQuantity, int maxQuantity)
        {
            Shop shop = _shops.GetById(shopID);
            return shop.AddQuantityRule(userId, subject, minQuantity, maxQuantity);
        }
        public int AddTotalPriceRule(int shopID, int userId, string subject, int targetPrice)
        {
            Shop shop = _shops.GetById(shopID);
            return shop.AddTotalPriceRule(userId, subject, targetPrice);
        }
        public int AddCompositeRule(int shopID, int userId, int Operator, List<int> rules)
        {
            Shop shop = _shops.GetById(shopID);
            return shop.AddCompositeRule(userId, CastLogicalOperator(Operator), rules);
        }
        public void UpdateRuleSubject(int shopID,int userId, int ruleId, string subject)
        {
            Shop shop = _shops.GetById(shopID);
            shop.UpdateRuleSubject(userId, ruleId, subject);
        }
        public void UpdateRuleQuantity(int shopID, int ruleId, int userId, int minQuantity, int maxQuantity)
        {
            Shop shop = _shops.GetById(shopID);
            shop.UpdateRuleQuantity(userId, ruleId, minQuantity, maxQuantity);
        }
        public void UpdateRuleTargetPrice(int shopID, int ruleId, int userId, int targetPrice)
        {
            Shop shop = _shops.GetById(shopID);
            shop.UpdateRuleTargetPrice(userId, ruleId, targetPrice);
        }
        public void UpdateCompositeOperator(int shopID, int ruleId, int userId, int Operator)
        {
            Shop shop = _shops.GetById(shopID);
            shop.UpdateCompositeOperator(userId, ruleId, CastLogicalOperator(Operator));
        }
        public void UpdateCompositeRules(int shopID, int ruleId, int userId, List<int> rules)
        {
            Shop shop = _shops.GetById(shopID);
            shop.UpdateCompositeRules(userId, ruleId, rules);
        }


        public Product AddProduct(int userId,int shopId,string productName,ISell sellMethod,string description, double price, int quantity, string category, SynchronizedCollection<string> keyWords)
        {
            Shop shop = _shops.GetById(shopId);
            return shop.AddProduct(userId, productName, sellMethod, description, price, CastCategory(category), quantity, keyWords);
        }

        public void AddReview(int shopId,int userId, string username ,int productID, double rate, string comment)
        {
            Shop shop = _shops.GetById(shopId);
            shop.AddReview(username, userId, productID, comment, rate);
        }

        public void CloseShop(int userId,int shopId)
        {
            Shop shop = _shops.GetById(shopId);
            shop.CloseShop(userId);
        }

        public Shop CreateShop(Member owner,string shopName)
        {
            if (FreeShopName(shopName))
            {
                Shop newShop = new Shop(_shopIdFactory++, shopName, owner);
                _shops.Add(newShop);
                return newShop;
            }
            else
            {
                throw new Exception("Shop name already exist.");
            }
        }
        private bool FreeShopName(string name)
        {
            foreach(Shop shop in _shops.GetAll())
            {
                if (shop.Name.ToLower().Equals(name.ToLower()))
                    return false;
            }
            return true;
        }

        public string GetMarketInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Market Information:");
            foreach(Shop shop in _shops.GetAll()) {
                sb.AppendLine(shop.GetInfo());
            }
            return sb.ToString();
        }

        public string GetShopInfo(int shopId)
        {
            Shop shop = _shops.GetById(shopId);
            return shop.GetInfo();
        }


        public List<Appointment> getShopPositions(int userId,int shopId)
        {
            Shop shop = _shops.GetById(shopId);
            return shop.GetShopPositions(userId);

        }

        public void OpenShop(int userId, int shopId)
        {
            Shop shop = _shops.GetById(shopId);
            shop.OpenShop(userId);
        }

        public void RemoveProduct(int userId,int shopId,int productID)
        {
            Shop shop = _shops.GetById(shopId);
            shop.RemoveProduct(userId, productID);
        }

        public HashSet<Product> Search(string wordToSearch,SearchType searchType, List<FilterSearchType> filterType)
        { 
            return _searcher.ApplySearch(wordToSearch, searchType, filterType,_shops.GetAllActiveShops());
        }
        public List<Purchase> ShowPurchaseHistory(int userId)
        {
            List<Purchase> result = new List<Purchase>();
            foreach(Shop shop in _shops.GetAll())
            {
                result.AddRange(shop.ShowUserPurchaseHistory(userId));
            }
            return result;
        }

        public List<Purchase> ShowShopHistory(int userId, int shopId)
        {
            Shop shop = _shops.GetById(shopId);
            return shop.ShowShopHistory(userId);
        }

        public void UpdateProductName(int userId, int shopId,int productID, string name)
        {
            Shop shop = _shops.GetById(shopId);
            shop.UpdateProductName(userId, productID, name);
        }

        public void UpdateProductPrice(int userId, int shopId, int productID, double price)
        {
            Shop shop = _shops.GetById(shopId);
            shop.UpdateProductPrice(userId, productID, price);
        }

        public void UpdateProductQuantity(int userId, int shopId, int productID, int qauntity)
        {
            Shop shop = _shops.GetById(shopId);
            shop.UpdateProductQuantity(userId, productID, qauntity);
        }
        public List<Shop> GetAll()
        {
            return _shops.GetAll();
        }
        public List<Shop> GetAllActiveShops()
        {
            return _shops.GetAllActiveShops();
        }
        private Category CastCategory(string categoryName)
        {
            try
            {
                return (Category)Enum.Parse(typeof(Category), categoryName);
            }
            catch (Exception) { return Category.None; }
        }
        private LogicalOperator CastLogicalOperator(int Operator)
        {
            LogicalOperator op = (LogicalOperator)Enum.ToObject(typeof(LogicalOperator), Operator);
            return op;
        }
        private NumericOperator CastNumericalOperator(int Operator)
        {
            NumericOperator op = (NumericOperator)Enum.ToObject(typeof(NumericOperator), Operator);
            return op;
        }
        private DateTime CastDateTime(string expirationDate)
        { 
            DateTime dateTime;
            if (DateTime.TryParse(expirationDate, out dateTime) && dateTime>DateTime.Now)
            {
                return dateTime;
            }
            else throw new Exception("Invalid Date.");
        }
        public void SendMessageToShop(int shopId,Member user, string message)
        {
            Shop shop = GetShop(shopId);
            shop.RecieveMessage(user.UserName, message);
        }

        public void SendReportToShop(int shopId, Member user, string comment)
        {
            Shop shop = GetShop(shopId);
            shop.RecieveReport(user.UserName, comment);
        }
        public Shop GetShopByName(string name)
        {
            return _shops.GetByName(name);
        }

        public void BidOnProduct(Member member, int shopID, int productId, int quantity, double suggestedPriceForOne)
        {
            Shop shop = GetShop(shopID);
            shop.BidOnProduct(member, productId, quantity, suggestedPriceForOne);
        }

        public void ApproveBid(Member member, int shopID, int userId, int productId)
        {
            Shop shop = GetShop(shopID);
            shop.ApproveBid(member, userId,productId);
        }

        public void OfferCounterBid(Member member, int shopID, int userId, int productId, double counterPrice)
        {
            Shop shop = GetShop(shopID);
            shop.OfferCounterBid(member, userId, productId, counterPrice);
        }

        public void ApproveCounterBid(int memberId, int shopId, int productId)
        {
            Shop shop = GetShop(shopId);
            shop.ApproveCounterBid(memberId, productId);
        }

        public void DissapproveBid(Member member, int shopID, int userId, int productId)
        {
            Shop shop = GetShop(shopID);
            shop.DissapproveBid(member, userId, productId);
        }
        public void RemoveBid(Member member, int shopID, int userId, int productId)
        {
            Shop shop = GetShop(shopID);
            shop.RemoveBid(member, userId, productId);
        }

        public void DeclineCounterBid(int id, int shopID, int productId)
        {
            Shop shop = GetShop(shopID);
            shop.DeclineCounterBid(id, productId);
        }
    }
}
