using Market.DomainLayer;
using Market.ServiceLayer;
using System.Data;

namespace ServerMarket.API
{
    public class EnterAsGuestRequest : IRequest
    {
        public EnterAsGuestRequest()
        {
        }
    }
    public class AddProductRequest : IRequest
    {
        public int ShopId { get; set; }
        public string ProductName { get; set; }
        public int SellType { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public List<string> KeyWords { get; set; }

        public AddProductRequest(int shopId, string productName, int sellType, string description, double price, int quantity, string category, List<string> keyWords)
        {
            ShopId = shopId;
            ProductName = productName;
            SellType = sellType;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
            KeyWords = keyWords;
        }
    }
    public class AddDiscountRequest : IRequest
    {
        public int ShopID { get; set; }
        public int DiscountType { get; set; }
        public List<int> Products { get; set; }
        public double Percentage { get; set; }

        public AddDiscountRequest(int shopId, int discountType, List<int> products, double percentage)
        {
            ShopID = shopId;
            DiscountType = discountType;
            Products = products;
            Percentage = percentage;
        }
    }
    public class AddPolicyRequest : IRequest
    {
        public int ShopID { get; set; }
        public string Description { get; set; }

        public AddPolicyRequest(int shopID, string description)
        {
            ShopID = shopID;
            Description = description;
        }
    }
    public class AddReviewRequest : IRequest
    {
        public int ShopID { get; set; }
        public string UserName { get; set; }
        public int ProductID { get; set; }
        public double Rate { get; set; }
        public string Comment { get; set; }

        public AddReviewRequest(int shopID, string userName, int productID, double rate, string comment)
        {

            ShopID = shopID;
            UserName = userName;
            ProductID = productID;
            Rate = rate;
            Comment = comment;
        }
    }
    public class AddToCartRequest : IRequest
    {

        public int ShopID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public AddToCartRequest(int shopID, int productID, int quantity)
        {
            ShopID = shopID;
            ProductID = productID;
            Quantity = quantity;
        }
    }

    public class AppointRequest : IRequest
    {

        public string AppointeeUserName { get; set; }
        public int ShopID { get; set; }
        public int Role { get; set; }
        public int Permission { get; set; }

        public AppointRequest(string appointeeUserName, int shopID, int role, int permission)
        {

            AppointeeUserName = appointeeUserName;
            ShopID = shopID;
            Role = role;
            Permission = permission;
        }
    }
    public class ChangePermisionRequest : IRequest
    {

        public string AppointeeUserName { get; set; }
        public int ShopId { get; set; }
        public int Permission { get; set; }

        public ChangePermisionRequest(string appointeeUserName, int shopId, int permission)
        {

            AppointeeUserName = appointeeUserName;
            ShopId = shopId;
            Permission = permission;
        }
    }
    public class CloseShopRequest : IRequest
    {

        public int ShopID { get; set; }

        public CloseShopRequest(int shopID)
        {

            ShopID = shopID;
        }
    }
    public class CreateShopRequest : IRequest
    {

        public string ShopName { get; set; }

        public CreateShopRequest(string shopName)
        {

            ShopName = shopName;
        }
    }
    public class GetMarketInfoRequest : IRequest
    {


        public GetMarketInfoRequest()
        {

        }
    }

    public class GetShopInfoRequest : IRequest
    {

        public int ShopID { get; set; }

        public GetShopInfoRequest(int shopID)
        {

            ShopID = shopID;
        }
    }

    public class GetShoppingCartInfoRequest : IRequest
    {


        public GetShoppingCartInfoRequest()
        {

        }
    }
    public class GetShoppingCartAmount : IRequest
    {


        public GetShoppingCartAmount()
        {

        }
    }

    public class GetShopPositionsRequest : IRequest
    {

        public int ShopID { get; set; }

        public GetShopPositionsRequest(int shopID)
        {

            ShopID = shopID;
        }
    }

    public class LoginRequest : IRequest
    {

        public string Username { get; set; }
        public string Password { get; set; }

        public LoginRequest(string username, string password)
        {

            Username = username;
            Password = password;
        }
    }

    public class LogoutRequest : IRequest
    {


        public LogoutRequest()
        {

        }
    }

    public class OpenShopRequest : IRequest
    {

        public int ShopID { get; set; }

        public OpenShopRequest(int shopID)
        {

            ShopID = shopID;
        }
    }

    public class PurchaseBasketRequest : IRequest
    {


        public int ShopId { get; set; }

        public string zip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string ccv { get; set; }
        public string holder { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string cardNumber { get; set; }
        public PurchaseBasketRequest(int shopId, string zip, string country, string city, string address, string name, string id, string ccv, string holder, string year, string month, string cardNumber)
        {
            ShopId = shopId;
            this.zip = zip;
            this.country = country;
            this.city = city;
            this.address = address;
            this.name = name;
            this.id = id;
            this.ccv = ccv;
            this.holder = holder;
            this.year = year;
            this.month = month;
            this.cardNumber = cardNumber;
        }

    }

