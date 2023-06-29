using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebSocketSharp.Server;
using Microsoft.AspNetCore.Session;
using System.Xml.Schema;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using Market.DomainLayer;

namespace ServerMarket.API
{
    public class ServerResponse<T>
    {
        public T value { get; set; }
        public string errorMessage { get; set; }
        public static ServerResponse<T> sendOkResponse(T val)
        {
            var response = new ServerResponse<T>
            {
                value = val,
            };
            return response;
        }
        public static ServerResponse<T> sendBadResponse(string msg)
        {
            var response = new ServerResponse<T>
            {
                errorMessage = msg,
            };
            return response;
        }
        // Add other properties as needed for more complex objects
    }

    [ApiController]
    [Route("api/market")]
    public class MarketController : ControllerBase
    {
        private IMarket service;
        private WebSocketServer notificationServer;
        private WebSocketServer logserver;
        private static IDictionary<string, IList<string>> buyerUnsentMessages = new Dictionary<string, IList<string>>();
        private static IDictionary<string, string> buyerIdToRelativeNotificationPath = new Dictionary<string, string>();
        public MarketController(WebSocketServer notificationServer, WebSocketServer lgs)
        {
            this.service = MarketService.GetInstance();
            this.notificationServer = notificationServer;
            NotificationManager.GetInstance().setNotificationServer(notificationServer);
            this.logserver = lgs;
        }
        private class NotificationsService : WebSocketBehavior
        {

        }
        public class logsService : WebSocketBehavior
        {

        }

        [HttpPost]
        [Route("add-product")]
        public async Task<ObjectResult> AddProduct([FromBody] AddProductRequest request)
        {
            Response<SProduct> response = await Task.Run(() => service.AddProduct(request.SessionId, request.ShopId, request.ProductName, request.SellType, request.Description, request.Price, request.Quantity, request.Category, request.KeyWords));
            if (response.ErrorOccured)
            {
                var addProductResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(addProductResponse);
            }
            else
            {
                var addProductResponse = new ServerResponse<SProduct>
                {
                    value = response.Value,
                };
                return Ok(addProductResponse);
            }
        }

        [HttpPost]
        [Route("add-review")]
        public async Task<ObjectResult> AddReview([FromBody] AddReviewRequest request)
        {
            Response response = await Task.Run(() => service.AddReview(request.SessionId, request.ShopID, request.UserName, request.ProductID, request.Rate, request.Comment));
            if (response.ErrorOccured)
            {
                var addReviewResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(addReviewResponse);
            }
            else
            {
                var addReviewResponse = new ServerResponse<string>
                {
                    value = "Add review success",
                };
                return Ok(addReviewResponse);
            }
        }

        [HttpPost]
        [Route("add-to-cart")]
        public async Task<ObjectResult> AddToCart([FromBody] AddToCartRequest request)
        {
            Response response = await Task.Run(() => service.AddToCart(request.SessionId, request.ShopID, request.ProductID, request.Quantity));
            if (response.ErrorOccured)
            {
                var addToCartResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(addToCartResponse);
            }
            else
            {
                var addToCartResponse = new ServerResponse<string>
                {
                    value = "Add to cart success",
                };
                return Ok(addToCartResponse);
            }
        }

        [HttpPost]
        [Route("appoint")]
        public async Task<ObjectResult> Appoint([FromBody] AppointRequest request)
        {
            Response response = await Task.Run(() => service.Appoint(request.SessionId, request.AppointeeUserName, request.ShopID, request.Role, request.Permission));
            if (response.ErrorOccured)
            {
                var appointResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(appointResponse);
            }
            else
            {
                var appointResponse = new ServerResponse<string>
                {
                    value = "Appoint success",
                };
                return Ok(appointResponse);
            }
        }
        [HttpPost]
        [Route("remove-appoint")]
        public async Task<ObjectResult> RemoveAppoint([FromBody] AppointRequest request)
        {
            Response response = await Task.Run(() => service.RemoveAppoint(request.SessionId, request.AppointeeUserName, request.ShopID));
            if (response.ErrorOccured)
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(removeAppointResponse);
            }
            else
            {
                var removeAppointResponse = new ServerResponse<string>
                {
                    value = "remove Appoint success",
                };
                return Ok(removeAppointResponse);
            }
        }

