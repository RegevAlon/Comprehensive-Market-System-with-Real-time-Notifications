using Market.DataLayer;
using Market.DomainLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.ServiceLayer;

namespace Tests.AT
{
    [TestClass]
    public class BidProductTest
    {
        UserManager marketservice;
        ShopManager SM;
        string PrimarysessionID;
        string PrimarysessionID2;
        string sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int price;
        SynchronizedCollection<String> keywords;
        MarketManager MM;
        private MarketService service;
        public BidProductTest()
        {
            MarketService service = MarketService.GetInstance();
            marketservice = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
                .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
                .Returns(true);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
        }

        [TestInitialize]
        public void Setup()
        {
            MarketContext.GetInstance().Dispose();
            PrimarysessionID = "1";
            PrimarysessionID2 = "2";
            shopID = 1;
            productID = 11;
            price = 20;
            quantity = 100;
            keywords = new SynchronizedCollection<string>();
            keywords.Add("green apple");
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
                .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
                .Returns(true);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;

        }

        [TestCleanup]
        public void Cleanup()
        {
            MarketService.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void AddToCartBidProductFail()
        {
            MarketService service = MarketService.GetInstance();
            service.EnterAsGuest(PrimarysessionID);
            service.Register(PrimarysessionID, "regev", "password");
            service.Login(PrimarysessionID, "regev", "password");
            service.CreateShop(PrimarysessionID, "Regev Yohananof");
            SShop dummyShop = service.GetShopInfo(PrimarysessionID, shopID).Value;
            Assert.IsTrue(dummyShop.name == "Regev Yohananof");
            service.AddProduct(PrimarysessionID, dummyShop.id, "bidProduct", 1,
                "blabla", 123.34, 20, "Food",
                new List<string>());
            dummyShop = service.GetShopInfo(PrimarysessionID, shopID).Value;
            SProduct p = dummyShop.products.First();
            Assert.IsTrue(p.sellType == 1);
            Assert.ThrowsException<Exception>(() => MM.AddToCart(PrimarysessionID, dummyShop.id, p.id, 5));
        }

        [TestMethod]
        public void AddBidOnProduct()
        {
            MarketService service = MarketService.GetInstance();
            service.Register(PrimarysessionID,"regev", "password");
            service.Login(PrimarysessionID, "regev", "password");
            SUser member= (SMember)service.GetUser(PrimarysessionID).Value;
            string benSessionId = "3";
            service.Register(benSessionId, "ben", "password");
            
            service.Login(benSessionId, "ben", "password");
            SMember ben =(SMember) service.GetUser(benSessionId).Value;
            service.CreateShop(PrimarysessionID, "Regev Yohananof");
            SShop dummyShop = service.GetShopInfo(PrimarysessionID,shopID).Value;
            Assert.IsTrue(dummyShop.name == "Regev Yohananof");
            service.AddProduct(PrimarysessionID, dummyShop.id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            dummyShop = service.GetShopInfo(PrimarysessionID, shopID).Value;
            //service.Logout(PrimarysessionID);
            
            SProduct p = dummyShop.products.First();
            Assert.IsTrue(p.sellType == 1);
            service.BidOnProduct(benSessionId, dummyShop.id, p.id, 5, 115);
            Assert.IsTrue(p.sellType == 1);
            BidSell bidsell = (BidSell)SM.GetShop(shopID).Products.First().SellMethod;
            Assert.IsTrue(bidsell.Bids.ContainsKey(ben.Id));
            Assert.IsTrue(service.GetMessages(PrimarysessionID).Value.Count > 0);
            string msg = $"Product Bid Event: Shop: \'{dummyShop.name}\', Product name: \'{p.name}\', " +
                         $"Product ID: '{p.id}', Quantity: {p.quantity}," +
                         $" Original Price: {p.price}, Suggested price per one: " +
                         $"{115}, Bidding member: {ben.Username}";
            string ownerMSG = service.GetMessages(PrimarysessionID).Value.First();
            Assert.AreEqual(ownerMSG, msg);
            Bid bid = bidsell.Bids[ben.Id];
            Assert.IsTrue(bid.SuggestedPrice == 115);
            Assert.IsTrue(bid.Quantity == 5);
            Assert.IsTrue(bid.AllApproved() == false);
        }

        [TestMethod]
        public void AddBidOnProductAndOwnerApprove()
        {
            MarketService service = MarketService.GetInstance();
            service.Register(PrimarysessionID,"regev", "password");
            service.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            string benSessionId = "3";
            service.Register(benSessionId,"ben", "password");
            
            service.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            service.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            service.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            service.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            service.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            service.Logout(benSessionId);
            service.Login(PrimarysessionID, "regev", "password");
            service.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            Assert.IsTrue(ben.Messages.Count > 0);
            string msg = $"Bid approved. Shop: {dummyShop.Name}, Product: \'{p.Name}\' added to your cart";
            Message benMSG = ben.Messages.First();
            Assert.AreEqual(benMSG.Comment, msg);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id] != null);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id].HasProduct(p));
            BasketItem basketItem = ben.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(p.Id);
            Assert.IsTrue(basketItem.Quantity == 5);
            Assert.IsTrue(basketItem.PriceAfterDiscount == 115);
        }

        [TestMethod]
        public void AddBidOnProductAndOwnerCounterAndMemberApprove()
        {
            marketservice.Register("regev", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Logout(benSessionId);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.OfferCounterBid(PrimarysessionID, dummyShop.Id, "ben", p.Id, 119);
            Assert.IsTrue(ben.Messages.Count > 0);
            string msg = $"Product Counter Bid Event: Shop: \'{dummyShop.Name}\', Product name: \'{p.Name}\', " +
                         $"Product ID: '{p.Id}', Quantity: {p.Quantity}," +
                         $" Original Price: {p.Price}, Member Suggested price per one:" +
                         $" {115}, Bidding member: {ben.UserName}, New Counter Price: {119}," +
                         $" Bidding Owner: {member.UserName}";
            Message benMSG = ben.Messages.First();
            Assert.AreEqual(benMSG.Comment, msg);
            marketservice.Login(benSessionId, "ben", "password");
            Assert.IsFalse(ben.ShoppingCart.BasketbyShop.ContainsKey(dummyShop.Id));
            MM.ApproveCounterBid(benSessionId, dummyShop.Id, p.Id);
            msg = $"Bid approved. Shop: {dummyShop.Name}, Product: \'{p.Name}\' added to your cart";
            benMSG = ben.Messages.Last();
            Assert.AreEqual(benMSG.Comment, msg);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id] != null);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id].HasProduct(p));
            BasketItem basketItem = ben.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(p.Id);
            Assert.IsTrue(basketItem.Quantity == 5);
            Assert.IsTrue(basketItem.PriceAfterDiscount == 119);
        }

        [TestMethod]
        public void AddBidOnProductAnd2OwnersApprove()
        {
            marketservice.Register("regev", "password");
            marketservice.Register("tamuz", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Appoint(PrimarysessionID, "tamuz", dummyShop.Id, 2, 128);
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Logout(benSessionId);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.Login("2333", "tamuz", "password");
            Member tamuz = marketservice.GetMember("2333");

            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            MM.ApproveBid("2333", dummyShop.Id, "ben", p.Id);
            Assert.IsTrue(ben.Messages.Count > 0);
            string msg = $"Bid approved. Shop: {dummyShop.Name}, Product: \'{p.Name}\' added to your cart";
            Message benMSG = ben.Messages.First();
            Assert.AreEqual(benMSG.Comment, msg);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id] != null);
            Assert.IsTrue(ben.ShoppingCart.BasketbyShop[dummyShop.Id].HasProduct(p));
            BasketItem basketItem = ben.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(p.Id);
            Assert.IsTrue(basketItem.Quantity == 5);
            Assert.IsTrue(basketItem.PriceAfterDiscount == 115);
        }

        [TestMethod]
        public void AddBidOnProductAndOwnerApproveAndUserRemoveFromCart()
        {
            marketservice.Register("regev", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            MM.RemoveFromCart(benSessionId, dummyShop.Id, p.Id);
            Assert.IsTrue(((BidSell)p.SellMethod).Bids.IsEmpty);
        }

        [TestMethod]

        public void AddBidOnProductAndOwnerApproveAndUserRemoveBid()
        {
            marketservice.Register("regev", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(PrimarysessionID, dummyShop.Id, ben.UserName, p.Id));
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(benSessionId, dummyShop.Id, ben.UserName, p.Id));

        }

        [TestMethod]
        public void AddBidOnProductAndOwnerApproveUserPurchaseAndBidAgain()
        {
            marketservice.Register("regev", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            MM.PurchaseShoppingCart(benSessionId, new PaymentDetails(), new DeliveryDetails());
            Assert.IsFalse(ben.ShoppingCart.BasketbyShop.ContainsKey(dummyShop.Id));
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 3, 13);
            BidSell bidsell = (BidSell)p.SellMethod;
            Assert.IsTrue(bidsell.Bids.ContainsKey(ben.Id));
            Bid bid = bidsell.Bids[ben.Id];
            Assert.IsTrue(bid.SuggestedPrice == 13);
            Assert.IsTrue(bid.Quantity == 3);
            Assert.IsTrue(bid.AllApproved() == false);

        }

        [TestMethod]
        public void AddBidOnProductAndManagerTryRemoveWithoutPermissions()
        {
            marketservice.Register("regev", "password");
            marketservice.Login(PrimarysessionID, "regev", "password");
            Member member = marketservice.GetMember(PrimarysessionID);
            marketservice.Register("ben", "password");
            string benSessionId = "3";
            marketservice.Login(benSessionId, "ben", "password");
            Member ben = marketservice.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food",
                new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            marketservice.Register("gal", "password");
            string galSessionId = "12345";
            marketservice.Login(galSessionId, "gal", "password");
            Member gal = marketservice.GetMember(galSessionId);
            MM.Appoint(PrimarysessionID, "gal", dummyShop.Id, 3, 8);
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(galSessionId, dummyShop.Id, "ben", p.Id));
        }
    }
}