    public class RegisterRequest : IRequest
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public RegisterRequest(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
    }

    public class RemoveDiscountRequest : IRequest
    {
        public RemoveDiscountRequest(int shopId, int discountID)
        {

            ShopId = shopId;
            DiscountID = discountID;
        }


        public int ShopId { get; set; }
        public int DiscountID { get; set; }
    }


    public class RemovePolicyRequest : IRequest
    {

        public int ShopID { get; set; }
        public int PolicyID { get; set; }
        public string Type { get; set; }

    }

    public class RemoveProductRequest : IRequest
    {

        public int ShopID { get; set; }
        public int ProductID { get; set; }

        public RemoveProductRequest() { }

        public RemoveProductRequest(int shopID, int productID)
        {

            ShopID = shopID;
            ProductID = productID;
        }
    }

    public class RemoveFromCartRequest : IRequest
    {

        public int ShopID { get; set; }
        public int ProductID { get; set; }

        public RemoveFromCartRequest() { }

        public RemoveFromCartRequest(int shopID, int productID)
        {

            ShopID = shopID;
            ProductID = productID;
        }
    }
    /*
        Response AddCompositePolicy(string sessionID, int shopID, string expirationdate, int Operator, List<int> policies, double precentage);
        Response AddDiscountPolicy(string sessionID, int shopID, string expirationdate, int ruledId, double precentage);
        Response AddPurchasePolicy(string sessionID, int shopID, string expirationdate, int ruledId);
         */
    public class AddCompositePolicyRequest : IRequest
    {
        public AddCompositePolicyRequest(int shopID, string expirationDate, string subject, int op, List<int> policies)
        {
            ShopID = shopID;
            ExpirationDate = expirationDate;
            Op = op;
            Subject = subject;
            Policies = policies;
        }

        public int ShopID { get; set; }
        public string Subject { get; set; }
        public string ExpirationDate { get; set; }
        public int Op { get; set; }
        public List<int> Policies { get; set; }

    }


    public class AddDiscountPolicyRequest : IRequest
    {
        public AddDiscountPolicyRequest(int shopID, string expirationdate, string subject, int ruleId, double percentage)
        {
            ShopID = shopID;
            Expirationdate = expirationdate;
            RuleId = ruleId; // Corrected property name
            Percentage = percentage;
            Subject = subject;
        }

        public int ShopID { get; set; }
        public string Expirationdate { get; set; }
        public string Subject { get; set; }
        public int RuleId { get; set; } // Corrected property name
        public double Percentage { get; set; }
    }

    public class AddPurchasePolicyRequest : IRequest
    {
        public AddPurchasePolicyRequest(int shopID, string expirationdate, string subject, int ruleId)
        {
            ShopID = shopID;
            Expirationdate = expirationdate;
            RuleId = ruleId;
            Subject = subject;
        }

        public int ShopID { get; set; }
        public string Expirationdate { get; set; }
        public int RuleId { get; set; }
        public string Subject { get; set; }
    }
    public class AddCompositeRuleRequest : IRequest
    {
        public AddCompositeRuleRequest(int shopID, int op, List<int> rules)
        {
            ShopID = shopID;
            Op = op;
            Rules = rules;
        }

        public int ShopID { get; set; }
        public int Op { get; set; }
        public List<int> Rules { get; set; }
    }

    public class AddQuantityRuleRequest : IRequest
    {
        public AddQuantityRuleRequest(int shopID, string subject, int minQuantity, int maxQuantity)
        {
            ShopID = shopID;
            Subject = subject;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
        }

        public int ShopID { get; set; }
        public string Subject { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }

    public class AddSimpleRuleRequest : IRequest
    {
        public AddSimpleRuleRequest(int shopID, string subject)
        {
            ShopID = shopID;
            Subject = subject;

        }