        [HttpPost]
        [Route("change-permission")]
        public async Task<ObjectResult> ChangePermission([FromBody] ChangePermisionRequest request)
        {
            Response response = await Task.Run(() => service.ChangePermission(request.SessionId, request.AppointeeUserName, request.ShopId, request.Permission));
            if (response.ErrorOccured)
            {
                var changePermissionResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(changePermissionResponse);
            }
            else
            {
                var changePermissionResponse = new ServerResponse<string>
                {
                    value = "change-permission-success",
                };
                return Ok(changePermissionResponse);
            }
        }

        [HttpPost]
        [Route("close-shop")]
        public async Task<ObjectResult> CloseShop([FromBody] CloseShopRequest request)
        {
            Response response = await Task.Run(() => service.CloseShop(request.SessionId, request.ShopID));
            if (response.ErrorOccured)
            {
                var closeShopResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(closeShopResponse);
            }
            else
            {
                var closeShopResponse = new ServerResponse<string>
                {
                    value = "close-shop-success",
                };
                return Ok(closeShopResponse);
            }
        }
        [HttpPost]
        [Route("create-shop")]
        public async Task<ObjectResult> CreateShop([FromBody] CreateShopRequest request)
        {
            Response<SShop> response = await Task.Run(() => service.CreateShop(request.SessionId, request.ShopName));
            if (response.ErrorOccured)
            {
                var createShopResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(createShopResponse);
            }
            else
            {
                var createShopResponse = new ServerResponse<SShop>
                {
                    value = response.Value,
                };
                return Ok(createShopResponse);
            }
        }
        /*
        [HttpPost]
        [Route("login")]
        public async Task<ObjectResult> Login([FromBody] LoginRequest request)
        {
            Response response = await Task.Run(() => service.Login(request.SessionId, request.Username, request.Password));
            if (response.ErrorOccured)
            {
                var LoginrResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(LoginrResponse);
            }
            else
            {
                var LoginrResponse = new ServerResponse<string>
                {
                    value = HttpContext.Session.Id,
                };
                return Ok(LoginrResponse);
            }

        }

        [HttpPost]
        [Route("logout")]
        public async Task<ObjectResult> Logout([FromBody] LogoutRequest request)
        {
            Response response = await Task.Run(() => service.Logout(request.SessionId));
            if (response.ErrorOccured)
            {
                var logoutResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(logoutResponse);
            }
            else
            {
                var logoutResponse = new ServerResponse<string>
                {
                    value = HttpContext.Session.Id,
                };
                return Ok(logoutResponse);
            }

        }
        */
        [HttpPost]
        [Route("login")]
        public async Task<ObjectResult> Login([FromBody] LoginRequest request)
        {
            string relativeServicePath = "/" + request.Username + "-notifications";
            try
            {
                if (notificationServer.WebSocketServices[relativeServicePath] == null)
                {
                    notificationServer.AddWebSocketService<NotificationsService>(relativeServicePath);

                }
            }
            catch (Exception ex)
            {
                var loginResponse = new ServerResponse<string>
                {
                    errorMessage = ex.Message,
                };
                return BadRequest(loginResponse);
            } // in case the client tries to login again

            string session = HttpContext.Session.Id;
            Response response = await Task.Run(() => service.Login(request.SessionId, request.Username, request.Password));
            if (response.ErrorOccured)
            {
                notificationServer.RemoveWebSocketService(relativeServicePath);
                var loginResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(loginResponse);
            }
            else
            {
                buyerIdToRelativeNotificationPath.Add(request.SessionId, relativeServicePath);
                var createShopResponse = new ServerResponse<string>
                {
                    value = session,
                };
                return Ok(createShopResponse);
            }
        }
        private void AddUnsentMessage(string username, IList<string> messages)
        {
            buyerUnsentMessages[username] = messages;
        }


