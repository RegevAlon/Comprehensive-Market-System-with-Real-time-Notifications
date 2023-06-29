using Market.DataLayer;
using Market.DomainLayer;
using Market.ServiceLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.IntegrationTests
{
    [TestClass]
    public class BidProductTests
    {
        UserManager UM;
        ShopManager SM;
        string PrimarysessionID;
        string sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int price;
        SynchronizedCollection<String> keywords;
        MarketManager MM;

        public BidProductTests()
        {
            UM = UserManager.GetInstance();
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
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void AddToCartBidProductFail()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            Assert.ThrowsException<Exception>(() => MM.AddToCart(PrimarysessionID, dummyShop.Id, p.Id, 5));
        }

        [TestMethod]
        public void AddBidOnProduct()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            Assert.IsTrue(p.SellMethod is BidSell);
            BidSell bidsell = (BidSell)p.SellMethod;
            Assert.IsTrue(bidsell.Bids.ContainsKey(ben.Id));
            Assert.IsTrue(member.Messages.Count > 0);
            string msg = $"Product Bid Event: Shop: \'{dummyShop.Name}\', Product name: \'{p.Name}\', " +
            $"Product ID: '{p.Id}', Quantity: {p.Quantity}," +
                $" Original Price: {p.Price}, Suggested price per one: " +
                $"{115}, Bidding member: {ben.UserName}";
            Message ownerMSG = member.Messages.First();
            Assert.AreEqual(ownerMSG.Comment, msg);
            Bid bid = bidsell.Bids[ben.Id];
            Assert.IsTrue(bid.SuggestedPrice == 115);
            Assert.IsTrue(bid.Quantity == 5);
            Assert.IsTrue(bid.AllApproved() == false);
        }

        [TestMethod]
        public void AddBidOnProductAndOwnerApprove()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Logout(benSessionId);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
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
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
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
            UM.Login(benSessionId, "ben", "password");
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
            UM.Register("regev", "password");
            UM.Register("tamuz", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Appoint(PrimarysessionID, "tamuz", dummyShop.Id, 2, 128);
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Logout(benSessionId);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.Login("2333", "tamuz", "password");
            Member tamuz = UM.GetMember("2333");

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
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
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
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(PrimarysessionID, dummyShop.Id,ben.UserName, p.Id));
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(benSessionId, dummyShop.Id, ben.UserName, p.Id));

        }

        [TestMethod]
        public void AddBidOnProductAndOwnerApproveUserPurchaseAndBidAgain()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
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
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            UM.Register("ben", "password");
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            UM.Register("gal", "password");
            string galSessionId = "12345";
            UM.Login(galSessionId, "gal", "password");
            Member gal = UM.GetMember(galSessionId);
            MM.Appoint(PrimarysessionID, "gal", dummyShop.Id, 3, 8);
            Assert.ThrowsException<Exception>(() => MM.RemoveBid(galSessionId, dummyShop.Id, "ben", p.Id));
        }

        [TestMethod]
        public void AddBidOnProductAnd3OwnersApprove()
        {
            UM.Register("regev", "password");
            UM.Register("tamuz", "password");
            UM.Register("ben", "password");

            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            string benSessionId = "3";
            UM.Login(benSessionId, "ben", "password");
            Member ben = UM.GetMember(benSessionId);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "bidProduct", 1, "blabla", 123.34, 20, "Food", new List<string>());
            MM.Logout(PrimarysessionID);

            Product p = dummyShop.Products.First();
            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.Appoint(PrimarysessionID, "tamuz", dummyShop.Id, 2, 128);
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 5, 115);

            MM.Logout(benSessionId);
            MM.Login("2333", "tamuz", "password");
            Member tamuz = UM.GetMember("2333");

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


    }
}