        public int ShopID { get; set; }
        public string Subject { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
    public class AddTotalPriceRuleRequest : IRequest
    {
        public AddTotalPriceRuleRequest(int shopID, string subject, int targetPrice)
        {
            ShopID = shopID;
            Subject = subject;
            TargetPrice = targetPrice;
        }

        public int ShopID { get; set; }
        public string Subject { get; set; }
        public int TargetPrice { get; set; }
    }

    public class CancelMembershipRequest : IRequest
    {

        public string MemberUserName { get; set; }

        public CancelMembershipRequest(string memberUserName)
        {

            MemberUserName = memberUserName;
        }
    }

    public class SearchRequest : IRequest
    {

        public string Word { get; set; }
        public List<int> SearchType { get; set; }
        public List<int> FilterType { get; set; }
        public int LowPrice { get; set; }
        public int HighPrice { get; set; }
        public int LowRate { get; set; }
        public int HighRate { get; set; }
        public string Category { get; set; }

        public SearchRequest() { }

        public SearchRequest(string word, List<int> searchType, List<int> filterType, int lowPrice, int highPrice, int lowRate, int highRate, string category)
        {

            Word = word;
            SearchType = searchType;
            FilterType = filterType;
            LowPrice = lowPrice;
            HighPrice = highPrice;
            LowRate = lowRate;
            HighRate = highRate;
            Category = category;
        }
    }

    public class SendMessageRequest : IRequest
    {

        public int ShopID { get; set; }
        public string Comment { get; set; }

        public SendMessageRequest() { }

        public SendMessageRequest(int shopID, string comment)
        {

            ShopID = shopID;
            Comment = comment;
        }
    }

    public class SendReportRequest : IRequest
    {

        public int ShopId { get; set; }
        public string Comment { get; set; }

        public SendReportRequest() { }

        public SendReportRequest(int shopId, string comment)
        {

            ShopId = shopId;
            Comment = comment;
        }
    }

    public class ShowMemberPurchaseHistoryRequest : IRequest
    {


        public ShowMemberPurchaseHistoryRequest() { }

        public ShowMemberPurchaseHistoryRequest(string sessionID)
        {

        }
    }

    public class ShowShopPurchaseHistoryRequest : IRequest
    {

        public int ShopID { get; set; }

        public ShowShopPurchaseHistoryRequest() { }

        public ShowShopPurchaseHistoryRequest(int shopID)
        {

            ShopID = shopID;
        }
    }

    public class UpdateProductNameRequest : IRequest
    {

        public int ShopId { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }

        public UpdateProductNameRequest() { }

        public UpdateProductNameRequest(int shopId, int productID, string name)
        {

            ShopId = shopId;
            ProductID = productID;
            Name = name;
        }
    }

    public class UpdateProductPriceRequest : IRequest
    {

        public int ShopId { get; set; }
        public int ProductID { get; set; }
        public double Price { get; set; }

        public UpdateProductPriceRequest() { }

        public UpdateProductPriceRequest(int shopId, int productID, double price)
        {

            ShopId = shopId;
            ProductID = productID;
            Price = price;
        }
    }

    public class UpdateProductQuantityRequest : IRequest
    {

        public int ShopId { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public UpdateProductQuantityRequest() { }

        public UpdateProductQuantityRequest(int shopId, int productID, int quantity)
        {

            ShopId = shopId;
            ProductID = productID;
            Quantity = quantity;
        }
    }

    public class UpdateCompositeRulesRequest : IRequest
    {
        public int ShopId { get; set; }
        public int RuleId { get; set; }
        public List<int> Rules { get; set; }

        public UpdateCompositeRulesRequest(int shopId, int ruleId, List<int> rules)
        {

            ShopId = shopId;
            RuleId = ruleId;
            Rules = rules;
        }
    }
    public class UpdateRuleQuantityRequest : IRequest
    {
        public int ShopId { get; set; }
        public int RuleId { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public UpdateRuleQuantityRequest(int shopId, int ruleId, int minQuantity, int maxQuantity)
        {
            ShopId = shopId;
            RuleId = ruleId;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
        }
    }
    public class UpdateRuleSubjectRequest : IRequest
    {
        public int ShopId { get; set; }
        public int RuleId { get; set; }
        public string Subject { get; set; }
        public UpdateRuleSubjectRequest(int shopId, int ruleId, string subject)
        {
            ShopId = shopId;
            RuleId = ruleId;
            Subject = subject;
        }
    }
    public class UpdateCompositeOperatorRequest : IRequest
    {
        public int ShopId { get; set; }
        public int RuleId { get; set; }
        public int Operator { get; set; }
        public UpdateCompositeOperatorRequest(int shopId, int ruleId, int op)
        {
            ShopId = shopId;
            RuleId = ruleId;
            Operator = op;
        }
    }

    public class GetUserRequest : IRequest
    {


        public GetUserRequest() { }

        public GetUserRequest(string sessionID)
        {

        }
    }

    public class PurchaseShoppingCartRequest : IRequest
    {
        public PurchaseShoppingCartRequest(string zip, string country, string city, string address, string name, string id, string ccv, string holder, string year, string month, string cardNumber)
        {
            this.zip = zip;
            this.country = country;
            this.city = city;
            this.address = address;
            this.name = name;
            this.id = id;
            this.ccv = ccv;
            this.holder = holder;
            this.year = year;
            this.month = month;
            this.cardNumber = cardNumber;
        }

        public string zip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string ccv { get; set; }
        public string holder { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string cardNumber { get; set; }
        public PurchaseShoppingCartRequest() { }
    }

    public class GetShopByNameRequest : IRequest
    {

        public string Name { get; set; }

        public GetShopByNameRequest() { }

        public GetShopByNameRequest(string name)
        {

            Name = name;
        }
    }

    public class GetUserShopsRequest : IRequest
    {
        public GetUserShopsRequest() { }
    }

    public class GetActiveMembersRequest : IRequest
    {
        public GetActiveMembersRequest() { }
    }

    public class GetAllMembersRequest : IRequest
    {
        public GetAllMembersRequest() { }
    }

    public class GetMessagesRequest : IRequest
    {
        public GetMessagesRequest() { }
    }
    public class GetMessagesNumberRequest : IRequest
    {
        public GetMessagesNumberRequest() { }
    }
    public class NotificationOnRequest : IRequest
    {
        public NotificationOnRequest() { }
    }
    public class NotificationOffRequest : IRequest
    {
        public NotificationOffRequest() { }
    }

    public class IsAdminRequest : IRequest
    {
        public IsAdminRequest() { }
    }

    public class UpdateBasketQuantityRequest : IRequest
    {

        public int ShopId { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public UpdateBasketQuantityRequest() { }

        public UpdateBasketQuantityRequest(int shopId, int productID, int quantity)
        {

            ShopId = shopId;
            ProductID = productID;
            Quantity = quantity;
        }
    }

    public class SetProductBidRequest : IRequest
    {
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double SuggestedPriceForOne { get; set; }

        public SetProductBidRequest() { }

        public SetProductBidRequest(int shopId, int productId, int quantity, double suggestedPriceForOne)
        {
            ShopId = shopId;
            ProductId = productId;
            Quantity = quantity;
            SuggestedPriceForOne = suggestedPriceForOne;
        }
    }

    public class ApproveBidRequest : IRequest
    {
        public int ShopId { get; set; }
        public string BidUsername { get; set; }
        public int ProductId { get; set; }
        public ApproveBidRequest() { }

        public ApproveBidRequest(int shopId, string bidUsername, int productId)
        {
            ShopId = shopId;
            BidUsername = bidUsername;
            ProductId = productId;
        }
    }

    public class DeclineBidRequest : IRequest
    {
        public int ShopId { get; set; }
        public string BidUsername { get; set; }
        public int ProductId { get; set; }
        public DeclineBidRequest() { }

        public DeclineBidRequest(int shopId, string bidUsername, int productId)
        {
            ShopId = shopId;
            BidUsername = bidUsername;
            ProductId = productId;
        }
    }

    public class OfferCounterBidRequest : IRequest
    {

        public int ShopId { get; set; }
        public string BidUsername { get; set; }
        public int ProductId { get; set; }
        public int CounterPrice { get; set; }
        public OfferCounterBidRequest() { }

        public OfferCounterBidRequest(int shopId, string bidUsername, int productId, int counterPrice)
        {
            ShopId = shopId;
            BidUsername = bidUsername;
            ProductId = productId;
            CounterPrice = counterPrice;
        }
    }

    public class RemoveBidRequest : IRequest
    {

        public int ShopId { get; set; }
        public string BidUsername { get; set; }
        public int ProductId { get; set; }
        public RemoveBidRequest() { }

        public RemoveBidRequest(int shopId, string bidUsername, int productId)
        {
            ShopId = shopId;
            BidUsername = bidUsername;
            ProductId = productId;
        }
    }

    public class ApproveCounterBidRequest : IRequest
    {
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public ApproveCounterBidRequest() { }

        public ApproveCounterBidRequest(int shopId, int productId)
        {
            ShopId = shopId;
            ProductId = productId;
        }
    }

    public class ApproveAppointmentRequest : IRequest
    {
        public int ShopId { get; set; }
        public string AppointeeUsername { get; set; }
        public ApproveAppointmentRequest() { }

        public ApproveAppointmentRequest(int shopId, string appointeeUsername)
        {
            ShopId = shopId;
            AppointeeUsername = appointeeUsername;
        }
    }

    public class DeclineAppointmentRequest : IRequest
    {
        public int ShopId { get; set; }
        public string AppointeeUsername { get; set; }
        public DeclineAppointmentRequest() { }

        public DeclineAppointmentRequest(int shopId, string appointeeUsername)
        {
            ShopId = shopId;
            AppointeeUsername = appointeeUsername;
        }
    }
    public class DeclineCounterBidRequest : IRequest
    {
        public int ShopId { get; set; }
        public int ProductId { get; set; }
        public DeclineCounterBidRequest() { }

        public DeclineCounterBidRequest(int shopId, int productId)
        {
            ShopId = shopId;
            ProductId = productId;
        }
    }
}