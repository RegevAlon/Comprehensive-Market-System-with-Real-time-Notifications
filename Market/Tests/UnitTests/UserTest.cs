using Microsoft.VisualStudio.TestTools.UnitTesting;
using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Market.DataLayer;
using Market.ServiceLayer;
using Moq;

namespace Market.DomainLayer.Tests
{
    [TestClass()]
    public class UserTest
    {
        User _guest;
        private Member _owner;
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
            s.AddProduct("2", _shop.Id, "Ball3", 0,"this is a ball3", 52.6, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p1 = _shop.Products.ToList().Find((p) => p.Id == 11);
            _p2 = _shop.Products.ToList().Find((p) => p.Id == 12);
            _p3 = _shop.Products.ToList().Find((p) => p.Id == 13);
            _p4 = _shop.Products.ToList().Find((p) => p.Id == 14);
            s.Register("3", "tamuzgindes", "54321");
            s.Register("4", "gal", "111111");
            s.Register("5", "gigi", "22222");
            s.Register("6", "regevon", "111111");
            s.Login("3", "tamuzgindes", "54321");
            s.Login("4", "gal", "111111");
            s.Login("5", "gigi", "22222");
            s.Login("6", "regevon", "111111");
            s.EnterAsGuest("7");
            _guest = UM.GetUser("7");
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
        public void PurchaseBasketSuccess()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            ShoppingCartPurchase p = _guest.Purchase(_shop.Id);
            Assert.IsTrue(p!=null);
            Assert.IsTrue(p.ShopPurchaseObjects[0].Basket.BasketItems[0].Product.Id==_p1.Id);
            Assert.IsTrue(p.ShopPurchaseObjects[0].Basket.BasketItems[0].Quantity == 20);
        }

        [TestMethod()]
        public void PurchaseBasketFail()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            _shop.UpdateProductQuantity(_owner.Id, _p1.Id, 19);
            Assert.ThrowsException<ArgumentException>(() => _guest.Purchase(_shop.Id));
        }

        [TestMethod()]
        public void AddToCartSuccess()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            Assert.IsTrue(_guest.ShoppingCart.BasketbyShop[_shop.Id] != null);
            Assert.IsTrue(_guest.ShoppingCart.BasketbyShop[_shop.Id].HasProduct(_p1));
            Assert.IsTrue(_guest.ShoppingCart.BasketbyShop[_shop.Id].BasketItems[0].Quantity==20);

        }

        [TestMethod()]
        public void AddToCartFail()
        {
            Assert.ThrowsException<Exception>(() => _guest.AddToCart(_shop,_p1.Id,20000));
        }

        [TestMethod()]
        public void RemoveFromCartSuccess()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            _guest.RemoveFromCart(_shop.Id, _p1.Id);
            Assert.IsTrue(!_guest.ShoppingCart.BasketbyShop.ContainsKey(_shop.Id));
        }

        [TestMethod()]
        public void RemoveFromCartFail()
        {
            Assert.ThrowsException<Exception>(() => _guest.RemoveFromCart(_shop.Id, _p1.Id));
        }

        [TestMethod()]
        public void RemoveBasketFromCartSuccess()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            _guest.RemoveBasketFromCart(_shop.Id);
            Assert.IsTrue(!_guest.ShoppingCart.BasketbyShop.ContainsKey(_shop.Id));
        }

        [TestMethod()]
        public void PurchaseShoppingCart()
        {
            _guest.AddToCart(_shop, _p1.Id, 20);
            ShoppingCartPurchase l =  _guest.PurchaseShoppingCart();
            Assert.IsTrue(l.ShopPurchaseObjects.Count == 1);
            Assert.IsTrue(_guest.ShoppingCart.BasketbyShop[_shop.Id].HasProduct(_p1));
            Assert.IsTrue(_guest.ShoppingCart.BasketbyShop[_shop.Id].BasketItems[0].Quantity == 20);
        }
    }
}