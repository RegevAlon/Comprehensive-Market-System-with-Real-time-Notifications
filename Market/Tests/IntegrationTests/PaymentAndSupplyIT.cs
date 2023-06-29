using Market.AT;
using Market.DomainLayer;
using Market.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.AT;
using Market.DataLayer;
using Moq;

namespace Market.IntegrationTests
{
    [TestClass]
    public class PaymentAndSupplyIT
    {
        UserManager UM;
        ShopManager SM;
        PaymentSystemMock PaymentSystem;
        ProductSupplierMock DeliverySystem;
        string PrimarysessionID;
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

        public PaymentAndSupplyIT()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            PaymentSystem = new PaymentSystemMock();
            DeliverySystem = new ProductSupplierMock();

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
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            BadPermission = 1;
            keywords = new SynchronizedCollection<string>();
            keywords.Add("green apple");
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<DomainLayer.IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            MarketManager.GetInstance().Dispose();
        }

        [TestMethod]
        public void DeliverQuantity_GoodCase()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int regevQuantity = quantity - 20;
            UM.AddToCart(PrimarysessionID, dummyShop, productID, regevQuantity);
            string secondBuyerSessionID = "2";
            UM.Logout(PrimarysessionID);
            UM.Register("ben", "password");
            UM.Login(secondBuyerSessionID, "ben", "password");
            Member secondBuyer = UM.GetMember(secondBuyerSessionID);
            int benQuantity = quantity - 20;
            UM.AddToCart(secondBuyerSessionID, dummyShop, productID, benQuantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = regevQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            if (!secondBuyer.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            totalprice = benQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            int desiredQuantity = regevQuantity + benQuantity;
            DeliverySystem.SetupCheckProductAvailability(myprod, myprod.Name, desiredQuantity - quantity, true);
            DeliverySystem.VerifyOrderProducts("apple", desiredQuantity);
            UM.Purchase(secondBuyerSessionID, shopID);
            UM.Login(PrimarysessionID, "regev", "password");
            UM.Purchase(PrimarysessionID, shopID);
            Assert.IsTrue(myprod.Quantity == 0);

        }
        [TestMethod]
        public void DeliverQuantity_BadCase()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            int regevQuantity = quantity - 20;
            UM.AddToCart(PrimarysessionID, dummyShop, productID, regevQuantity);
            string secondBuyerSessionID = "3";
            UM.Logout(PrimarysessionID);
            UM.Register("ben", "password");
            UM.Login(secondBuyerSessionID, "ben", "password");
            Member secondBuyer = UM.GetMember(secondBuyerSessionID);
            int benQuantity = quantity - 20;
            UM.AddToCart(secondBuyerSessionID, dummyShop, productID, benQuantity);
            Basket basket;
            if (!member.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = regevQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            if (!secondBuyer.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            totalprice = benQuantity * (myprod.Price);
            Assert.IsTrue(basket.GetBasketPrice() == totalprice);
            int desiredQuantity = regevQuantity + benQuantity;
            UM.Login(PrimarysessionID, "regev", "password");
            UM.Purchase(PrimarysessionID, shopID);
            try
            {
                UM.Purchase(secondBuyerSessionID, shopID);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Product {myprod.Name}: In supply: {quantity - regevQuantity}, You required: {benQuantity}");
            }

            Assert.IsTrue(myprod.Quantity == quantity - regevQuantity );

        }
    }
}
