using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.AT;
using Market.DataLayer;
using Market.DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Market.AT
{
    [TestClass]

    public class SystemAT
    {
        Proxy proxy;
        string sessionID;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;
        public SystemAT()
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
        }

        [TestMethod]
        public void NotificationOn_loggedIn()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            List<string> userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 0);
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, OwnerRole, GoodPermission));
            userMessages = proxy.GetMessages(sessionID);
            Assert.IsTrue(userMessages.Count > 0);

        }

        [TestMethod]
        public void NotificationOn_loggedOut()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            List<string> userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 0);
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, OwnerRole, GoodPermission));
            userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 1);
            Assert.IsTrue(proxy.Logout(sessionID));
            string appointedOfAppointedSessionId = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedOfAppointedSessionId));
            Assert.IsTrue(proxy.Register(appointedOfAppointedSessionId, "secondUsertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedOfAppointedSessionId, "secondUsertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(appointedseesionID, "secondUsertoappoint", shopID, OwnerRole, GoodPermission));
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 2);
            userMessages = proxy.GetMessages(appointedseesionID);
            Assert.AreEqual(userMessages.Count, 2);
        }

        [TestMethod]
        public void NotificationOff_loggedIn()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.Notification_off(sessionID));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            List<string> userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 0);
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, OwnerRole, GoodPermission));
            userMessages = proxy.GetMessages(sessionID);
            Assert.IsTrue(userMessages.Count > 0);

        }

        [TestMethod]
        public void NotificationOff_loggedOut()
        {
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            Assert.IsTrue(proxy.Notification_off(sessionID));
            Assert.IsTrue(proxy.createShop(sessionID, "Regev's Shop - the bex"));
            int shopID = 1;
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            List<string> userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 0);
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, OwnerRole, GoodPermission));
            userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 1);
            Assert.IsTrue(proxy.Logout(sessionID));
            string appointedOfAppointedSessionId = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedOfAppointedSessionId));
            Assert.IsTrue(proxy.Register(appointedOfAppointedSessionId, "secondUsertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedOfAppointedSessionId, "secondUsertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(appointedseesionID, "secondUsertoappoint", shopID, OwnerRole, GoodPermission));
            Assert.IsTrue(proxy.Login(sessionID, "user", "password"));
            userMessages = proxy.GetMessages(sessionID);
            Assert.AreEqual(userMessages.Count, 2);
            userMessages = proxy.GetMessages(appointedseesionID);
            Assert.AreEqual(userMessages.Count, 2);
        }

    }
}