        [HttpPost]
        [Route("logout")]
        public async Task<ObjectResult> Logout([FromBody] LogoutRequest request)
        {
            Response response = await Task.Run(() => service.Logout(request.SessionId));
            ServerResponse<string> logoutResponse;
            if (response.ErrorOccured)
            {
                logoutResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(logoutResponse);
            }
            // Clearing the connection
            if (buyerIdToRelativeNotificationPath.ContainsKey(request.SessionId))
            {
                notificationServer.RemoveWebSocketService(buyerIdToRelativeNotificationPath[request.SessionId]);
                buyerIdToRelativeNotificationPath.Remove(request.SessionId);//TODO
            }

            logoutResponse = new ServerResponse<string>
            {
                value = "create-shop-success",
            };
            return Ok(logoutResponse);
        }

        [HttpPost]
        [Route("enter-as-guest")]
        public async Task<ObjectResult> EnterAsGuest([FromBody] EnterAsGuestRequest request)
        {
            string session = HttpContext.Session.Id;
            Response response = await Task.Run(() => service.EnterAsGuest(session));
            if (response.ErrorOccured)
            {
                var enterAsGuestResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(enterAsGuestResponse);
            }
            else
            {
                var enterAsGuestResponse = new ServerResponse<string>
                {
                    value = session,
                };
                return Ok(enterAsGuestResponse);
            }
        }


        [HttpPost]
        [Route("open-shop")]
        public async Task<ObjectResult> OpenShop([FromBody] OpenShopRequest request)
        {
            Response response = await Task.Run(() => service.OpenShop(request.SessionId, request.ShopID));
            if (response.ErrorOccured)
            {
                var openShopResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(openShopResponse);
            }
            else
            {
                var openShopResponse = new ServerResponse<string>
                {
                    value = "open-shop-success",
                };
                return Ok(openShopResponse);
            }
        }

        [HttpPost]
        [Route("purchase-basket")]
        public async Task<ObjectResult> PurchaseBasket([FromBody] PurchaseBasketRequest request)
        {
            Response response = await Task.Run(() => service.PurchaseBasket(request.SessionId, request.ShopId, request.cardNumber,
                request.month, request.year, request.holder, request.ccv
                , request.id, request.name, request.address, request.city, request.country, request.zip));
            if (response.ErrorOccured)
            {
                var purchaseBasketResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(purchaseBasketResponse);
            }
            else
            {
                var purchaseBasketResponse = new ServerResponse<string>
                {
                    value = "purchase-basket-success",
                };
                return Ok(purchaseBasketResponse);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ObjectResult> Register([FromBody] RegisterRequest request)
        {
            Response response = await Task.Run(() => service.Register(HttpContext.Session.Id, request.Username, request.Password));
            if (response.ErrorOccured)
            {
                var registerResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(registerResponse);
            }
            else
            {
                var registerResponse = new ServerResponse<string>
                {
                    value = HttpContext.Session.Id,
                };
                return Ok(registerResponse);
            }

        }

        [HttpPost]
        [Route("remove-policy")]
        public async Task<ObjectResult> RemovePolicy([FromBody] RemovePolicyRequest request)
        {
            Response response = await Task.Run(() => service.RemovePolicy(request.SessionId, request.ShopID, request.PolicyID, request.Type));
            if (response.ErrorOccured)
            {
                var removePolicyResponse = new ServerResponse<string>
                {
                    errorMessage = response.ErrorMessage,
                };
                return BadRequest(removePolicyResponse);
            }
            else
            {
                var removePolicyResponse = new ServerResponse<string>
                {
                    value = "remove-policy-success",
                };
                return Ok(removePolicyResponse);
            }
        }
        [HttpPost]
        [Route("remove-product")]
        public async Task<ObjectResult> RemoveProduct([FromBody] RemoveProductRequest request)
        {
            Response response = await Task.Run(() => service.RemoveProduct(request.SessionId, request.ShopID, request.ProductID));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("remove-product-success"));
            }
        }

