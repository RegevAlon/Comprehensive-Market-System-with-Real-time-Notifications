using Microsoft.VisualStudio.TestTools.UnitTesting;
using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer;
using Market.ServiceLayer;
using Moq;

namespace Market.DomainLayer.Tests
{
    [TestClass()]
    public class EventManagerTest
    {
        private Member _member;
        private Guest _guest;

        private Member _manager1;
        private Member _manager2;
        private Member _manager3;
        private Shop _shop;
        private EventManager em;

        [TestInitialize]
        public void Initialize()
        {
            MarketService s = MarketService.GetInstance();
            MarketContext.GetInstance().Dispose();
            MarketContext context = MarketContext.GetInstance();
            UserManager UM = UserManager.GetInstance();
            ShopManager SM = ShopManager.GetInstance();
            s.Register("2", "benalvo", "12345");
            s.Login("2", "benalvo", "12345");
            s.CreateShop("2", "shop1");
            _shop = SM.GetShopByName("shop1");
            em = _shop.EventManager;
            s.Register("3", "tamuzgindes", "54321");
            s.Register("4", "gal", "111111");
            s.Register("5", "gigi", "22222");
            s.Register("6", "regevon", "111111");
            s.Login("3", "tamuzgindes", "54321");
            s.Login("4", "gal", "111111");
            s.Login("5", "gigi", "22222");
            s.Login("6", "regevon", "111111");
            _member = UM.GetMember("3"); ;
            _manager1 = UM.GetMember("4");
            _manager2 = UM.GetMember("5");
            _manager3 = UM.GetMember("6");
        }

        [TestMethod()]
        public void SubscribeSuccess()
        {
            em.Subscribe(_member, new ReportEvent("sadfcsd", "asfsafv"));
            Assert.IsTrue(em.Listeners["Report Event"].Contains(_member));
        }

        [TestMethod()]
        public void SubscribeFail()
        {
            em.Subscribe(_member, new ReportEvent("sadfcsd", "asfsafv"));
            Assert.ThrowsException<Exception>(() => em.Subscribe(_member, new ReportEvent("sadfcsd", "asfsafv")));
        }

        [TestMethod()]
        public void UnsubscribeSuccess()
        {
            em.Subscribe(_member, new ReportEvent("sadfcsd", "asfsafv"));
            em.Unsubscribe(_member, new ReportEvent("sadfcsd", "asfsafv"));
            Assert.IsTrue(!em.Listeners["Report Event"].Contains(_member));
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
        public void UnSubscribeFail()
        {
            Assert.ThrowsException<Exception>(() => em.Unsubscribe(_member, new ReportEvent("sadfcsd", "asfsafv")));
        }

        [TestMethod()]
        public void NotifySubscribers()
        {
            em.Subscribe(_member, new ReportEvent("sadfcsd", "asfsafv"));
            em.NotifySubscribers(new ReportEvent("asf", "asf"));
            Assert.IsTrue(_member.Messages.Count > 0);
        }

        [TestMethod()]
        public void SubscribeToAll()
        {
            em.SubscribeToAll(_member);
            Assert.IsTrue(em.Listeners.Values.ToList().FindAll((users) => users.Contains(_member)).Count() == em.Listeners.Keys.Count);
        }
    }
}