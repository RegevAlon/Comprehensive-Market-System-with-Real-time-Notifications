using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DomainLayer;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Xml.Linq;
using System.Net;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.SqlServer.Management.XEvent;
using Newtonsoft.Json.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Market.ServiceLayer
{

    public class MarketService : IMarket
    {
        private MarketManager _marketManager;
        private static Logger _logger;
        private static MarketService _marketService = null;
        private static object _lock = new object();
        private static MarketService _instance = null;

        private MarketService()
        {
            _marketManager = MarketManager.GetInstance();
            _logger = new Logger(new TimestampedTextWriterTraceListener(File.Open("eventlog.txt", FileMode.Append)),
                                 new TimestampedTextWriterTraceListener(File.Open("errorlog.txt", FileMode.Append)));
        }

        public static MarketService GetInstance()
        {
            if (_marketService == null)
            {
                lock (_lock)
                {
                    if (_marketService == null)
                    {
                        _marketService = new MarketService();
                    }
                }
            }
            return _marketService;
        }

        public void Dispose()
        {
            _marketManager.Dispose();
            _logger.Close();
            _marketService = new MarketService();
        }

        public Response<string> EnterAsGuest(string sessionID)
        {
            try
            {
                _marketManager.EnterAsGuest(sessionID);
                _logger.Log($"EnterAsGuest: Guest entered with sessionID {sessionID}");
                return Response<string>.FromValue(sessionID); ;
            }
            catch (Exception e)
            {
                _logger.Error($"EnterAsGuest: Error occurred: {e.Message}");
                return Response<string>.FromError(e.Message);
            }
        }
        public Response AddSimpleRule(string sessionID, int shopID, string subject)
        {
            try
            {
                _marketManager.AddSimpleRule(sessionID, shopID, subject);
                _logger.Log($"AddRule: Rule added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddRule: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response AddQuantityRule(string sessionID, int shopID, string subject, int minQuantity, int maxQuantity)
        {
            try
            {
                _marketManager.AddQuantityRule(sessionID, shopID, subject, minQuantity, maxQuantity);
                _logger.Log($"AddRule: Rule added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddRule: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response AddTotalPriceRule(string sessionID, int shopID, string subject, int targetPrice)
        {
            try
            {
                _marketManager.AddTotalPriceRule(sessionID, shopID, subject, targetPrice);
                _logger.Log($"AddRule: Rule added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddRule: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response AddCompositeRule(string sessionID, int shopID, int Operator, List<int> rules)
        {
            try
            {
                _marketManager.AddCompositeRule(sessionID, shopID, Operator, rules);
                _logger.Log($"AddRule: Rule added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddRule: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response UpdateRuleSubject(string sessionID, int shopID, int ruleId, string subject)
        {
            try
            {
                _marketManager.UpdateRuleSubject(sessionID, shopID, ruleId, subject);
                _logger.Log($"UpdateRuleSubject: Rule Updated successfully for shopID {shopID} with subject {subject}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("UpdateRuleSubject: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response UpdateRuleQuantity(string sessionID, int shopID, int ruleId, int minQuantity, int maxQuantity)
        {
            try
            {
                _marketManager.UpdateRuleQuantity(sessionID, shopID, ruleId, minQuantity, maxQuantity);
                _logger.Log($"UpdateRuleSubject: Rule Updated successfully for shopID {shopID} with minQuantity {minQuantity} and maxQuantity {maxQuantity}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("UpdateRuleQuantity: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }
        public Response<List<string>> GetMessages(string sessionID)
        {
            try
            {
                List<Message> messages = _marketManager.GetMessages(sessionID);
                List<string> res = new List<string>();
                foreach (Message message in messages) { res.Add(message.Comment); }
                _logger.Log($"Get Messages: get messages for sessionId: {sessionID}");
                return Response<List<string>>.FromValue(res);
            }
            catch (Exception e)
            {
                _logger.Error($"Get Messages: Error occured for session id: {sessionID}");
                return Response<List<string>>.FromError(e.Message);
            }
        }

        public Response UpdateRuleTargetPrice(string sessionID, int shopID, int ruleId, int targetPrice)
        {
            try
            {
                _marketManager.UpdateRuleTargetPrice(sessionID, shopID, ruleId, targetPrice);
                _logger.Log($"UpdateRuleSubject: Rule Updated successfully for shopID {shopID} with targetPrice {targetPrice}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("UpdateRuleTargetPrice: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response UpdateCompositeOperator(string sessionID, int shopID, int ruleId, int Operator)
        {
            try
            {
                _marketManager.UpdateCompositeOperator(sessionID, shopID, ruleId, Operator);
                _logger.Log($"UpdateRuleSubject: Rule Updated successfully for shopID {shopID} with Operator {Operator}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("UpdateCompositeOperator: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response UpdateCompositeRules(string sessionID, int shopID, int ruleId, List<int> rules)
        {
            try
            {
                _marketManager.UpdateCompositeRules(sessionID, shopID, ruleId, rules);
                _logger.Log($"UpdateCompositeRules: Rule Updated successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("UpdateCompositeRules: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }
        public Response AddPurchasePolicy(string sessionID, int shopID, string expirationdate, string subject, int ruledId)
        {
            try
            {
                _marketManager.AddPurchasePolicy(sessionID, shopID, expirationdate, subject, ruledId);
                _logger.Log($"AddPolicy: Policy added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddPolicy: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response AddDiscountPolicy(string sessionID, int shopID, string expirationdate, string subject, int ruledId, double precentage)
        {
            try
            {
                _marketManager.AddDiscountPolicy(sessionID, shopID, expirationdate, subject, ruledId, precentage);
                _logger.Log($"AddPolicy: Policy added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddPolicy: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }
        public Response AddCompositePolicy(string sessionID, int shopID, string expirationdate, string subject, int Operator, List<int> policies)
        {
            try
            {
                _marketManager.AddCompositePolicy(sessionID, shopID, expirationdate, subject, Operator, policies);
                _logger.Log($"AddPolicy: Policy added successfully for shopID {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("AddPolicy: Error occurred: " + e.Message);
                return new Response(e.Message);
            }
        }

        //public Response GetMember(string sessionID)
        //{
        //    throw new NotImplementedException();
        //}

        public Response<SProduct> AddProduct(string sessionID, int shopId, string productName,int sellType, string description, double price, int quantity, string category, List<string> keyWords)
        {
            try
            {
                Product addedProduct = _marketManager.AddProduct(sessionID, shopId, productName,sellType, description, price, quantity, category, keyWords);
                SProduct newProduct = new SProduct(addedProduct);
                _logger.Log($"AddProduct: Product added successfully for shopID {shopId} with name {productName} and quantity {quantity}");
                return Response<SProduct>.FromValue(newProduct);
            }
            catch (Exception e)
            {
                _logger.Error($"AddProduct: Error occurred: {e.Message}");
                return Response<SProduct>.FromError(e.Message);
            }
        }

        public Response AddReview(string sessionID, int shopId, string userName, int productID, double rate, string comment)
        {
            try
            {
                _marketManager.AddReview(sessionID, shopId, userName, productID, rate, comment);
                _logger.Log($"AddReview: Review added successfully for shopID {shopId}, productID {productID} and userName {userName}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"AddReview: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response AddToCart(string sessionID, int shopId, int productID, int quantity)
        {
            try
            {
                _marketManager.AddToCart(sessionID, shopId, productID, quantity);
                _logger.Log($"AddToCart: Product with ID {productID} added to cart successfully for shopID {shopId} and quantity {quantity}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"AddToCart: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response RemoveAppoint(string sessionID, string appointeeUserName, int shopID)
        {
            try
            {
                _marketManager.RemoveAppoint(sessionID, appointeeUserName, shopID);
                _logger.Log($"Remove appoint: User {appointeeUserName} was unAppointed from his role in the shop {shopID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Remove appoint: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response Appoint(string sessionID, string appointeeUserName, int shopId, int role, int permission)
        {
            try
            {
                _marketManager.Appoint(sessionID, appointeeUserName, shopId, role, permission);
                _logger.Log($"Appoint: User {appointeeUserName} was appointed to shop {shopId} with role {role} and permission {permission}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Appoint: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response ChangePermission(string sessionID, string appointeeUserName, int shopId, int permission)
        {
            try
            {
                _marketManager.ChangePermissions(sessionID, appointeeUserName, shopId, permission);
                _logger.Log($"ChangePermission: User {appointeeUserName} permission was changed to {permission} in shop {shopId}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"ChangePermission: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response CloseShop(string sessionID, int shopID)
        {
            try
            {
                _marketManager.CloseShop(sessionID, shopID);
                _logger.Log($"CloseShop: Shop {shopID} was closed by session {sessionID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"CloseShop: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response<SShop> CreateShop(string sessionID, string shopName)
        {
            try
            {
                Shop addedShop = _marketManager.CreateShop(sessionID, shopName);
                SShop newShop = new SShop(addedShop);
                _logger.Log($"CreateShop: Shop {shopName} was created by session {sessionID}");
                return Response<SShop>.FromValue(newShop);
            }
            catch (Exception e)
            {
                _logger.Error($"CreateShop: Error occurred: {e.Message}");
                return Response<SShop>.FromError(e.Message);
            }
        }

        public Response<List<SShop>> GetMarketInfo(string sessionID)
        {
            try
            {
                List<SShop> serviceShops = new List<SShop>();
                List<Shop> bussinessShops = _marketManager.GetMarketInfo(sessionID).ToList();
                _logger.Log($"GetMarketInfo: Market info was retrieved by session {sessionID}");
                foreach (Shop s in bussinessShops)
                    serviceShops.Add(new SShop(s));
                return Response<List<SShop>>.FromValue(serviceShops);
            }
            catch (Exception e)
            {
                _logger.Error($"GetMarketInfo: Error occurred: {e.Message}");
                return Response<List<SShop>>.FromError(e.Message);
            }
        }

        public Response<SShop> GetShopInfo(string sessionID, int shopID)
        {
            try
            {
                SShop shop = new SShop(_marketManager.GetShopInfo(sessionID, shopID));
                _logger.Log($"GetShopInfo: Shop info retrieved successfully for shopID {shopID}");
                return Response<SShop>.FromValue(shop);
            }
            catch (Exception e)
            {
                _logger.Error($"GetShopInfo: Error occurred: {e.Message}");
                return Response<SShop>.FromError(e.Message);
            }
        }

        public Response<SShoppingCart> GetShoppingCartInfo(string sessionID)
        {
            try
            {
                SShoppingCart shoppingCart = new SShoppingCart(_marketManager.GetShoppingCartInfo(sessionID));
                _logger.Log($"GetShoppingCartInfo: Shopping cart info retrieved successfully for sessionID {sessionID}");
                return Response<SShoppingCart>.FromValue(shoppingCart);
            }
            catch (Exception e)
            {
                _logger.Error($"GetShoppingCartInfo: Error occurred: {e.Message}");
                return Response<SShoppingCart>.FromError(e.Message);
            }
        }

        public Response<List<SAppointment>> GetShopPositions(string sessionID, int shopID)
        {
            try
            {
                List<Appointment> bussinessApp = _marketManager.GetShopPositions(sessionID, shopID).ToList();
                List<SAppointment> serviceApp = new List<SAppointment>();
                foreach (Appointment app in bussinessApp)
                {
                    serviceApp.Add(new SAppointment(app));
                }
                _logger.Log($"getShopPositions: Shop positions retrieved successfully for shopID {shopID}");
                return Response<List<SAppointment>>.FromValue(serviceApp);
            }
            catch (Exception e)
            {
                _logger.Error($"getShopPositions: Error occurred: {e.Message}");
                return Response<List<SAppointment>>.FromError(e.Message);
            }
        }

        public Response Login(string sessionID, string username, string password)
        {
            try
            {
                _marketManager.Login(sessionID, username, password);
                _logger.Log($"Login: User {username} logged in successfully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Login: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response Logout(string sessionID)
        {
            try
            {
                _marketManager.Logout(sessionID);
                _logger.Log($"Logout: Session {sessionID} logged out successfully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Logout: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response OpenShop(string sessionID, int shopID)
        {
            try
            {
                _marketManager.OpenShop(sessionID, shopID);
                _logger.Log($"OpenShop: Shop {shopID} opened successfully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"OpenShop: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response PurchaseBasket(string sessionID, int shopId, string cardNumber, string month, string year, string holder, string ccv, string id,
            string name, string address, string city, string country, string zip)
        {
            try
            {
                PaymentDetails paymentDetails = new PaymentDetails(cardNumber, month, year, holder, ccv, id);
                DeliveryDetails deliveryDetails = new DeliveryDetails(name, address, city, country, zip);
                _marketManager.PurchaseBasket(sessionID, shopId, paymentDetails, deliveryDetails);
                _logger.Log($"PurchaseBasket: Basket purchased successfully for session {sessionID} and shopID {shopId}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"PurchaseBasket: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response PurchaseShoppingCart(string sessionID, string cardNumber, string month, string year, string holder, string ccv, string id,
            string name, string address, string city, string country, string zip)
        {
            try
            {
                PaymentDetails paymentDetails = new PaymentDetails(cardNumber, month, year, holder, ccv, id);
                DeliveryDetails deliveryDetails = new DeliveryDetails(name, address, city, country, zip);
                _marketManager.PurchaseShoppingCart(sessionID, paymentDetails, deliveryDetails);
                _logger.Log($"PurchaseShoppingCart: ShoppingCart purchased successfully for session {sessionID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"PurchaseShoppingCart: Error occurred: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response Register(string sessionID, string username, string password)
        {
            try
            {
                _marketManager.Register(sessionID, username, password);
                _logger.Log($"User registered successfully: {username}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error registering user: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response RemovePolicy(string sessionID, int shopID, int policyID, string type)
        {
            try
            {
                _marketManager.RemovePolicy(sessionID, shopID, policyID, type);
                _logger.Log($"policy removed successfully. Shop ID: {shopID}, Policy ID: {policyID}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error removing policy. Shop ID: {shopID}, Policy ID: {policyID}, Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response RemoveProduct(string sessionID, int shopId, int productID)
        {
            try
            {
                _marketManager.RemoveProduct(sessionID, shopId, productID);
                _logger.Log("Product removed successfully. Shop ID: " + shopId + ", Product ID: " + productID);
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("Error removing product. Shop ID: " + shopId + ", Product ID: " + productID + ", Error message: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response RemoveFromCart(string sessionID, int shopID, int productID)
        {
            try
            {
                _marketManager.RemoveFromCart(sessionID, shopID, productID);
                _logger.Log("Product removed from shopping cart successfully. Shop ID: " + shopID + ", Product ID: " + productID);
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("Error removing product from shopping cart. Shop ID: " + shopID + ", Product ID: " + productID + ", Error message: " + e.Message);
                return new Response(e.Message);
            }
        }

        public Response<List<SProduct>> Search(string sessionID, string word, List<int> searchType, List<int> filterType, int lowPrice, int highPrice, int lowRate, int highRate, string category)
        {
            try
            {
                List<SFilterSearch> filterSearchTypes = GetFilterTypes(filterType, lowPrice, highPrice, lowRate, highRate, category);
                List<FilterSearchType> filters = ParseSFilterSearch(filterSearchTypes);
                HashSet<Product> bProducts = _marketManager.Search(sessionID, word, searchType, filters);
                List<SProduct> products = new List<SProduct>();
                foreach (Product product in bProducts)
                {
                    products.Add(new SProduct(product));
                }
                _logger.Log($"Search query '{word}' returned {products.Count} products");
                return Response<List<SProduct>>.FromValue(products);
            }
            catch (Exception e)
            {
                _logger.Error($"Search query '{word}' failed: {e.Message}");
                return Response<List<SProduct>>.FromError(e.Message);
            }
        }

        public Response SendMessage(string sessionID, int shopID, string comment)
        {
            try
            {
                _marketManager.SendMessage(sessionID, shopID, comment);
                _logger.Log($"Message sent from session {sessionID} to shop {shopID}: {comment}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to send message from session {sessionID} to shop {shopID}: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response SendReport(string sessionID, int shopId, string comment)
        {
            try
            {
                _marketManager.SendReport(sessionID, shopId, comment);
                _logger.Log($"Report sent from session {sessionID} to shop {shopId}: {comment}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to send report from session {sessionID} to shop {shopId}: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response<List<SPurchase>> ShowMemberPurchaseHistory(string sessionID)
        {
            try
            {
                List<Purchase> bPurchase = _marketManager.ShowPurchaseHistory(sessionID);
                List<SPurchase> sPurchases = new List<SPurchase>();
                foreach (Purchase purchase in bPurchase)
                {
                    sPurchases.Add(new SPurchase(purchase));
                }
                _logger.Log($"Session {sessionID} requested purchase history");
                return Response<List<SPurchase>>.FromValue(sPurchases);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to retrieve purchase history for session {sessionID}: {e.Message}");
                return Response<List<SPurchase>>.FromError(e.Message);
            }
        }

        public Response<List<SPurchase>> ShowShopPurchaseHistory(string sessionID, int shopID)
        {
            try
            {
                List<Purchase> bPurchase = _marketManager.ShowShopHistory(sessionID, shopID);
                List<SPurchase> sPurchases = new List<SPurchase>();
                foreach (Purchase purchase in bPurchase)
                {
                    sPurchases.Add(new SPurchase(purchase));
                }
                _logger.Log($"Session {sessionID} requested shop {shopID} purchase history");
                return Response<List<SPurchase>>.FromValue(sPurchases);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to retrieve purchase history for session {sessionID} and shop {shopID}: {e.Message}");
                return Response<List<SPurchase>>.FromError(e.Message);
            }
        }

        public Response UpdateProductName(string sessionID, int shopId, int productID, string name)
        {
            try
            {
                _marketManager.UpdateProductName(sessionID, shopId, productID, name);
                _logger.Log($"Product name updated for product ID: {productID}, new name: {name}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to update product name for product ID: {productID}, error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response UpdateProductPrice(string sessionID, int shopId, int productID, double price)
        {
            try
            {
                _marketManager.UpdateProductPrice(sessionID, shopId, productID, price);
                _logger.Log($"Product price updated for product ID: {productID}, new price: {price}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to update product price for product ID: {productID}, error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response UpdateProductQuantity(string sessionID, int shopId, int productID, int quantity)
        {
            try
            {
                _marketManager.UpdateProductQuantity(sessionID, shopId, productID, quantity);
                _logger.Log($"Product quantity updated for product ID: {productID}, new quantity: {quantity}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to update product quantity for product ID: {productID}, error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response UpdateBasketItemQuantity(string sessionID, int shopId, int productID, int quantity)
        {
            try
            {
                _marketManager.UpdateBasketItemQuantity(sessionID, shopId, productID, quantity);
                _logger.Log($"BasketItem quantity updated for product ID: {productID}, new quantity: {quantity}");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to update basketItem quantity for product ID: {productID}, error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        private List<SFilterSearch> GetFilterTypes(List<int> codes, int lowPrice, int highPrice, int lowRate, int highRate, string category)
        {
            List<SFilterSearch> filters = new List<SFilterSearch>();
            foreach (int code in codes)
            {
                if(code == 0)
                    filters.Add(new SFilterSearch(code, lowPrice, highPrice, category));
                else
                    filters.Add(new SFilterSearch(code, lowRate, highRate, category));
            }
            return filters;
        }

        private List<FilterSearchType> ParseSFilterSearch(List<SFilterSearch> sFilterSearches)
        {
            List<FilterSearchType> filters = new List<FilterSearchType>();
            foreach (SFilterSearch sFilter in sFilterSearches)
            {
                if (sFilter.Code == 0)
                {
                    filters.Add(new PriceRangeFilter(sFilter.Low, sFilter.High));
                }
                else if (sFilter.Code == 1)
                {
                    filters.Add(new ProductRatingFilter(sFilter.Low, sFilter.High));
                }
                else if (sFilter.Code == 2)
                {
                    filters.Add(new CategoryFilter((sFilter.Category)));
                }
            }
            return filters;
        }


        public Response<SUser> GetUser(string sessionID)
        {
            try
            {
                DomainLayer.User user = _marketManager.GetUser(sessionID);
                if (user is Guest)
                {
                    _logger.Log($"Guest user with session ID {sessionID} accessed getUser method");
                    return Response<SUser>.FromValue(new SUser(user));
                }
                else
                {
                    _logger.Log($"Member user with session ID {sessionID} accessed getUser method");
                    return Response<SUser>.FromValue(new SMember(user as Member));
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred in getUser method for session ID {sessionID}: {e.Message}");
                return Response<SUser>.FromError(e.Message);
            }
        }

        public Response<SShop> GetShopByName(string sessionID, string name)
        {
            try
            {
                Shop shop = _marketManager.GetShopByName(name);
                _logger.Log($"User with session ID {sessionID} searched for shop with name '{name}'");
                return Response<SShop>.FromValue(new SShop(shop));
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred in GetShopByName method for session ID {sessionID} and shop name '{name}': {e.Message}");
                return Response<SShop>.FromError(e.Message);
            }
        }

        public Response<List<SShop>> GetUserShops(string sessionID)
        {
            try
            {
                List<Shop> shops = _marketManager.GetUserShops(sessionID);
                List<SShop> sShops = new List<SShop>();
                foreach (Shop shop in shops)
                {
                    sShops.Add(new SShop(shop));
                }
                _logger.Log($"User with session ID {sessionID} accessed GetUserShops method");
                return Response<List<SShop>>.FromValue(sShops);
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred in GetUserShops method for session ID {sessionID}: {e.Message}");
                return Response<List<SShop>>.FromError(e.Message);
            }
        }
        public Response<List<string>> GetAllMembers(string sessionID)
        {
            try
            {
                List<Member> members = _marketManager.GetAllMembers(sessionID);
                List<string> sMembers = new List<string>();
                foreach (Member member in members)
                {
                    sMembers.Add(member.UserName);
                }
                _logger.Log($"User with session ID {sessionID} accessed GetAllMembers method");
                return Response<List<string>>.FromValue(sMembers);
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred in GetAllMembers method for session ID {sessionID}: {e.Message}");
                return Response<List<string>>.FromError(e.Message);
            }
        }
        public Response<List<string>> GetActiveMembers(string sessionID)
        {
            try
            {
                List<Member> members = _marketManager.GetActiveMembers(sessionID);
                List<string> sMembers = new List<string>();
                foreach (Member member in members)
                {
                    sMembers.Add(member.UserName);
                }
                _logger.Log($"User with session ID {sessionID} accessed GetActiveMembers method");
                return Response<List<string>>.FromValue(sMembers);
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred in GetActiveMembers method for session ID {sessionID}: {e.Message}");
                return Response<List<string>>.FromError(e.Message);
            }
        }

        public Response CancelMembership(string sessionID, string memberName)
        {
            try
            {
                _marketManager.CancelMembership(sessionID, memberName);
                _logger.Log("Membership canceled successfully. Member name: " + memberName + "\n");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error("Error in cancel membership. Member name: " + memberName + ", Error message: " + e.Message);
                return new Response(e.Message);
            }
        }
        public Response NotificationOn(string sessionID)
        {
            try
            {
                _marketManager.NotificationOn(sessionID);
                _logger.Log($"session id {sessionID} turn on notification");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in turn on notifications. SessionId: {sessionID} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response NotificationOff(string sessionID)
        {
            try
            {
                _marketManager.NotificationOff(sessionID);
                _logger.Log($"session id {sessionID} turn off notification");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in turn off notifications. SessionId: {sessionID} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response<bool> IsSystemAdmin(string sessionID)
        {
            return Response<bool>.FromValue(_marketManager.IsSystemAdmin(sessionID));
        }
        public Response<bool> IsLoggedIn(string userName)
        {
            throw new NotImplementedException();
        }

        public Response AddSystemAdmin(string adminUserName)
        {
            try
            {
                _marketManager.AppointSystemAdmin(adminUserName);
                _logger.Log($"System admin with the name: {adminUserName}, has been added succefully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in add system admin. Member's user name: {adminUserName} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response BidOnProduct(string sessionId, int shopID, int productId, int quantity, double suggestedPriceForOne)
        {
            try
            {
                _marketManager.BidOnProduct(sessionId, shopID, productId, quantity, suggestedPriceForOne);
                _logger.Log($"Bid On Product: sessionId {sessionId} bids on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in bid on product. Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response ApproveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            try
            {
                _marketManager.ApproveBid(sessionId, shopID, bidUsername, productId);
                _logger.Log($"Approved Bid: sessionId {sessionId} approved bid on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Approve Bid: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response OfferCounterBid(string sessionId,int shopID, string bidUsername, int productId, double counterPrice)
        {
            try
            {
                _marketManager.OfferCounterBid(sessionId, shopID, bidUsername, productId,counterPrice);
                _logger.Log($"Offer Counter Bid: sessionId {sessionId} offer counter price bid on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Offer Counter Price: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response ApproveCounterBid(string sessionId, int shopId, int productId)
        {
            try
            {
                _marketManager.ApproveCounterBid(sessionId, shopId, productId);
                _logger.Log($"Approved Counter Bid: sessionId {sessionId} approved counter bid on product {productId} in shop {shopId} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Approve Counter Bid: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response DissapproveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            try
            {
                _marketManager.DissapproveBid(sessionId, shopID, bidUsername, productId);
                _logger.Log($"Dissapprove Bid: sessionId {sessionId} dissapprove bid on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Dissapprove Bid: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }

        public Response RemoveBid(string sessionId, int shopID, string bidUsername, int productId)
        {
            try
            {
                _marketManager.RemoveBid(sessionId, shopID, bidUsername, productId);
                _logger.Log($"Remove Bid: sessionId {sessionId} Remove bid on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Remove Bid: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response DeclineCounterBid(string sessionId, int shopID, int productId)
        {
            try
            {
                _marketManager.DeclineCounterBid(sessionId, shopID, productId);
                _logger.Log($"Decline Counter Bid: sessionId {sessionId} declined counter bid on product {productId} in shop {shopID} successfullyy");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Decline Counter Bid: Member's sessionId: {sessionId} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response ApproveAppointment(string session, int shopId, string appointeeUserName)
        {
            try
            {
                _marketManager.AddApproval(session, shopId, appointeeUserName);
                _logger.Log($"Approval for the appointee: {appointeeUserName}, has been added succefully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in add approval. Appointee's user name: {appointeeUserName} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public Response DeclineAppointment(string session, int shopId, string appointeeUserName)
        {
            try
            {
                _marketManager.AddDecline(session, shopId, appointeeUserName);
                _logger.Log($"Decline for the appointee: {appointeeUserName}, has been added succefully");
                return new Response();
            }
            catch (Exception e)
            {
                _logger.Error($"Error in add decline. Appointee's user name: {appointeeUserName} Error message: {e.Message}");
                return new Response(e.Message);
            }
        }
        public void WriteToLogger(string msg, bool isError)
        {
            if (isError)
                _logger.Error(msg);
            else
                _logger.Log(msg);
        }

        public Response<int> GetMessagesNumberRequest(string sessionId)
        {
            try
            {
                int notifications = _marketManager.GetMessagesNumberRequest(sessionId);
                return Response<int>.FromValue(notifications);
            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }
        }

        public Response<int> GetShoppingCartAmount(string sessionId)
        {
            try
            {
                int productAmounts = _marketManager.GetShoppingCartAmount(sessionId);
                return Response<int>.FromValue(productAmounts);
            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }
        }
    }
}
