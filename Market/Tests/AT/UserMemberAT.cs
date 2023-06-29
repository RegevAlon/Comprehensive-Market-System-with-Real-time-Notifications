using Azure;
using Market.DataLayer;
using Market.DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AT
{
    [TestClass]
    public class UserMemberAT
    {
        Proxy proxy;
        string sessionID;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;

        public UserMemberAT()
        {
            proxy = new Proxy();
        }

        [TestInitialize]
        public void Setup()
        {
            MarketContext.GetInstance().Dispose();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            BadPermission = 1;
            sessionID = proxy.getSessionId();
            proxy.EnterAsGuest(sessionID);
            proxy.Register(sessionID, "user", "password");
        }

        [TestCleanup]
        public void Cleanup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void Logout_GoodCase()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.Logout(sessionID));
        }

        [TestMethod]
        public void Logout_BadCase()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            sessionID = (int.Parse(sessionID)+1).ToString();
            Assert.IsFalse(proxy.Logout(sessionID));
        }
        [TestMethod]
        public void CreateShop_GoodCase()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            Assert.IsFalse(proxy.Appoint(sessionID, "usertoappoint", shopID, FounderRole, GoodPermission));
        }

        [TestMethod]
        public void CreateShop_BadCase()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            Assert.IsFalse(proxy.createShop(appointedseesionID, "Regev's Shop - the bex"));
        }

    }

}
