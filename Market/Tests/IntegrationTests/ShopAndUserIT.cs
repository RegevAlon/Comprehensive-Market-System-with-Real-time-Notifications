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
    public class ShopAndUserIT
    {
        UserManager UM;
        ShopManager SM;
        MarketManager MM;
        PaymentSystemMock PaymentSystem;
        ProductSupplierMock DeliverySystem;
        string PrimarysessionID;
        string sessionID_Owner;
        int shopID;
        int secondShopID;
        int productID;
        int quantity;
        int price;
        SynchronizedCollection<String> keywords;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;

        public ShopAndUserIT()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();
            PaymentSystem = new PaymentSystemMock();
            DeliverySystem = new ProductSupplierMock();

        }

        [TestInitialize]
        public void Setup()
        {
            MarketContext.GetInstance().Dispose();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<DomainLayer.IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            PrimarysessionID = "1";
            shopID = 1;
            secondShopID = 2;
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();

        }

        [TestMethod]
        public void UniqueShop()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            Shop dummyShop = SM.GetShop(shopID);
            string secondBuyerSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(secondBuyerSessionID, "ben", "password");
            Member secondOpener = UM.GetMember(secondBuyerSessionID);
            try
            {
                MM.CreateShop(secondBuyerSessionID, "Regev Yohananof");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Shop name already exist.");

            }
            Assert.IsTrue(SM.GetShopByName("Regev Yohananof") == dummyShop);
        }
        [TestMethod]
        public void AddProductstoClosedShop()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "4";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            SM.CloseShop(member.Id, shopID);
            try
            {
                UM.AddToCart(BuyerSessionID, dummyShop, productID, quantity);
            }
            catch (Exception e)
            {
            }
            Basket basket;
            if (!buyer.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(myprod.Quantity == quantity);
        }
        [TestMethod]
        public void PurchaseCartOfClosedShop()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            UM.AddToCart(BuyerSessionID, dummyShop, productID, quantity);
            SM.CloseShop(member.Id, shopID);
            Basket basket;
            Assert.IsTrue(myprod.Quantity == quantity);
            try
            {
                UM.Purchase(BuyerSessionID, shopID);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Shop: {dummyShop.Name} is not active anymore");

            }
        }
        [TestMethod]
        public void AddBasketItemAndChangeQunatity()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, 20, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            UM.AddToCart(BuyerSessionID, dummyShop, productID, 20);
            Assert.IsTrue(buyer.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(productID).Quantity == 20);
            UM.UpdateBasketItemQuantity(buyer,dummyShop.Id,myprod.Id,15);
            Assert.IsTrue(buyer.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(productID).Quantity == 15);
        }

        [TestMethod]
        public void AddBasketItemAndChangeQunatityToZero()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, 20, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            UM.AddToCart(BuyerSessionID, dummyShop, productID, 20);
            Assert.IsTrue(buyer.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(productID).Quantity == 20);
            UM.UpdateBasketItemQuantity(buyer, dummyShop.Id, myprod.Id, 0);
            Assert.IsTrue(!buyer.ShoppingCart.BasketbyShop.ContainsKey(dummyShop.Id));
        }

        [TestMethod]
        public void AddBasketItemAndChangeQunatityToNegative()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, 20, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            UM.AddToCart(BuyerSessionID, dummyShop, productID, 20);
            Assert.IsTrue(buyer.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(productID).Quantity == 20);
            Assert.ThrowsException<Exception>(()=> UM.UpdateBasketItemQuantity(buyer, dummyShop.Id, myprod.Id, -1));
        }

        [TestMethod]
        public void AddBasketItemAndChangeQunatityToBiggerThanStoreSupply()
        {
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");
            SM.AddProduct(member.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, 20, "Food", keywords);
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            string BuyerSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(BuyerSessionID, "ben", "password");
            Member buyer = UM.GetMember(BuyerSessionID);
            UM.AddToCart(BuyerSessionID, dummyShop, productID, 20);
            Assert.IsTrue(buyer.ShoppingCart.BasketbyShop[dummyShop.Id].FindBasketItem(productID).Quantity == 20);
            Assert.ThrowsException<Exception>(() => UM.UpdateBasketItemQuantity(buyer, dummyShop.Id, myprod.Id, 21));
        }

        [TestMethod]
        public void PurchaseShoppingCartValidAndApplyDiscount()
        {
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<DomainLayer.IPaymentSystem>();
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
            MM.AddProduct(PrimarysessionID, shopID, "apple", 0, "green apple from the field", 15, 200, "Food", keywords.ToList());
            Shop dummyShop = SM.GetShop(shopID);
            Product myprod = dummyShop.Products.ElementAt(0);
            MM.AddQuantityRule(PrimarysessionID, dummyShop.Id, "Food", 1, 3);
            MM.AddDiscountPolicy(PrimarysessionID, dummyShop.Id, DateTime.Now.AddDays(5).ToString(), "Food", 11, 0.5);
            MM.AddToCart(PrimarysessionID, dummyShop.Id, productID, 1);
            MM.PurchaseShoppingCart(PrimarysessionID, new PaymentDetails(), new DeliveryDetails());
            Assert.IsTrue(myprod.Quantity == 199);
            Member member = UM.GetMember(PrimarysessionID);
            ShoppingCartPurchase scp = member.UserPurchases.ElementAt(0);
            Assert.IsNotNull(scp);
            Assert.AreEqual(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Product.Id , myprod.Id);
            Assert.AreEqual(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).Quantity, 1);
            Assert.AreEqual(scp.ShopPurchaseObjects.ElementAt(0).Basket.BasketItems.ElementAt(0).PriceAfterDiscount, 7.5);
            Assert.AreEqual(scp.ShopPurchaseObjects.ElementAt(0).Basket.TotalPrice, 7.5);
            Assert.AreEqual(scp.PurchaseStatus, PurchaseStatus.Success);
        }



    }
}
