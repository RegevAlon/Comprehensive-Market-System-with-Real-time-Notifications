using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Market.DataLayer;
using Market.ServiceLayer;
using Market.AT;
using Moq;

namespace Market.DomainLayer.Tests
{
    [TestClass()]
    public class BasketTest
    {
        private Member _owner;
        private Basket _basket;
        private Shop _shop;
        private Product _p1;
        private Product _p2;
        private Product _p3;
        private Product _p4;

        [TestInitialize]
        public void Initialize()
        {
            MarketService s = MarketService.GetInstance();
            MarketContext.GetInstance().Dispose();
            MarketContext context = MarketContext.GetInstance();
            UserManager UM = UserManager.GetInstance();
            ShopManager SM = ShopManager.GetInstance();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            s.Register("2", "benalvo", "12345");
            s.Login("2", "benalvo", "12345");
            s.CreateShop("2", "shop1");
            _owner = UM.GetMember("2");
            _shop = SM.GetShopByName("shop1");
            s.AddProduct("2", _shop.Id, "Ball",0, "this is a ball", 52.6, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop.Id, "Ball1",0, "this is a ball1", 52.6, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop.Id, "Ball2",0, "this is a ball2", 52.6, 80, Category.None.ToString(), new List<string>());
            s.AddProduct("2", _shop.Id, "Ball3",0, "this is a ball3", 52.6, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p1 = _shop.Products.ToList().Find((p) => p.Id == 11);
            _p2 = _shop.Products.ToList().Find((p) => p.Id == 12);
            _p3 = _shop.Products.ToList().Find((p) => p.Id == 13);
            _p4 = _shop.Products.ToList().Find((p) => p.Id == 14);
            s.AddToCart("2", _shop.Id, _p4.Id, 1);
            _basket = _owner.ShoppingCart.BasketbyShop[_shop.Id];
        }

        [TestMethod()]
        public void AddProduct()
        {
            _basket.AddProduct(_p1, 20);
            Assert.IsTrue(_basket.FindBasketItem(_p1.Id).Quantity == 20);
        }
        [TestCleanup]
        public void Cleanup()
        {
            MarketService.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
        }
        [TestMethod()]
        public void AddProductRequestSuccess()
        {
            _basket.AddProductRequest(_p1.Id, 20);
            Assert.IsTrue(_basket.FindBasketItem(_p1.Id).Quantity == 20);
        }

        [TestMethod()]
        public void AddProductRequestFail()
        {
            Assert.ThrowsException<Exception>(() => _basket.AddProductRequest(_p1.Id, 20000));
        }

        [TestMethod()]
        public void RemoveProductSuccess()
        {
            _basket.AddProductRequest(_p1.Id, 20);
            _basket.AddProductRequest(_p2.Id, 1);
            _basket.RemoveProduct(_p2.Id);
            Assert.IsTrue(!_basket.HasProduct(_p2));
        }

        [TestMethod()]
        public void RemoveProductFail()
        {
            Assert.ThrowsException<Exception>(() => _basket.RemoveProduct(_p1.Id));
        }

        [TestMethod()]
        public void GetBasketPrice()
        {
            _basket.RemoveProduct(_p4.Id);
            _basket.AddProductRequest(_p1.Id, 20);
            _basket.AddProductRequest(_p2.Id, 1);
            Assert.IsTrue(_basket.GetBasketPrice()==20*_p1.Price + _p2.Price);
        }

        [TestMethod()]
        public void PurchaseSuccess()
        {
            _basket.AddProductRequest(_p1.Id, 20);
            _basket.AddProductRequest(_p2.Id, 1);
            Purchase p = _basket.Purchase(1);
            Assert.IsTrue(p != null);
        }

        public void PurchaseFail()
        {
            _basket.AddProductRequest(_p1.Id, 20);
            _basket.AddProductRequest(_p2.Id, 1);
            _shop.UpdateProductQuantity(1, _p1.Id, 19);
            Assert.ThrowsException<Exception>(() => _basket.Purchase(1));
        }
    }
}