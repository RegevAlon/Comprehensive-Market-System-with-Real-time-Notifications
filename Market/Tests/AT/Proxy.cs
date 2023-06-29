using Market.DomainLayer;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Market.AT
{
    public class Proxy
    {
        MarketService service;
        string sessid = "2";
        public Proxy()
        {
            service = MarketService.GetInstance();
        }
        public bool AddSystemAdmin(string adminUserName)
        {
            Response res = service.AddSystemAdmin(adminUserName);
            return !res.ErrorOccured;
        }
        public bool PurchaseCart(string sessionID)
        {
            Response res = service.PurchaseShoppingCart(sessionID, "2222333344445555", "12", "2023", "Israel Israelovice", "242", "205893654","ben","ff","as","israel","6794141");
            return !res.ErrorOccured;
        }
        public bool Register(string sessionID,string username, string password)
        {
            Response res=service.Register(sessionID,username,password);
            return !res.ErrorOccured;

        }
        public void Dispose()
        {
            service.Dispose();
        }
        public bool Logout(string sessionID)
        {
            Response res = service.Logout(sessionID);
            return !res.ErrorOccured;
        }
        public bool SubsribeEvent(string sessionID, int shopID, string Event)
        {
            //not implemented yet
            return true;
        }

        public bool Login(string sessionID, string username, string password)
        {
            Response res = service.Login(sessionID, username, password);
            return !res.ErrorOccured;
        }

        public bool RemoveAppoint(string sessionID, string username, int shopId)
        {
            Response res = service.RemoveAppoint(sessionID, username, shopId);
            return !res.ErrorOccured;
        }

        public bool RemoveProduct(string sessionID, int shopID, int productID)
        {
            Response res = service.RemoveProduct(sessionID, shopID, productID);
            return !res.ErrorOccured;
        }

        public List<SShop> GetMarketInfo(string sessionID)
        {
            Response<List<SShop>> shops = service.GetMarketInfo(sessionID);
            return shops.Value;
        }
        public List<SProduct> SearchProducts(string sessionID, string word, List<int> searchType, List<int> filterType, int lowPrice, int highPrice, int lowRate, int highRate, string category)
        {
            Response<List<SProduct>> res = service.Search(sessionID, word, searchType, filterType, lowPrice, highPrice, lowRate, highRate, category);
            return res.Value;
        }
        public bool AddProduct(string sessionID, int shopId, string productName, string description, double price, int quantity, string category, List<string> keyWords)
        {
            Response<SProduct> res = service.AddProduct(sessionID, shopId, productName,0, description, price, quantity, category, keyWords);
            return !res.ErrorOccured;
        }

        public bool AreDictionariesEqual<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            foreach (var kvp in dict1)
            {
                if (!dict2.TryGetValue(kvp.Key, out var value) || !value.Equals(kvp.Value))
                {
                    return false;
                }
            }

            return true;
        }


        public bool createShop(string sessid1, string shop1)
        {
            Response res = service.CreateShop(sessid1, shop1);
            return !res.ErrorOccured;
        }
        public string getSessionId()
        {
            sessid = (int.Parse(sessid) + 1).ToString();
            return sessid;
        }
        /*
        public bool checkEqualProducts(string productName, string description, double price, int quantity, string category, List<string> keyWords, SProduct prod)
        {
            if ((productName == null) || (productName != prod.Name) || (description != prod.Description) ||
                (price != prod.Price) || (quantity != prod.Quantity) || (category != prod.Category))
            {
                return false;
            }
            return true;
        }
        */
        public bool AddToCart(string sessionID, int shopId, int productID, int quantity)
        {
            Response res = service.AddToCart(sessionID, shopId, productID, quantity);
            return !res.ErrorOccured;
        }

        public bool changePermission(string sessionID, string appointerUserName, int shopId, int permission)
        {
            Response res = service.ChangePermission(sessionID, appointerUserName, shopId, permission);
            return !res.ErrorOccured;
        }
        public SShoppingCart GetShoppingCartInfo(string sessionID)
        {
            Response<SShoppingCart> res = service.GetShoppingCartInfo(sessionID);
            if (res.ErrorOccured)
                return null;
            return res.Value;
        }

        public bool EnterAsGuest(string sessionID)
        {
            Response res = service.EnterAsGuest(sessionID);
            return !res.ErrorOccured;
        }
        public bool closeShop(string sessionID, int shopID)
        {
            Response res = service.CloseShop(sessionID, shopID);
            return !res.ErrorOccured;
        }

        public bool GetShopPositions(string sessionID, int shopID)
        {
            Response res = service.GetShopPositions(sessionID, shopID);
            return !res.ErrorOccured;
        }

        public bool UpdateProductPrice(string sessionID, int shopId, int productID, double price)
        {
            Response res = service.UpdateProductPrice(sessionID, shopId, productID, price);
            return !res.ErrorOccured;
        }

        public bool ShowPurchaseHistory(string sessionID)
        {
            Response res = service.ShowMemberPurchaseHistory(sessionID);
            return !res.ErrorOccured;
        }
        public bool Appoint(string sessionID, string userName, int shopId, int role, int permission)
        {
            Response res = service.Appoint(sessionID, userName, shopId, role, permission);
            return !res.ErrorOccured;
        }

        public bool AddPurchasePolicy(string sessionID, int shopId, string expirationDate, string subject, int ruleId)
        {
            Response res = service.AddPurchasePolicy(sessionID, shopId, expirationDate, subject, ruleId);
            return !res.ErrorOccured;
        }
        public bool RemovePolicy(string sessionID, int shopId, int policyId, string type)
        {
            Response res = service.RemovePolicy(sessionID, shopId, policyId, type);
            return !res.ErrorOccured;
        }


        public bool AddSimpleRule(string sessionID, int shopId, string subject)
        {
            Response res = service.AddSimpleRule(sessionID, shopId, subject);
            return !res.ErrorOccured;
        }
        public bool UpdateRuleSubject(string sessionID, int shopId, int ruleId, string subject)
        {
            Response res = service.UpdateRuleSubject(sessionID, shopId, ruleId, subject);
            return !res.ErrorOccured;
        }
        //TODO: Implement check if server is up.
        public bool OpenMarket(string sessionID)
        {
            return true;
        }

        public bool AcceptPayment(string primarySessionID)
        {
            return true;
        }

        public bool GetPaymentMethods(string primarySessionID, int shopID)
        {
            return false;
        }

        public List<string> GetAllMembers(string sessionID)
        {
            Response<List<string>> res = service.GetAllMembers(sessionID);
            if (!res.ErrorOccured)
                return res.Value;
            return null;

        }
        public List<string> GetActiveMembers(string sessionID)
        {
            Response<List<string>> res = service.GetActiveMembers(sessionID);
            if (!res.ErrorOccured)
                return res.Value;
            return null;
        }

        public bool CancelMembership(string sessionID, string memberName)
        {
            Response res = service.CancelMembership(sessionID, memberName);
            return !res.ErrorOccured;
        }
        public List<string> GetMessages(string sessionID)
        {
            Response<List<string>> res = service.GetMessages(sessionID);
            return res.Value;
        }
        public bool Notification_on(string sessionID)
        {
            Response res = service.NotificationOn(sessionID);
            return !res.ErrorOccured;
        }
        public bool Notification_off(string sessionID)
        {
            Response res = service.NotificationOff(sessionID);
            return !res.ErrorOccured;
        }
        public List<SProduct> Notification_off(string sessionID, string word, List<int> searchType, List<int> filterType, int lowPrice, int highPrice, int lowRate, int highRate, string category)
        {
            Response<List<SProduct>> res = service.Search( sessionID,  word,  searchType, filterType,  lowPrice,  highPrice,  lowRate,  highRate,  category);
            return res.Value;
        }

    }


}