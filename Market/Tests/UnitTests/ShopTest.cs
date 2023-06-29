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
    public class ShopTest
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
        public void AddAppointmentSuccess()
        {
            MarketService.GetInstance().Register("20", "ido", "123");
            MarketService.GetInstance().Login("20", "ido", "123");
            Member user = UserManager.GetInstance().GetMember("20");
            _shop.AddAppointment(user, new Appointment(user, _shop, _owner, Role.Manager, Permission.ManageSupply));
            Assert.IsTrue(_shop.Appointments.ContainsKey(user.Id));
        }

        [TestMethod()]
        public void AddAppointmentFail()
        {
            MarketService.GetInstance().Register("20", "ido", "123");
            MarketService.GetInstance().Login("20", "ido", "123");
            Member user = UserManager.GetInstance().GetMember("20");
            Assert.ThrowsException<Exception>(()=>_shop.AddAppointment(_owner, new Appointment(user, _shop, user, Role.Manager, Permission.ManageSupply)));
        }

        [TestMethod()]
        public void SearchByKeywordsSuccess()
        {
            List<Product> l = _shop.SearchByKeywords("basketball");
            Assert.IsTrue(l.Contains(_p1) && l.Contains(_p2));
        }
        [TestMethod()]
        public void SearchByKeywordsFail()
        {
            List<Product> l = _shop.SearchByKeywords("asgfdsfg");
            Assert.IsTrue(l.Count()==0);
        }

        [TestMethod()]
        public void SearchByNameSuccess()
        {
            List<Product> l = _shop.SearchByName("ball");
            Assert.IsTrue(l.Contains(_p1) && l.Contains(_p2)&&l.Contains(_p3)&&l.Contains(_p4));
        }
        [TestMethod()]
        public void SearchByNameFail()
        {
            List<Product> l = _shop.SearchByName("asfg");
            Assert.IsTrue(l.Count() == 0);
        }

        [TestMethod()]
        public void SearchByCategorySuccess()
        {
            List<Product> l = _shop.SearchByCategory(Category.Pockemon);
            Assert.IsTrue(!l.Contains(_p1) && l.Contains(_p2) && !l.Contains(_p3) && !l.Contains(_p4));
        }
        [TestMethod()]
        public void SearchByCategoryFail()
        {
            List<Product> l = _shop.SearchByCategory(Category.Food);
            Assert.IsTrue(l.Count() == 0);
        }

        [TestMethod()]
        public void AddProductSuccess()
        {
            _shop.AddProduct(_owner.Id, "Ball4", new RegularSell(), "this is a ball4", 4784, Category.None, 21, new SynchronizedCollection<string> { "soccer", "basketball", "round" });
            Assert.IsTrue(_shop.Products.ToList().Find((p) => p.Name == "Ball4") != null);
        }
        public void AddProductFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.AddProduct(_owner.Id+1, "Ball4",new RegularSell(), "this is a ball4", 4784, Category.None, 21, new SynchronizedCollection<string> { "soccer", "basketball", "round" }));
        }

        [TestMethod()]
        public void RemoveProductSuccess()
        {
            _shop.AddProduct(_owner.Id, "Ball4",new RegularSell(), "this is a ball4", 4784, Category.None, 21, new SynchronizedCollection<string> { "soccer", "basketball", "round" });
            Product p1 = _shop.Products.ToList().Find((p) => p.Name == "Ball4");
            _shop.RemoveProduct(_owner.Id, p1.Id);
            Assert.IsTrue(!_shop.Products.Contains(p1));
        }

        [TestMethod()]
        public void RemoveProductFail()
        {
            _shop.AddProduct(_owner.Id, "Ball4",new RegularSell(), "this is a ball4", 4784, Category.None, 21, new SynchronizedCollection<string> { "soccer", "basketball", "round" });
            Product p1 = _shop.Products.ToList().Find((p) => p.Name == "Ball4");
            Assert.ThrowsException<Exception>(() => _shop.RemoveProduct(_owner.Id + 1, p1.Id));
        }
        [TestMethod()]
        public void AddReviewSuccess()
        {
            Member user = new Member(2, "ido", "123");
            _shop.AddReview(user.UserName, user.Id, _p1.Id, "best product ever XD", 4.5);
            Assert.IsTrue(_p1.Reviews.Count() > 0);
        }
        public void AddReviewFail()
        {
            Member user = new Member(2, "ido", "123");
            Assert.ThrowsException<Exception>(() => _shop.AddReview(user.UserName, user.Id, 4896, "best product ever XD", 4.5));
        }

        [TestMethod()]
        public void ShowShopHistorySuccess()
        {
            List<Purchase> l = _shop.ShowShopHistory(_owner.Id);
            Assert.IsTrue(l!=null);
        }

        [TestMethod()]
        public void ShowShopHistoryFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.ShowShopHistory(_owner.Id + 1));
        }

        [TestMethod()]
        public void ShowUserPurchaseHistorySuccess()
        {
            List<Purchase> l = _shop.ShowUserPurchaseHistory(1);
            Assert.IsTrue(l != null);
        }

        [TestMethod()]
        public void GetShopPositionsSuccess()
        {
            List<Appointment> l = _shop.GetShopPositions(_owner.Id);
            Assert.IsTrue(l.Count()==1);
        }

        [TestMethod()]
        public void GetShopPositionsFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.GetShopPositions(_owner.Id+1));
        }

        [TestMethod()]
        public void AddProductToBasketSuccess()
        {
            Basket basket = new Basket(2, _shop);
            _shop.AddProductToBasket(basket, _p1.Id, 25);
            Assert.IsTrue(basket.FindBasketItem(_p1.Id).Quantity==25);
        }

        [TestMethod()]
        public void AddProductToBasketFail()
        {
            Basket basket = new Basket(2, _shop);
            Assert.ThrowsException<Exception>(() => _shop.AddProductToBasket(basket,_p1.Id, 2533));
        }

        [TestMethod()]
        public void CloseShopSuccess()
        {
            _shop.CloseShop(_owner.Id);
            Assert.IsTrue(!_shop.Active);
        }

        [TestMethod()]
        public void CloseShopFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.CloseShop(_owner.Id+1));
            Assert.IsTrue(_shop.Active);
        }

        [TestMethod()]
        public void OpenShopSuccess()
        {
            _shop.Active = false;
            _shop.OpenShop(_owner.Id);
            Assert.IsTrue(_shop.Active);
        }

        [TestMethod()]
        public void OpenShopFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.OpenShop(2));
            Assert.IsTrue(_shop.Active);
        }

        [TestMethod()]
        public void UpdateProductNameSuccess()
        {
            _shop.UpdateProductName(_owner.Id, _p1.Id, "Ben");
            Assert.IsTrue(_p1.Name=="Ben");
        }

        [TestMethod()]
        public void UpdateProductNameFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.UpdateProductName(14, _p1.Id, "Ben"));
        }

        [TestMethod()]
        public void UpdateProductPriceSuccess()
        {
            _shop.UpdateProductPrice(_owner.Id, _p1.Id, 45555);
            Assert.IsTrue(_p1.Price == 45555);
        }

        [TestMethod()]
        public void UpdateProductPriceFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.UpdateProductPrice(14, _p1.Id, 45555));
        }

        [TestMethod()]
        public void UpdateProductQuantitySuccess()
        {
            _shop.UpdateProductQuantity(_owner.Id, _p1.Id, 45555);
            Assert.IsTrue(_p1.Quantity == 45555);
        }

        [TestMethod()]
        public void UpdateProductQuantityFail()
        {
            Assert.ThrowsException<Exception>(() => _shop.UpdateProductQuantity(14, _p1.Id, 45555));
        }
    }
}