using Microsoft.VisualStudio.TestTools.UnitTesting;
using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.ServiceLayer;
using Market.RepoLayer;
using System.Collections.Concurrent;
using Market.DataLayer;
using Moq;

namespace Market.DomainLayer.Tests
{
    [TestClass()]
    public class SearchTest
    {
        private Member _owner;
        private Shop _shop1;
        private Product _p11;
        private Product _p21;
        private Product _p31;
        private Product _p41;
        private Shop _shop2;
        private Product _p12;
        private Product _p22;
        private Product _p32;
        private Product _p42;
        private Search search;
        List<Shop> _shops;

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
            _shop1 = SM.GetShopByName("shop1");
            s.AddProduct("2", _shop1.Id, "Ball",0, "this is a ball", 52.6, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop1.Id, "Ball1",0, "this is a ball1",0.8, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop1.Id, "Ball2",0, "this is a ball2", 45, 80, Category.None.ToString(), new List<string>());
            s.AddProduct("2", _shop1.Id, "Ball3",0, "this is a ball3",250, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p11 = _shop1.Products.ToList().Find((p) => p.Id == 11);
            _p21 = _shop1.Products.ToList().Find((p) => p.Id == 12);
            _p31 = _shop1.Products.ToList().Find((p) => p.Id == 13);
            _p41 = _shop1.Products.ToList().Find((p) => p.Id == 14);
            s.CreateShop("2", "shop2");
            _shop2 = SM.GetShopByName("shop2");
            s.AddProduct("2", _shop2.Id, "Ball",0, "this is a ball", 49, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop2.Id, "Ball1",0, "this is a ball1", 64, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop2.Id, "Ball2",0, "this is a ball2", 74, 80, Category.None.ToString(), new List<string>());
            s.AddProduct("2", _shop2.Id, "Ball3",0, "this is a ball3", 1, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p12 = _shop2.Products.ToList().Find((p) => p.Id == 21);
            _p22 = _shop2.Products.ToList().Find((p) => p.Id == 22);
            _p32 = _shop2.Products.ToList().Find((p) => p.Id == 23);
            _p42 = _shop2.Products.ToList().Find((p) => p.Id == 24);
            s.Register("3", "tamuzgindes", "54321");
            s.Register("4", "gal", "111111");
            s.Register("5", "gigi", "22222");
            s.Register("6", "regevon", "111111");
            s.Login("3", "tamuzgindes", "54321");
            s.Login("4", "gal", "111111");
            s.Login("5", "gigi", "22222");
            s.Login("6", "regevon", "111111");
            _shops = new List<Shop>() { _shop1, _shop2 };
            search = new Search();

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
        public void ApplySearchByKeyword()
        {
            Assert.IsTrue(search.ApplySearch("soccer", SearchType.Keywords, new List<FilterSearchType> { new PriceRangeFilter(2, 50) }, _shops).Count() == 1);
        }
        [TestMethod()]
        public void ApplySearchByCategory()
        {
            HashSet<Product> l = search.ApplySearch("Pockemon", SearchType.Category, new List<FilterSearchType>(),_shops);
            Assert.IsTrue(l.Count()==2);
            Assert.IsTrue(l.Contains(_p21)&&l.Contains(_p22));
        }
        [TestMethod()]
        public void ApplySearchByName()
        {
            HashSet<Product> l = search.ApplySearch("Ball", SearchType.Name, new List<FilterSearchType> { new PriceRangeFilter(0, 50) },_shops);
            Assert.IsTrue(l.Count() == 4);
            Assert.IsTrue(l.Contains(_p21) && l.Contains(_p31)&& l.Contains(_p12)&& l.Contains(_p42));

        }
    }
}