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
    public class ProductTest
    {
        private Member _owner;
        private Shop _shop;
        private Product _p1;
        private Product _p2;
        private Product _p3;
        private Product _p4;

        [TestInitialize]
        public void Initialize()
        {
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            MarketService s = MarketService.GetInstance();
            MarketContext.GetInstance().Dispose();
            MarketContext context = MarketContext.GetInstance();
            UserManager UM = UserManager.GetInstance();
            ShopManager SM = ShopManager.GetInstance();
            s.Register("2", "benalvo", "12345");
            s.Login("2", "benalvo", "12345");
            s.CreateShop("2", "shop1");
            _owner = UM.GetMember("2");
            _shop = SM.GetShopByName("shop1");
            s.AddProduct("2", _shop.Id, "Ball",0, "this is a ball", 52.6, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop.Id, "Ball1",0, "this is a ball1", 52.6, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop.Id, "Ball2", 0,"this is a ball2", 52.6, 80, Category.None.ToString(), new List<string>());
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
        public void ContainKeyword()
        {
            Assert.IsTrue(_p1.ContainKeyword("soccer"));
            Assert.IsTrue(_p1.ContainKeyword("SOCCER"));
        }

        [TestMethod()]
        public void AddReviewSuccess()
        {
            _p1.AddReview(_owner.Id, _owner.UserName, "my first review", 4.5);
            Assert.IsTrue(_p1.Reviews.Count>0);
            Assert.IsTrue(_p1.Reviews[0].Comment == "my first review");
            Assert.IsTrue(_p1.Reviews[0].User == _owner.UserName);
        }

        [TestMethod()]
        public void AddReviewFail()
        {
            _p1.AddReview(_owner.Id, _owner.UserName, "my first review", 4.5);
            Assert.ThrowsException<Exception>(() => _p1.AddReview(_owner.Id, _owner.UserName, "my second review", 4.5));

        }

        [TestMethod()]
        public void GetRate()
        {
            _p1.AddReview(_owner.Id, _owner.UserName, "my first review", 5);
            _p1.AddReview(3, "user1", "my first review", 3);
            _p1.AddReview(4, "user2", "my first review", 5);
            _p1.AddReview(5, "user3", "my first review", 3);
            Assert.IsTrue(_p1.GetRate() == 4);
        }
    }
}