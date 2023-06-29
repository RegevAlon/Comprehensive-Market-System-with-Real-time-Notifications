using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Market.DomainLayer;

namespace Market.ServiceLayer
{
    public interface IMarket
    {
        Response<SProduct> AddProduct(string sessionID, int shopId, string productName, int sellType, string description, double price, int quantity, string category, List<string> keyWords);
        Response AddReview(string sessionID, int shopId, string userName, int productID, double rate, string comment);
        Response AddToCart(string sessionID, int shopId, int productID, int quantity);
        Response Appoint(string sessionID, string appointeeUserName, int shopId, int role, int permission);
        Response RemoveAppoint(string sessionID, string appointeeUserName, int shopID);
        Response ChangePermission(string sessionID, string appointeeUserName, int shopId, int permission);
        Response CloseShop(string sessionID, int shopID);
        Response<SShop> CreateShop(string sessionID, string shopName);
        Response<List<SShop>> GetMarketInfo(string sessionID);
        Response<SShop> GetShopInfo(string sessionID, int shopID);
        Response<List<string>> GetActiveMembers(string sessionID);
        Response<List<string>> GetAllMembers(string sessionID);
        Response<SShoppingCart> GetShoppingCartInfo(string sessionID);
        Response<List<SAppointment>> GetShopPositions(string sessionID, int shopID);
        Response Login(string sessionID, string username, string password);
        Response Logout(string sessionID);
        Response OpenShop(string sessionID, int shopID);
        Response PurchaseBasket(string sessionID, int shopId, string cardNumber, string month, string year, string holder, string ccv, string id,
            string name, string address, string city, string country, string zip);
        Response Register(string sessionID, string username, string password);
        Response RemovePolicy(string sessionID, int shopID, int policyID, string type);
        Response RemoveProduct(string sessionID, int shopId, int productID);
        Response RemoveFromCart(string sessionID, int shopID, int productID);
        Response<List<SProduct>> Search(string sessionID, string word, List<int> searchType, List<int> filterType, int lowPrice, int highPrice, int lowRate, int highRate, string category);
        Response SendMessage(string sessionID, int shopID, string comment);
        Response SendReport(string sessionID, int shopId, string comment);
        Response<List<SPurchase>> ShowMemberPurchaseHistory(string sessionID);
        Response<List<SPurchase>> ShowShopPurchaseHistory(string sessionID, int shopID);
        Response UpdateProductName(string sessionID, int shopId, int productID, string name);
        Response UpdateProductPrice(string sessionID, int shopId, int productID, double price);
        Response UpdateProductQuantity(string sessionID, int shopId, int productID, int quantity);
        Response<SUser> GetUser(string sessionID);
        Response PurchaseShoppingCart(string sessionID, string cardNumber, string month, string year, string holder, string ccv, string id,
            string name, string address, string city, string country, string zip);
        Response<SShop> GetShopByName(string sessionID, string name);
        Response<List<SShop>> GetUserShops(string sessionID);
        Response<string> EnterAsGuest(string sessionID);
        Response NotificationOn(string sessionID);
        Response NotificationOff(string sessionID);
        Response CancelMembership(string sessionID, string memberName);
        Response AddCompositePolicy(string sessionID, int shopID, string expirationdate, string subject, int Operator, List<int> policies);
        Response AddDiscountPolicy(string sessionID, int shopID, string expirationdate, string subject, int ruledId, double precentage);
        Response AddPurchasePolicy(string sessionID, int shopID, string expirationdate, string subject, int ruledId);
        Response UpdateCompositeRules(string sessionID, int shopID, int ruleId, List<int> rules);
        Response UpdateCompositeOperator(string sessionID, int shopID, int ruleId, int Operator);
        Response UpdateRuleTargetPrice(string sessionID, int shopID, int ruleId, int targetPrice);
        Response UpdateRuleQuantity(string sessionID, int shopID, int ruleId, int minQuantity, int maxQuantity);
        Response UpdateRuleSubject(string sessionID, int shopID, int ruleId, string subject);
        Response AddCompositeRule(string sessionID, int shopID, int Operator, List<int> rules);
        Response AddQuantityRule(string sessionID, int shopID, string subject, int minQuantity, int maxQuantity);
        Response AddSimpleRule(string sessionID, int shopID, string subject);
        Response AddTotalPriceRule(string sessionID, int shopID, string subject, int targetPrice);
        Response<List<string>> GetMessages(string sessionID);
        Response<bool> IsSystemAdmin(string sessionID);
        Response UpdateBasketItemQuantity(string sessionID, int shopId, int productID, int quantity);
        Response BidOnProduct(string sessionId, int shopID,int productId,int quantity,double suggestedPriceForOne);
        Response ApproveBid(string sessionId, int shopID,string bidUsername, int productId);
        Response OfferCounterBid(string sessionId,int shopId, string bidUsername, int productId, double counterPrice);
        Response ApproveCounterBid(string sessionId, int shopId, int productId);
        Response DissapproveBid(string sessionId, int shopID, string bidUsername, int productId);
        Response DeclineCounterBid(string sessionId, int shopID, int productId);
        Response RemoveBid(string sessionId, int shopID, string bidUsername, int productId);
        Response ApproveAppointment(string session, int shopId, string appointeeUserName);
        Response DeclineAppointment(string session, int shopId, string appointeeUserName);
        Response<int> GetMessagesNumberRequest(string sessionId);
        Response<int> GetShoppingCartAmount(string sessionId);
    }
}
