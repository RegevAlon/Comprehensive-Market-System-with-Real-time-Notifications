using Azure.Messaging;
using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer;
using Market.IntegrationTests;
using Market.ServiceLayer;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Tests.UnitTests
{
    [TestClass]
    public class MarketContextTests
    {
        MarketContext _context = MarketContext.GetInstance();
        [TestInitialize]
        public void Initialize()
        {
            _context.Dispose();

        }
        [TestCleanup]
        public void CleanUp()
        {
            _context.Dispose();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
        }
        [TestMethod]
        public void DisposeTest()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void MarketContextMemberAdd()
        {
            MemberDTO member1 = new MemberDTO(1, "tamuz", "123", true);
            var add1 = _context.Add<MemberDTO>(member1);
            _context.SaveChanges();
            Assert.AreEqual(_context.Find<MemberDTO>(member1.Id), member1);
            MemberDTO queryMember1 = (_context.Members.Where(m => m.Id == member1.Id)).ToArray<MemberDTO>()[0];
            Assert.AreEqual(queryMember1, member1);

            string messageContent1 = "First check of messages!";
            string messageContent2 = "Gigi HaManiak";

            MessageDTO message1 = new MessageDTO(new Message(messageContent1));
            MessageDTO message2 = new MessageDTO(new Message(messageContent2));

            List<MessageDTO> messages2 = new List<MessageDTO>() { message1, message2 };
            MemberDTO member2 = new MemberDTO(2, "gal", "blabla1010", messages2, false);
            _context.Add<MemberDTO>(member2);
            _context.SaveChanges();
            Assert.AreEqual(_context.Find<MemberDTO>(member2.Id), member2);
            MemberDTO queryMember2 = (_context.Members.Where(m => m.Id == member2.Id)).ToArray<MemberDTO>()[0];
            Assert.AreEqual(queryMember2, member2);
            _context.SaveChanges();
            Assert.AreEqual(_context.Find<MessageDTO>(message1.Id).MessageContent, message1.MessageContent);
            MessageDTO queryMessage1 = (_context.Messages.Where(m => m.Id == message1.Id)).ToArray<MessageDTO>()[0];
            Assert.AreEqual(queryMessage1.MessageContent, message1.MessageContent);
        }

        [TestMethod]
        public void MarketContextShopAdd()
        {
            MemberDTO member1 = new MemberDTO(1, "tamuz", "123", true);
            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(1, 1, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            //string reviewerUsername, double rate, string comment
            string reviewComment1 = "Great product. Had betters though";
            string reviewComment2 = "Doesn't worth the money";
            ReviewDTO review1 = new ReviewDTO("Gal", 4.2, reviewComment1);
            ReviewDTO review2 = new ReviewDTO("Ben", 2, reviewComment2);
            //string name, double price, int quantity, int category, string description, string keywords, List< ReviewDTO > reviews)

            ProductDTO product1 = new ProductDTO(1, "WaterMelon", 30.02, 50, "Fruits", "Best Watermelon in town!", "Fruits,Melons,Green,Summer Fruits", new List<ReviewDTO> { review1, review2 });

            string reviewComment3 = "Great Pumpkin!";
            string reviewComment4 = "Pricy...";
            ReviewDTO review3 = new ReviewDTO("User123", 5, reviewComment3);
            ReviewDTO review4 = new ReviewDTO("Guest11", 1, reviewComment4);
            ProductDTO product2 = new ProductDTO(2, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", new List<ReviewDTO> { review3, review4 });
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.SaveChanges();


            _context.Add<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            Assert.AreEqual(_context.Find<ShopDTO>(shop.Id), shop);
            ShopDTO queryShop = (_context.Shops.Where(m => m.Id == shop.Id)).ToArray<ShopDTO>()[0];
            Assert.AreEqual(shop, queryShop);

            AppointmentDTO queryAppoint = _context.Find<AppointmentDTO>(appointment1.MemberId, appointment1.ShopId);
            Assert.AreEqual(appointment1, queryAppoint);

        }

        [TestMethod]
        public void MarketContextAppoints()
        {
            MemberDTO member1 = new MemberDTO(1, "Tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Gal", "321", true);

            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(1, 1, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            //string reviewerUsername, double rate, string comment
            string reviewComment1 = "Great product. Had betters though";
            string reviewComment2 = "Doesn't worth the money";
            ReviewDTO review1 = new ReviewDTO("Gal", 4.2, reviewComment1);
            ReviewDTO review2 = new ReviewDTO("Ben", 2, reviewComment2);
            //string name, double price, int quantity, int category, string description, string keywords, List<ReviewDTO> reviews)
            ProductDTO product1 = new ProductDTO(1, "WaterMelon", 30.02, 50, "Fruits", "Best Watermelon in town!", "Fruits,Melons,Green,Summer Fruits", new List<ReviewDTO> { review1, review2 });

            string reviewComment3 = "Great Pumpkin!";
            string reviewComment4 = "Pricy...";
            ReviewDTO review3 = new ReviewDTO("User123", 5, reviewComment3);
            ReviewDTO review4 = new ReviewDTO("Guest11", 1, reviewComment4);
            ProductDTO product2 = new ProductDTO(2, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", new List<ReviewDTO> { review3, review4 });
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            Assert.AreEqual(_context.Find<ShopDTO>(shop.Id), shop);
            ShopDTO queryShop = (_context.Shops.Where(m => m.Id == shop.Id)).ToArray<ShopDTO>()[0];
            Assert.AreEqual(shop, queryShop);

            AppointmentDTO queryAppoint = _context.Find<AppointmentDTO>(appointment1.MemberId, appointment1.ShopId);
            Assert.AreEqual(appointment1, queryAppoint);

            AppointmentDTO appointment2 = new AppointmentDTO(1, 2, member1, new List<MemberDTO>(), "Manager", ((int)Market.DomainLayer.Permission.Appoint));
            appointment1.Appointees.Add(new AppointeesDTO(member2));
            _context.Add<AppointmentDTO>(appointment2);

            _context.SaveChanges();

            AppointmentDTO queryAppoint2 = _context.Find<AppointmentDTO>(appointment2.MemberId, appointment2.ShopId);
            Assert.AreEqual(appointment2, queryAppoint2);

            _context.Remove<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            AppointmentDTO queryAppoint3 = _context.Find<AppointmentDTO>(appointment1.MemberId, appointment1.ShopId);
            Assert.IsNull(queryAppoint3);

        }

        [TestMethod]
        public void MarketContextAddToBasket()
        {
            MemberDTO member1 = new MemberDTO(1, "tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Benchuk", "321", false);

            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(1, 1, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            //string reviewerUsername, double rate, string comment
            string reviewComment1 = "Great product. Had betters though";
            string reviewComment2 = "Doesn't worth the money";
            ReviewDTO review1 = new ReviewDTO("Gal", 4.2, reviewComment1);
            ReviewDTO review2 = new ReviewDTO("Ben", 2, reviewComment2);
            //string name, double price, int quantity, int category, string description, string keywords, List< ReviewDTO > reviews)

            ProductDTO product1 = new ProductDTO(1, "WaterMelon", 30.02, 50, "Fruits", "Best Watermelon in town!", "Fruits,Melons,Green,Summer Fruits", new List<ReviewDTO> { review1, review2 });

            string reviewComment3 = "Great Pumpkin!";
            string reviewComment4 = "Pricy...";
            ReviewDTO review3 = new ReviewDTO("User123", 5, reviewComment3);
            ReviewDTO review4 = new ReviewDTO("Guest11", 1, reviewComment4);
            ProductDTO product2 = new ProductDTO(2, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", new List<ReviewDTO> { review3, review4 });
            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.Add<MemberDTO>(member2);
            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);

            //member1.Appointments.Add(appointment1);
            //shop.Appointments.Add(appointment1);
            _context.SaveChanges();


            //int shopId, int shoppingCartId
            BasketDTO basket1 = new BasketDTO(shop.Id, new List<BasketItemDTO>());
            //int id, int productId, double priceAfterDiscount, int quantity
            BasketItemDTO basketItem1 = new BasketItemDTO(product1, 28.77, 900, 10);
            BasketItemDTO basketItem2 = new BasketItemDTO(product2, 10000, 100000, 2);
            basket1.BasketItems.Add(basketItem1);
            basket1.BasketItems.Add(basketItem2);
            member2.ShoppingCart.Baskets.Add(basket1);
            _context.SaveChanges();


            ShoppingCartDTO queryShoppingCart = _context.Find<ShoppingCartDTO>(member2.ShoppingCart.Id);
            Assert.AreEqual(basket1, queryShoppingCart.Baskets[0]);

            BasketItemDTO queryBasketItem1 = _context.Find<BasketItemDTO>(basketItem1.Id);
            Assert.AreEqual(basketItem1, queryBasketItem1);

            BasketItemDTO queryBasketItem2 = _context.Find<BasketItemDTO>(basketItem2.Id);
            Assert.AreEqual(basketItem2, queryBasketItem2);

        }
        [TestMethod]
        public void MarketContextPurchase()
        {
            MemberDTO member1 = new MemberDTO(1, "tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Benchuk", "321", false);

            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            ShopDTO shop2 = new ShopDTO(2, "Ben's Shop", true, 2.0);
            AppointmentDTO appointment1 = new AppointmentDTO(1, 1, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));
            AppointmentDTO appointment2 = new AppointmentDTO(1, 2, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            //string reviewerUsername, double rate, string comment
            string reviewComment1 = "Great product. Had betters though";
            string reviewComment2 = "Doesn't worth the money";
            ReviewDTO review1 = new ReviewDTO("Gal", 4.2, reviewComment1);
            ReviewDTO review2 = new ReviewDTO("Ben", 2, reviewComment2);
            //string name, double price, int quantity, int category, string description, string keywords, List<ReviewDTO> reviews)

            ProductDTO product1 = new ProductDTO(1, "WaterMelon", 30.02, 50, "Fruits", "Best Watermelon in town!", "Fruits,Melons,Green,Summer Fruits", new List<ReviewDTO> { review1, review2 });

            string reviewComment3 = "Great Pumpkin!";
            string reviewComment4 = "Pricy...";
            ReviewDTO review3 = new ReviewDTO("User123", 5, reviewComment3);
            ReviewDTO review4 = new ReviewDTO("Guest11", 1, reviewComment4);
            ProductDTO product2 = new ProductDTO(2, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", new List<ReviewDTO> { review3, review4 });
            shop.Products.Add(product1);
            shop.Products.Add(product2);
            ProductDTO product3 = new ProductDTO(3, "Candys", 1, 1000, "sweets", "Candys, price per 1kg.", "Candys,Snacks,Party", new List<ReviewDTO> { });
            shop2.Products.Add(product3);


            _context.Add<ShopDTO>(shop);
            _context.Add<ShopDTO>(shop2);
            _context.Add<MemberDTO>(member1);
            _context.Add<MemberDTO>(member2);
            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);
            _context.Add<AppointmentDTO>(appointment2);

            //member1.Appointments.Add(appointment1);
            //shop.Appointments.Add(appointment1);
            //member1.Appointments.Add(appointment2);
            //shop2.Appointments.Add(appointment2);
            _context.SaveChanges();


            BasketDTO basket1 = new BasketDTO(shop.Id, new List<BasketItemDTO>());
            //int id, int productId, double priceAfterDiscount, int quantity
            BasketItemDTO basketItem1 = new BasketItemDTO(product1, 28.77, 900, 10);
            BasketItemDTO basketItem2 = new BasketItemDTO(product2, 10000, 100000, 2);
            basket1.BasketItems.Add(basketItem1);
            basket1.BasketItems.Add(basketItem2);
            member2.ShoppingCart.Baskets.Add(basket1);
            _context.SaveChanges();


            ShoppingCartDTO queryShoppingCart = _context.Find<ShoppingCartDTO>(member2.ShoppingCart.Id);
            Assert.AreEqual(1, queryShoppingCart.Baskets.Count);
            Assert.AreEqual(basket1, queryShoppingCart.Baskets[0]);

            BasketItemDTO queryBasketItem1 = _context.Find<BasketItemDTO>(basketItem1.Id);
            Assert.AreEqual(basketItem1, queryBasketItem1);

            BasketItemDTO queryBasketItem2 = _context.Find<BasketItemDTO>(basketItem2.Id);
            Assert.AreEqual(basketItem2, queryBasketItem2);


            BasketDTO basket2 = new BasketDTO(shop2.Id, new List<BasketItemDTO>());
            //int id, int productId, double priceAfterDiscount, int quantity
            BasketItemDTO basketItem3 = new BasketItemDTO(product3, 10, 20, 20);
            basket2.BasketItems.Add(basketItem3);
            member2.ShoppingCart.Baskets.Add(basket2);
            _context.SaveChanges();

            ShoppingCartDTO queryShoppingCart1 = _context.Find<ShoppingCartDTO>(member2.ShoppingCart.Id);
            Assert.AreEqual(basket2, queryShoppingCart1.Baskets[1]);

            BasketItemDTO queryBasketItem3 = _context.Find<BasketItemDTO>(basketItem3.Id);
            Assert.AreEqual(basketItem3, queryBasketItem3);


            //int id, int shopId, List<BasketItemDTO> items, int buyerId, double price, string purchaseStatus
            PurchaseDTO purchase1 = new PurchaseDTO(1, shop.Id, basket1.BasketItems, member2.Id, basketItem1.PriceAfterDiscount + basketItem2.PriceAfterDiscount, Market.DomainLayer.PurchaseStatus.Pending.ToString());
            PurchaseDTO purchase2 = new PurchaseDTO(2, shop2.Id, basket2.BasketItems, member2.Id, basketItem3.PriceAfterDiscount, Market.DomainLayer.PurchaseStatus.Pending.ToString());
            //int id, List<PurchaseDTO> shopsPurchases, double price, int purchaseStatus
            ShoppingCartPurchaseDTO shoppingCartPurchase = new ShoppingCartPurchaseDTO(1, new List<PurchaseDTO> { purchase1, purchase2 }, purchase1.Price + purchase2.Price, Market.DomainLayer.PurchaseStatus.Pending.ToString(), 1, 1);

            member2.ShoppingCartPurchases.Add(shoppingCartPurchase);
            shop.Purchases.Add(purchase1);
            shop2.Purchases.Add(purchase2);

            //_context.Add<ShoppingCartPurchaseDTO>(shoppingCartPurchase)
            ;            //_context.Update<MemberDTO>(member2);
            _context.SaveChanges();

            ShoppingCartPurchaseDTO queryShoppingCartPurchase = _context.Find<ShoppingCartPurchaseDTO>(shoppingCartPurchase.Id);
            Assert.AreEqual(shoppingCartPurchase, queryShoppingCartPurchase);

            PurchaseDTO queryPurchase1 = _context.Find<PurchaseDTO>(purchase1.Id);
            Assert.AreEqual(purchase1, queryPurchase1);

            PurchaseDTO queryPurchase2 = _context.Find<PurchaseDTO>(purchase2.Id);
            Assert.AreEqual(purchase2, queryPurchase2);
        }
        [TestMethod]
        public void MarketContextAddPoliciesAndRules()
        {
            //make shop to add the rules and policies to
            MemberDTO member1 = new MemberDTO(1, "tamuz", "123", true);
            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(1, 1, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            //make products for rules
            string reviewComment1 = "Great product. Had betters though";
            string reviewComment2 = "Doesn't worth the money";
            string reviewComment3 = "Great Pumpkin!";
            string reviewComment4 = "Pricy...";

            ReviewDTO review1 = new ReviewDTO("Gal", 4.2, reviewComment1);
            ReviewDTO review2 = new ReviewDTO("Ben", 2, reviewComment2);
            ReviewDTO review3 = new ReviewDTO("User123", 5, reviewComment3);
            ReviewDTO review4 = new ReviewDTO("Guest11", 1, reviewComment4);

            ProductDTO product1 = new ProductDTO(1, "WaterMelon", 30.02, 50, "Fruits", "Best Watermelon in town!", "Fruits,Melons,Green,Summer Fruits", new List<ReviewDTO> { review1, review2 });
            ProductDTO product2 = new ProductDTO(2, "Pumpkin", 15, 25, "Vegtables", "Sliced pumpkin, price per 1kg.", "Vegtables,Melons,Orange", new List<ReviewDTO> { review3, review4 });

            shop.Products.Add(product1);
            shop.Products.Add(product2);

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            //just to make sure the shop is in the DB
            Assert.AreEqual(_context.Find<ShopDTO>(shop.Id), shop);
            ShopDTO queryShop = (_context.Shops.Where(m => m.Id == shop.Id)).ToArray<ShopDTO>()[0];
            Assert.AreEqual(shop, queryShop);

            ProductDTO emptyProduct = new ProductDTO(-1, "Dummy Product", 0, 0, "Non", "Do not use!", "", new List<ReviewDTO>());
            RuleSubjectDTO ruleSubjet1 = new RuleSubjectDTO(product1, "null");
            RuleSubjectDTO ruleSubjet2 = new RuleSubjectDTO(emptyProduct, "Fruits");
            RuleSubjectDTO ruleSubjet3 = new RuleSubjectDTO(product2, "null");


            PolicySubjectDTO policySubjet1 = new PolicySubjectDTO(product1, "null");
            PolicySubjectDTO policySubjet2 = new PolicySubjectDTO(emptyProduct, "Vegtables");
            PolicySubjectDTO policySubjet3 = new PolicySubjectDTO(emptyProduct, "Food");


            QuantityRuleDTO quantityRule1 = new QuantityRuleDTO(ruleSubjet1, 10, 1000);
            QuantityRuleDTO quantityRule2 = new QuantityRuleDTO(ruleSubjet3, 10, 1000);
            TotalPriceRuleDTO totalPriceRule = new TotalPriceRuleDTO(ruleSubjet2, 200);
            SimpleRuleDTO simpleRule = new SimpleRuleDTO(ruleSubjet1);
            CompositeRuleDTO compositeRule = new CompositeRuleDTO(ruleSubjet1, new List<RuleDTO> { quantityRule1, quantityRule2 }, "xor");

            shop.Rules.Add(quantityRule1);
            shop.Rules.Add(quantityRule2);
            shop.Rules.Add(totalPriceRule);
            shop.Rules.Add(simpleRule);
            _context.SaveChanges();

            shop.Rules.Add(compositeRule);

            _context.SaveChanges();

            RuleDTO queryRule1 = _context.Find<RuleDTO>(quantityRule1.Id);
            Assert.AreEqual(quantityRule1, queryRule1);

            RuleDTO queryRule2 = _context.Find<RuleDTO>(quantityRule2.Id);
            Assert.AreEqual(quantityRule2, queryRule2);

            RuleDTO querytotalPriceRule = _context.Find<RuleDTO>(totalPriceRule.Id);
            Assert.AreEqual(totalPriceRule, querytotalPriceRule);

            RuleDTO querySimpleRule = _context.Find<RuleDTO>(simpleRule.Id);
            Assert.AreEqual(simpleRule, querySimpleRule);

            RuleDTO queryCompositeRule = _context.Find<RuleDTO>(compositeRule.Id);
            Assert.AreEqual(compositeRule, queryCompositeRule);

            //just to make sure the shop is in the DB
            Assert.AreEqual(_context.Find<ShopDTO>(shop.Id), shop);
            ShopDTO queryShopWithRules = (_context.Shops.Where(m => m.Id == shop.Id)).ToArray<ShopDTO>()[0];
            Assert.AreEqual(shop, queryShopWithRules);

            PurchasePolicyDTO purPolicy1 = new PurchasePolicyDTO(11, DateTime.Today.AddDays(60), totalPriceRule.Id, policySubjet2);
            PurchasePolicyDTO purPolicy2 = new PurchasePolicyDTO(12, DateTime.Today.AddDays(60), quantityRule1.Id, policySubjet3);
            //to add to the shop
            DiscountPolicyDTO disPolicy = new DiscountPolicyDTO(13, DateTime.Today.AddDays(300), simpleRule.Id, policySubjet1, 10);
            DiscountCompositePolicyDTO disCompPolicy = new DiscountCompositePolicyDTO(14, DateTime.Today.AddDays(60), simpleRule.Id, policySubjet3, 25, "Add", new List<PolicyDTO> { purPolicy1, purPolicy2 });
            shop.Policies.Add(purPolicy1);
            shop.Policies.Add(purPolicy2);
            shop.Policies.Add(disPolicy);
            _context.SaveChanges();

            shop.Policies.Add(disCompPolicy);

            _context.SaveChanges();

            PolicyDTO queryPurPolicy1 = _context.Find<PolicyDTO>(purPolicy1.Id);
            Assert.AreEqual(purPolicy1, queryPurPolicy1);

            PolicyDTO queryPurPoliciy2 = _context.Find<PolicyDTO>(purPolicy2.Id);
            Assert.AreEqual(purPolicy2, queryPurPoliciy2);

            PolicyDTO queryDisPolicy = _context.Find<PolicyDTO>(disPolicy.Id);
            Assert.AreEqual(disPolicy, queryDisPolicy);

            PolicyDTO queryDisCompPolicy = _context.Find<PolicyDTO>(disCompPolicy.Id);
            Assert.AreEqual(disCompPolicy, queryDisCompPolicy);

            //just to make sure the shop is in the DB
            Assert.AreEqual(_context.Find<ShopDTO>(shop.Id), shop);
            ShopDTO queryShopWithPolicies = (_context.Shops.Where(m => m.Id == shop.Id)).ToArray<ShopDTO>()[0];
            Assert.AreEqual(shop, queryShopWithPolicies);
        }
        [TestMethod]
        public void MarketContextEvents()
        {
            MemberDTO member1 = new MemberDTO(1, "Tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Gal", "321", true);
            MemberDTO member3 = new MemberDTO(3, "Ben", "111", true);

            ShopDTO shop1 = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            ShopDTO shop2 = new ShopDTO(2, "Tamuz's Shop", true, 5.0);

            _context.Add<MemberDTO>(member1);
            _context.Add<MemberDTO>(member2);
            _context.Add<MemberDTO>(member3);

            _context.Add<ShopDTO>(shop1);
            _context.Add<ShopDTO>(shop2);

            _context.SaveChanges();

            string eventName1 = "Add Appointment Event";
            string eventName2 = "Product Sell Event";
            string eventName3 = "Report Event";
            string eventName4 = "Message Event";

            EventDTO event1 = new EventDTO(eventName1, shop1.Id, member1);
            EventDTO event2 = new EventDTO(eventName2, shop1.Id, member1);

            EventDTO event3 = new EventDTO(eventName3, shop1.Id, member1);
            EventDTO event4 = new EventDTO(eventName3, shop1.Id, member2);

            EventDTO event5 = new EventDTO(eventName4, shop1.Id, member2);
            EventDTO event6 = new EventDTO(eventName4, shop2.Id, member2);

            _context.Add<EventDTO>(event1);
            _context.Add<EventDTO>(event2);
            _context.Add<EventDTO>(event3);
            _context.Add<EventDTO>(event4);
            _context.Add<EventDTO>(event5);
            _context.Add<EventDTO>(event6);
            _context.SaveChanges();

            List<MemberDTO> listeners1 = MarketContext.GetInstance().Events.Where(e => e.ShopId == shop1.Id & e.Name.ToLower().Equals(eventName1.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners1.Count);
            Assert.AreEqual(listeners1[0].Id, member1.Id);

            List<MemberDTO> listeners2 = MarketContext.GetInstance().Events.Where(e => e.ShopId == shop1.Id & e.Name.ToLower().Equals(eventName2.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners2.Count);
            Assert.AreEqual(listeners2[0].Id, member1.Id);


            List<MemberDTO> listeners3 = MarketContext.GetInstance().Events.Where(e => e.ShopId == shop1.Id & e.Name.ToLower().Equals(eventName3.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(2, listeners3.Count);
            Assert.AreEqual(listeners3[0].Id, member1.Id);
            Assert.AreEqual(listeners3[1].Id, member2.Id);

            List<MemberDTO> listeners4 = MarketContext.GetInstance().Events.Where(e => e.ShopId == shop1.Id & e.Name.ToLower().Equals(eventName4.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners4.Count);
            Assert.AreEqual(listeners4[0].Id, member2.Id);

            List<MemberDTO> listeners5 = MarketContext.GetInstance().Events.Where(e => e.ShopId == shop2.Id & e.Name.ToLower().Equals(eventName4.ToLower())).Select(e => e.Listener).ToList();
            Assert.AreEqual(1, listeners5.Count);
            Assert.AreEqual(listeners5[0].Id, member2.Id);

        }
        [TestMethod]
        public void MarketContextPendingAgreements()
        {
            MemberDTO member1 = new MemberDTO(1, "Tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Gal", "321", true);
            MemberDTO member3 = new MemberDTO(3, "Ben", "111", true);


            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(member1.Id, shop.Id, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.Add<MemberDTO>(member2);
            _context.Add<MemberDTO>(member3);

            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            AppointmentDTO queryAppoint = _context.Find<AppointmentDTO>(appointment1.MemberId, appointment1.ShopId);
            Assert.AreEqual(appointment1, queryAppoint);

            //The appoint will be added sucssefully because there are no owners to give their approve
            AppointmentDTO appointment2 = new AppointmentDTO(member2.Id, shop.Id, member1, new List<MemberDTO>(), "Owner", ((int)Market.DomainLayer.Permission.All));

            appointment1.Appointees.Add(new AppointeesDTO(member2));
            _context.Add<AppointmentDTO>(appointment2);
            _context.SaveChanges();

            AppointmentDTO queryAppoint2 = _context.Find<AppointmentDTO>(appointment2.MemberId, appointment2.ShopId);
            Assert.AreEqual(appointment2, queryAppoint2);

            //The appoint will not be added sucssefully because member2 needs to approve
            PendingAgreementDTO pending1 = new PendingAgreementDTO(shop.Id, member1.Id, member3.Id);
            AgreementAnswerDTO ans1 = new AgreementAnswerDTO(member2.Id, "Pending");
            pending1.Answers.Add(ans1);
            shop.PendingAgreements.Add(pending1);
            _context.Update<ShopDTO>(shop);
            _context.SaveChanges();
            ans1.Answer = "Approved";
            _context.SaveChanges();

            PendingAgreementDTO queryPending1 = _context.Find<PendingAgreementDTO>(pending1.ShopId, pending1.AppointeeId);
            Assert.AreEqual(pending1, queryPending1);
            Assert.AreEqual(ans1, queryPending1.Answers[0]);
            Assert.AreEqual("Approved", queryPending1.Answers[0].Answer);


        }
    }
}