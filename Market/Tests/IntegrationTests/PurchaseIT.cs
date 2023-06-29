using Market.AT;
using Market.DataLayer;
using Market.DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Market.IntegrationTests
{
    [TestClass]
    public class PurchaseIT
    {
        UserManager UM;
        ShopManager SM;
        string PrimarysessionID;
        string sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int productID2;
        int price;
        SynchronizedCollection<String> keywords;
        MarketManager MM;

        public PurchaseIT()
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
            productID2 = 21;

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
        public void CreateShop()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(dummyShop.Name == "Regev Yohananof");
            Appointment app;
            if (!member.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Member.UserName == "regev");
        }

        [TestMethod]
        public void AddProductToShop()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Assert.IsTrue(!(dummyShop.Products.Count() == 0));


        }
        [TestMethod]

        public void AddProductToBasket()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");

            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            UM.AddToCart(PrimarysessionID, dummyShop, productID, quantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
        }
        [TestMethod]

        public void AddProductToBasketAndLogout()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            UM.AddToCart(PrimarysessionID, dummyShop, productID, quantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            UM.Logout(PrimarysessionID);
            UM.Login(PrimarysessionID, "regev", "password");
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
        }
        [TestMethod]

        public void PurchaseBasket()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity - 1;
            UM.AddToCart(PrimarysessionID, dummyShop, productID, newQuantity);
            UM.Purchase(PrimarysessionID, shopID);
            Assert.IsTrue(myprod.Quantity == quantity - newQuantity);

        }
        [TestMethod]

        public void PurchaseBasketWithTwoMembers_GoodCase()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity / 2;
            UM.AddToCart(PrimarysessionID, dummyShop, productID, newQuantity);
            string secondBuyerSessionID = "3";
            UM.Logout(PrimarysessionID);
            UM.Register("ben", "password");
            UM.Login(secondBuyerSessionID, "ben", "password");
            Member secondBuyer = UM.GetMember(secondBuyerSessionID);
            UM.AddToCart(secondBuyerSessionID, dummyShop, productID, newQuantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = newQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            if (!secondBuyer.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            totalprice = newQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            UM.Login(PrimarysessionID, "regev", "password");
            UM.Purchase(PrimarysessionID, shopID);
            UM.Purchase(secondBuyerSessionID, shopID);
            Assert.IsTrue(myprod.Quantity == (quantity - (newQuantity) * 2));

        }

        [TestMethod]

        public void PurchaseBasketWithTwoMembers_BadCase()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = 50;
            UM.AddToCart(PrimarysessionID, dummyShop, productID, newQuantity);
            string secondBuyerSessionID = "5";
            UM.Logout(PrimarysessionID);
            UM.Register("ben", "password");
            UM.Login(secondBuyerSessionID, "ben", "password");
            Member secondBuyer = UM.GetMember(secondBuyerSessionID);
            int majorQuantity = 60;
            UM.AddToCart(secondBuyerSessionID, dummyShop, productID, majorQuantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = newQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            if (!secondBuyer.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            totalprice = majorQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            UM.Purchase(secondBuyerSessionID, shopID);
            try
            {
                UM.Login(PrimarysessionID, "regev", "password");
                UM.Purchase(PrimarysessionID, shopID);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Product {myprod.Name}: In supply: {quantity - majorQuantity}, You required: {newQuantity}");
            }
            Assert.IsTrue(myprod.Quantity == (quantity - majorQuantity));
        }

        [TestMethod]
        public void PurchaseShoppingCartValidPurchase()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(10000);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity - 1;
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            Assert.IsTrue(myprod.Quantity == quantity - newQuantity);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp.PaymentId == 10000 && scp.DeliveryId == 10000);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Success);
        }

        [TestMethod]
        public void PurchaseShoppingCartInValidOrderId()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(-1);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(10000);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity - 1;
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            Assert.ThrowsException<Exception>(() => MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails()));
            Assert.IsTrue(myprod.Quantity == quantity);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp.PaymentId == -1 && scp.DeliveryId == -1);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Failed);
        }

        [TestMethod]
        public void PurchaseShoppingCartInValidPaymentId()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(-1);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity - 1;
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            Assert.ThrowsException<Exception>(() => MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails()));
            mockDeliverySystem.Verify((m) => m.CancelOrder(10000), Times.Once());
            Assert.IsTrue(myprod.Quantity == quantity);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp.PaymentId == -1 && scp.DeliveryId == -1);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Failed);
        }

        [TestMethod]
        public void PurchaseShoppingCartTwiceValidPurchase()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(15214);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = 50;
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            Assert.IsTrue(myprod.Quantity == quantity - newQuantity);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            ShoppingCartPurchase scp2 = member.UserPurchases.ElementAt(1);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp.PaymentId == 15214 && scp.DeliveryId == 10000);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Success);
            Assert.IsNotNull(scp2);
            Assert.IsTrue(scp2.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp2.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp2.PaymentId == 15214 && scp.DeliveryId == 10000);
            Assert.IsTrue(scp2.PurchaseStatus == PurchaseStatus.Success);
        }

        [TestMethod]
        public void PurchaseShoppingCartTwiceDifferentShopsValidPurchase()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(15214);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.CreateShop(PrimarysessionID, "Ben Express");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Shop dummyShop2 = SM.GetShop(2);

            MM.AddProduct(PrimarysessionID, 2, "apple2", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());


            Product myprod = dummyShop.Products.ElementAt(0);
            Product myprod2 = dummyShop2.Products.ElementAt(0);

            int newQuantity = 50;
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, newQuantity);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            Assert.IsTrue(myprod.Quantity == quantity - newQuantity);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            MM.AddToCart(PrimarysessionID, dummyShop2.Id, productID2, newQuantity);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            ShoppingCartPurchase scp2 = member.UserPurchases.ElementAt(1);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod.Id);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp.PaymentId == 15214 && scp.DeliveryId == 10000);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Success);
            Assert.IsNotNull(scp2);
            Assert.IsTrue(scp2.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id == myprod2.Id);
            Assert.IsTrue(scp2.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity == newQuantity);
            Assert.IsTrue(scp2.PaymentId == 15214 && scp.DeliveryId == 10000);
            Assert.IsTrue(scp2.PurchaseStatus == PurchaseStatus.Success);
        }

        [TestMethod]
        public void FailPurchaseBeacusePurchasePolicyAddsToCartAndSuccessPurchase()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(15214);
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
            MM.AddProduct(PrimarysessionID, dummyShop.Id, "regularProduct", 0, "blabla", 854, 84, "Food", new List<string>());
            MM.AddQuantityRule(PrimarysessionID, dummyShop.Id, "Food",3,10);
            MM.AddPurchasePolicy(PrimarysessionID, dummyShop.Id, DateTime.Now.AddDays(2).ToString(), "Food", 11);
            MM.Logout(PrimarysessionID);
            Product p = dummyShop.Products.First();
            Product p2 = dummyShop.Products.Last();

            Assert.IsTrue(p.SellMethod.GetType().Name == "BidSell");
            MM.BidOnProduct(benSessionId, dummyShop.Id, p.Id, 2, 115);
            MM.Login(PrimarysessionID, "regev", "password");
            MM.ApproveBid(PrimarysessionID, dummyShop.Id, "ben", p.Id);
            Assert.ThrowsException<Exception>(() => MM.PurchaseShoppingCart(benSessionId, new PaymentDetails(),new DeliveryDetails()));
            MM.AddToCart(benSessionId, dummyShop.Id, p2.Id, 3);
            MM.PurchaseShoppingCart(benSessionId, new PaymentDetails(), new DeliveryDetails());
            ShoppingCartPurchase scp = ben.UserPurchases.ElementAt(0);
            Assert.IsNotNull(scp);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.FindBasketItem(p.Id)!=null);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.FindBasketItem(p2.Id) != null);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.FindBasketItem(p.Id).Quantity==2);
            Assert.IsTrue(scp.ShopPurchaseObjects.ElementAt(0).Basket.FindBasketItem(p2.Id).Quantity == 3);
            Assert.IsTrue(scp.PurchaseStatus == PurchaseStatus.Success);

        }

        [TestMethod]
        public void GuestPurchaseShoppingCartValidPurchase()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(10000);

            // Inject the mock implementations into the MM instance for this test method
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
            MM.Register(PrimarysessionID, "regev", "password");
            string guestSessionId = "1234";
            MM.Login(PrimarysessionID, "regev", "password");
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", price, quantity, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int newQuantity = quantity - 1;
            MM.EnterAsGuest(guestSessionId);
            MM.AddToCart(guestSessionId, dummyShop.Id, productID, newQuantity);
            MM.PurchaseShoppingCart(guestSessionId, new PaymentDetails(), new DeliveryDetails());
            Assert.IsTrue(myprod.Quantity == quantity - newQuantity);
            
        }
    }
}
