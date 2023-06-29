using Market.AT;
using Market.DataLayer;
using Market.DomainLayer;
using Market.RepoLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.IntegrationTests
{
    [TestClass]
    public class LoadingFromDataBaseTests
    {
        UserManager UM;
        ShopManager SM;
        MarketManager MM;
        MarketContext MC;
        string PrimarysessionID;
        string sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int price;
        SynchronizedCollection<String> keywords;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;
        Shop shop;

        public LoadingFromDataBaseTests()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();
            MC = MarketContext.GetInstance();
        }


        [TestInitialize]
        public void Setup()
        {
            MarketManager MM = MarketManager.GetInstance();
            MC.Dispose();
            PrimarysessionID = "1";
            shopID = 1;
            productID = 10;
            price = 20;
            quantity = 100;
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            BadPermission = 1;
            keywords = new SynchronizedCollection<string>();
            keywords.Add("green apple");
            UM.EnterAsGuest(PrimarysessionID);
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            shop = MM.GetShopInfo(PrimarysessionID, 1);
            MM.AddProduct(PrimarysessionID, shop.Id, "regularProduct", 0, "blablkabla", 452.23, 70, "Food", keywords.ToList());
            MM.AddProduct(PrimarysessionID, shop.Id, "bidProduct", 1, "blassdfblkabla", 25.2, 100, "Food", keywords.ToList());
            SM.Shops.Shops = new System.Collections.Concurrent.ConcurrentDictionary<int, Shop>();
            MM.AddToCart(PrimarysessionID, 1, 11, 5);
            MM.AddQuantityRule(PrimarysessionID, 1, "Food", 1, 100);
            MM.AddPurchasePolicy(PrimarysessionID, 1, DateTime.Now.AddDays(8).ToString(),"Food",11);
            MM.AddDiscountPolicy(PrimarysessionID, 1, DateTime.Now.AddDays(8).ToString(), "Food",11,0.3);
            MM.Register("12143","regev2", "password");


            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();

            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(10000);

            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;

            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            MM.AddToCart(PrimarysessionID, 1, 11, 5);
            MM.BidOnProduct(PrimarysessionID, shop.Id, 12, 3, 22.7);
            UM.Logout(PrimarysessionID);
            MM.ResetDomainData();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MC.Dispose();
            MarketManager.GetInstance().Dispose();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            mockDeliverySystem.Setup(d => d.OrderDelivery(It.IsAny<ShoppingCartPurchase>(), It.IsAny<DeliveryDetails>()))
             .Returns(10000);

            // Example: Set up the mock payment system to return a specific transaction ID
            mockPaymentSystem.Setup(p => p.Pay(It.IsAny<ShoppingCartPurchase>(), It.IsAny<PaymentDetails>()))
                .Returns(10000);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
        }

        [TestMethod]
        public void CheckMemberUpload()
        {
            UM.Login(PrimarysessionID, "regev", "password");
            shop = MM.GetShopInfo(PrimarysessionID, 1);
            Member m = UM.GetMember(PrimarysessionID);
            Assert.IsNotNull(m);
            Assert.IsNotNull(m.Appointments);
            Assert.IsNotNull(m.UserPurchases);
            Assert.IsNotNull(m.Messages);
            Assert.IsTrue(m.Messages.Any(m => m.Comment.Contains("Product Sell Event")));
            Assert.IsFalse(m.IsSystemAdmin);
            ShoppingCartPurchase scp = m.UserPurchases.First();
            Assert.AreEqual(10_000, scp.PaymentId);
            Assert.AreEqual(10_000, scp.DeliveryId);
            Assert.AreEqual(PurchaseStatus.Success,scp.PurchaseStatus);
            Assert.AreEqual(shop.Id, scp.ShopPurchaseObjects.First().ShopId);
            Assert.IsNotNull(m.ShoppingCart.BasketbyShop[shop.Id]);
            Assert.IsTrue(m.ShoppingCart.HasBasketItem(1,11));
            Appointment app = AppointmentRepo.GetInstance().GetById(m.Id, shop.Id);
            Appointment shopApp = shop.Appointments[m.Id];
            Appointment userApp = m.Appointments[shop.Id];
            Assert.IsTrue(shop.Appointments[m.Id]== m.Appointments[shop.Id]);
        }
        [TestMethod]
        public void CheckShopUpload()
        {
            UM.Login(PrimarysessionID, "regev", "password");
            MM.Appoint(PrimarysessionID, "regev2", 1, 3, 2);
            MM.AddProduct(PrimarysessionID, 1, "ben",0, "ben", 1, 3, "None", new List<string>());
            MM.ResetDomainData();
            UM.Login(PrimarysessionID, "regev", "password");
            UM.Login("123234", "regev2", "password");

            Member m = UM.GetMember(PrimarysessionID);
            Shop s = SM.GetShop(1);
            Member m2 = UM.GetMember("123234");
            Assert.IsNotNull(m);
            Assert.IsNotNull(s.Appointments);
            Assert.IsNotNull(s.Purchases);
            Assert.IsNotNull(s.DiscountPolicyManager);
            Assert.IsNotNull(s.PurchasePolicyManager);
            Assert.IsNotNull(s.Rules);
            Appointment sApp = s.Appointments[m.Id];
            Appointment mApp = m.Appointments[s.Id];
            Appointment sApp2 = s.Appointments[m2.Id];
            Appointment mApp2 = m2.Appointments[s.Id];
            Assert.IsTrue(s.Appointments.Count == 2 && sApp == mApp && sApp2 == mApp2);
            Assert.IsTrue(m.Appointments[s.Id].Apointees.Count() == 1);

        }
    }
}