        [HttpPost]
        [Route("remove-from-cart")]
        public async Task<ObjectResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            Response response = await Task.Run(() => service.RemoveFromCart(request.SessionId, request.ShopID, request.ProductID));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("remove-from-cart-success"));
            }
        }


        [HttpPost]
        [Route("send-message")]
        public async Task<ObjectResult> SendMessage([FromBody] SendMessageRequest request)
        {
            Response response = await Task.Run(() => service.SendMessage(request.SessionId, request.ShopID, request.Comment));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("send-message-success"));
            }
        }
        [HttpPost]
        [Route("send-report")]
        public async Task<ObjectResult> SendReport([FromBody] SendReportRequest request)
        {
            Response response = await Task.Run(() => service.SendReport(request.SessionId, request.ShopId, request.Comment));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("send-report-success"));
            }
        }

        [HttpPost]
        [Route("update-product-name")]
        public async Task<ObjectResult> UpdateProductName([FromBody] UpdateProductNameRequest request)
        {
            Response response = await Task.Run(() => service.UpdateProductName(request.SessionId, request.ShopId, request.ProductID, request.Name));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-product-name-success"));
            }
        }
        [HttpPost]
        [Route("update-product-price")]
        public async Task<ObjectResult> UpdateProductPrice([FromBody] UpdateProductPriceRequest request)
        {
            Response response = await Task.Run(() => service.UpdateProductPrice(request.SessionId, request.ShopId, request.ProductID, request.Price));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-product-price-success"));

            }
        }
        [HttpPost]
        [Route("update-product-quantity")]
        public async Task<ObjectResult> UpdateProductQuantity([FromBody] UpdateProductQuantityRequest request)
        {
            Response response = await Task.Run(() => service.UpdateProductQuantity(request.SessionId, request.ShopId, request.ProductID, request.Quantity));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-product-quantity-success"));
            }
        }


        [HttpPost]
        [Route("cancel-membership")]
        public async Task<ObjectResult> CancelMembership([FromBody] CancelMembershipRequest request)
        {
            Response response = await Task.Run(() => service.CancelMembership(request.SessionId, request.MemberUserName));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("cancel-membership-success"));
            }
        }

        [HttpPost]
        [Route("add-composit-policy")]
        public async Task<ObjectResult> AddCompositePolicy([FromBody] AddCompositePolicyRequest request)
        {
            Response response = await Task.Run(() => service.AddCompositePolicy(request.SessionId, request.ShopID, request.ExpirationDate, request.Subject, request.Op, request.Policies));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-composit-policy-success"));
            }
        }

        [HttpPost]
        [Route("add-discount-policy")]
        public async Task<ObjectResult> AddDiscountPolicy([FromBody] AddDiscountPolicyRequest request)
        {
            Response response = await Task.Run(() => service.AddDiscountPolicy(request.SessionId, request.ShopID, request.Expirationdate, request.Subject, request.RuleId, request.Percentage));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-discount-policy-success"));
            }
        }

        [HttpPost]
        [Route("add-purchase-policy")]
        public async Task<ObjectResult> AddPurchasePolicy([FromBody] AddPurchasePolicyRequest request)
        {
            Response response = await Task.Run(() => service.AddPurchasePolicy(request.SessionId, request.ShopID, request.Expirationdate, request.Subject, request.RuleId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-purchase-policy-success"));
            }
        }
        [HttpPost]
        [Route("add-composit-rule")]
        public async Task<ObjectResult> AddCompositeRule([FromBody] AddCompositeRuleRequest request)
        {
            Response response = await Task.Run(() => service.AddCompositeRule(request.SessionId, request.ShopID, request.Op, request.Rules));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-composit-rule-success"));
            }
        }

        [HttpPost]
        [Route("add-quantity-rule")]
        public async Task<ObjectResult> AddQuantityRule([FromBody] AddQuantityRuleRequest request)
        {
            Response response = await Task.Run(() => service.AddQuantityRule(request.SessionId, request.ShopID, request.Subject, request.MinQuantity, request.MaxQuantity));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-quantity-rule-success"));
            }
        }

        [HttpPost]
        [Route("add-simple-rule")]
        public async Task<ObjectResult> AddSimpleRule([FromBody] AddSimpleRuleRequest request)
        {
            Response response = await Task.Run(() => service.AddSimpleRule(request.SessionId, request.ShopID, request.Subject));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-simple-rule-success"));
            }
        }

        [HttpPost]
        [Route("add-total-price-rule")]
        public async Task<ObjectResult> AddTotalPriceRule([FromBody] AddTotalPriceRuleRequest request)
        {
            Response response = await Task.Run(() => service.AddTotalPriceRule(request.SessionId, request.ShopID, request.Subject, request.TargetPrice));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("add-total-price-rule-success"));
            }
        }

        [HttpPost]
        [Route("update-composite-rule")]
        public async Task<ObjectResult> UpdateCompositeRules([FromBody] UpdateCompositeRulesRequest request)
        {
            Response response = await Task.Run(() => service.UpdateCompositeRules(request.SessionId, request.ShopId, request.RuleId, request.Rules));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-composite-rule-success"));
            }
        }
        [HttpPost]
        [Route("update-rule-quantity")]
        public async Task<ObjectResult> UpdateRuleQuantity([FromBody] UpdateRuleQuantityRequest request)
        {
            Response response = await Task.Run(() => service.UpdateRuleQuantity(request.SessionId, request.ShopId, request.RuleId, request.MinQuantity, request.MaxQuantity));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-rule-quantity-success"));
            }
        }
        [HttpPost]
        [Route("update-rule-subject")]
        public async Task<ObjectResult> UpdateRuleSubject([FromBody] UpdateRuleSubjectRequest request)
        {
            Response response = await Task.Run(() => service.UpdateRuleSubject(request.SessionId, request.ShopId, request.RuleId, request.Subject));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-rule-subject-success"));
            }
        }

        [HttpPost]
        [Route("update-composite-operator")]
        public async Task<ObjectResult> UpdateCompositeOperator([FromBody] UpdateCompositeOperatorRequest request)
        {
            Response response = await Task.Run(() => service.UpdateCompositeOperator(request.SessionId, request.ShopId, request.RuleId, request.Operator));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-composite-operator-success"));
            }
        }
        [HttpPost]
        [Route("purchase-shopping-cart")]
        public async Task<ObjectResult> PurchaseShoppingCart([FromBody] PurchaseShoppingCartRequest request)
        {
            Response response = await Task.Run(() => service.PurchaseShoppingCart(request.SessionId, request.cardNumber,
                request.month, request.year, request.holder, request.ccv, request.id,
                request.name, request.address, request.city, request.country, request.zip));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("purchase-shopping-cart-success"));
            }
        }

        [HttpPost]
        [Route("notification-on")]
        public async Task<ObjectResult> NotificationOn([FromBody] NotificationOnRequest request)
        {
            Response response = await Task.Run(() => service.NotificationOn(request.SessionId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("notification-on"));
            }
        }
        [HttpPost]
        [Route("notification-off")]
        public async Task<ObjectResult> NotificationOn([FromBody] NotificationOffRequest request)
        {
            Response response = await Task.Run(() => service.NotificationOff(request.SessionId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("notification-on-success"));
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////
        //-----------------------------Getters-------------------------------------------

        [HttpPost]
        [Route("get-market-info")]
        public ActionResult<Response<List<SShop>>> GetMarketInfo([FromBody] GetMarketInfoRequest request)
        {
            Response<List<SShop>> response = service.GetMarketInfo(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SShop>>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-shop-info")]
        public ActionResult<Response<SShop>> GetShopInfo([FromBody] GetShopInfoRequest request)
        {
            Response<SShop> response = service.GetShopInfo(request.SessionId, request.ShopID);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<SShop>.sendOkResponse(response.Value));
            }
        }


        [HttpPost]
        [Route("get-shopping-cart-info")]
        public ActionResult<Response<SShoppingCart>> GetShoppingCartInfo([FromBody] GetShoppingCartInfoRequest request)
        {
            Response<SShoppingCart> response = service.GetShoppingCartInfo(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<SShoppingCart>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-shopping-cart-amount")]
        public ActionResult<Response<int>> GetShoppingCartAmount([FromBody] GetShoppingCartInfoRequest request)
        {
            Response<int> response = service.GetShoppingCartAmount(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-active-members")]
        public ActionResult<Response<List<string>>> GetActiveMembers([FromBody] GetActiveMembersRequest request)
        {
            Response<List<string>> response = service.GetActiveMembers(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<string>>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-all-members")]
        public ActionResult<Response<List<string>>> GetAllMembers([FromBody] GetAllMembersRequest request)
        {
            Response<List<string>> response = service.GetAllMembers(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<string>>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-shop-positions")]
        public ActionResult<Response<SAppointment>> GetShopPositions([FromBody] GetShopPositionsRequest request)
        {
            Response<List<SAppointment>> response = service.GetShopPositions(request.SessionId, request.ShopID);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SAppointment>>.sendOkResponse(response.Value));
            }
        }


        [HttpPost]
        [Route("search")]
        public ActionResult<Response<List<SProduct>>> Search([FromBody] SearchRequest request)
        {
            Response<List<SProduct>> response = service.Search(request.SessionId, request.Word, request.SearchType, request.FilterType, request.LowPrice, request.HighPrice, request.LowRate, request.HighRate, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SProduct>>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("show-member-purchase-history")]
        public ActionResult<Response<List<SPurchase>>> ShowMemberPurchaseHistory([FromBody] ShowMemberPurchaseHistoryRequest request)
        {
            Response<List<SPurchase>> response = service.ShowMemberPurchaseHistory(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SPurchase>>.sendOkResponse(response.Value));
            }
        }


        [HttpPost]
        [Route("show-shop-purchase-history")]
        public ActionResult<Response<List<SPurchase>>> ShowShopPurchaseHistory([FromBody] ShowShopPurchaseHistoryRequest request)
        {
            Response<List<SPurchase>> response = service.ShowShopPurchaseHistory(request.SessionId, request.ShopID);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SPurchase>>.sendOkResponse(response.Value));
            }
        }


        [HttpPost]
        [Route("get-user")]
        public ActionResult<Response<SUser>> GetUser([FromBody] GetUserRequest request)
        {
            Response<SUser> response = service.GetUser(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<SUser>.sendOkResponse(response.Value));
            }
        }



        [HttpPost]
        [Route("get-shop-by-name")]
        public ActionResult<Response<SShop>> GetShopByName([FromBody] GetShopByNameRequest request)
        {
            Response<SShop> response = service.GetShopByName(request.SessionId, request.Name);

            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<SShop>.sendOkResponse(response.Value));
            }
        }


        [HttpPost]
        [Route("get-user-shops")]
        public ActionResult<Response<List<SShop>>> GetUserShops([FromBody] GetUserShopsRequest request)
        {
            Response<List<SShop>> response = service.GetUserShops(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<SShop>>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("get-messages")]
        public async Task<ActionResult<Response<List<string>>>> GetMessages([FromBody] GetMessagesRequest request)
        {
            Response<List<string>> response = await Task.Run(() => service.GetMessages(request.SessionId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<List<string>>.sendOkResponse(response.Value));
            }
        }
        [HttpPost]
        [Route("get-messages-number")]
        public async Task<ActionResult<Response<int>>> GetMessagesNumberRequest([FromBody] GetMessagesRequest request)
        {
            Response<int> response = await Task.Run(() => service.GetMessagesNumberRequest(request.SessionId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<int>.sendOkResponse(response.Value));
            }
        }

        [HttpPost]
        [Route("is-admin")]
        public ActionResult<Response<bool>> IsAdmin([FromBody] IsAdminRequest request)
        {
            Response<bool> response = service.IsSystemAdmin(request.SessionId);
            if (response.ErrorOccured)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response.Value);
            }
        }

        [HttpPost]
        [Route("update-basket-quantity")]
        public async Task<ObjectResult> UpdateBasketItemQuantity([FromBody] UpdateBasketQuantityRequest request)
        {
            Response response = await Task.Run(() => service.UpdateBasketItemQuantity(request.SessionId, request.ShopId, request.ProductID, request.Quantity));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("update-basket-quantity-success"));
            }
        }

        [HttpPost]
        [Route("set-product-bid")]
        public async Task<ObjectResult> SetProductBid([FromBody] SetProductBidRequest request)
        {
            Response response = await Task.Run(() => service.BidOnProduct(request.SessionId, request.ShopId, request.ProductId, request.Quantity, request.SuggestedPriceForOne));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("set-product-bid"));
            }
        }

        [HttpPost]
        [Route("approve-bid")]
        public async Task<ObjectResult> ApproveBid([FromBody] ApproveBidRequest request)
        {
            Response response = await Task.Run(() => service.ApproveBid(request.SessionId, request.ShopId, request.BidUsername, request.ProductId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("approve-bid"));
            }
        }

        [HttpPost]
        [Route("decline-bid")]
        public async Task<ObjectResult> DeclineBid([FromBody] DeclineBidRequest request)
        {
            Response response = await Task.Run(() => service.DissapproveBid(request.SessionId, request.ShopId, request.BidUsername, request.ProductId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("decline-bid"));
            }
        }

        [HttpPost]
        [Route("offer-counter-bid")]
        public async Task<ObjectResult> OfferCounterBid([FromBody] OfferCounterBidRequest request)
        {
            Response response = await Task.Run(() => service.OfferCounterBid(request.SessionId, request.ShopId, request.BidUsername, request.ProductId, request.CounterPrice));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("offer-counter-bid"));
            }
        }

        [HttpPost]
        [Route("remove-bid")]
        public async Task<ObjectResult> RemoveBid([FromBody] RemoveBidRequest request)
        {
            Response response = await Task.Run(() => service.RemoveBid(request.SessionId, request.ShopId, request.BidUsername, request.ProductId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("remove-bid"));
            }
        }

        [HttpPost]
        [Route("approve-counter-bid")]
        public async Task<ObjectResult> ApproveCounterBid([FromBody] ApproveCounterBidRequest request)
        {
            Response response = await Task.Run(() => service.ApproveCounterBid(request.SessionId, request.ShopId, request.ProductId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("approve-counter-bid"));
            }
        }

        [HttpPost]
        [Route("approve-appointment")]
        public async Task<ObjectResult> ApproveAppointment([FromBody] ApproveAppointmentRequest request)
        {
            Response response = await Task.Run(() => service.ApproveAppointment(request.SessionId, request.ShopId, request.AppointeeUsername));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("approve-appointment"));
            }
        }

        [HttpPost]
        [Route("decline-appointment")]
        public async Task<ObjectResult> DeclineAppointment([FromBody] DeclineAppointmentRequest request)
        {
            Response response = await Task.Run(() => service.DeclineAppointment(request.SessionId, request.ShopId, request.AppointeeUsername));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("decline-appointment"));
            }
        }

        [HttpPost]
        [Route("decline-counter-bid")]
        public async Task<ObjectResult> DeclineCounterBid([FromBody] DeclineCounterBidRequest request)
        {
            Response response = await Task.Run(() => service.DeclineCounterBid(request.SessionId, request.ShopId, request.ProductId));
            if (response.ErrorOccured)
            {
                return BadRequest(ServerResponse<string>.sendBadResponse(response.ErrorMessage));
            }
            else
            {
                return Ok(ServerResponse<string>.sendOkResponse("decline-counter-bid"));
            }
        }
    }
}
