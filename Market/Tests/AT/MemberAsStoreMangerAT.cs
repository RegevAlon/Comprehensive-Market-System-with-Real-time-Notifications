using Market.AT;
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
    public enum ATPermission
    {
        ManageSupply = 1,
        Appoint = 2,
        Policy = 4,
        UserApplications = 8,
        ShopPurchaseHistory = 16,
        ShopAppointmentsInfo = 32,
        OpenCloseShop = 64,
        All = ManageSupply | Appoint | Policy | UserApplications | ShopPurchaseHistory | ShopAppointmentsInfo | OpenCloseShop
    }

    [TestClass]
    public class MemberAsStoreMangerAT
    {
        Proxy proxy;
        string sessionID_manager;
        string sessionID_owner;
        string sessionID_member;

        string username_manager;
        string username_owner;
        string username_member;

        string password_manager;
        string password_owner;
        string password_memeber;

        int shopID;

        public MemberAsStoreMangerAT()
        {
            proxy = new Proxy();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
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
            shopID = 1;
            sessionID_manager = proxy.getSessionId();
            sessionID_owner = proxy.getSessionId();
            sessionID_member = proxy.getSessionId();

            username_manager = "manager";
            username_owner = "owner";
            username_member = "member";

            password_manager = "manager123";
            password_owner = "owner123";
            password_memeber = "memeber123";

            proxy.EnterAsGuest(sessionID_owner);
            proxy.Register(sessionID_owner, username_owner, password_owner);
            proxy.Login(sessionID_owner,username_owner, password_owner);

            proxy.EnterAsGuest(sessionID_manager);
            proxy.Register(sessionID_manager, username_manager, password_manager);
            proxy.Login(sessionID_manager,username_manager, password_manager);

            proxy.EnterAsGuest(sessionID_member);
            proxy.Register(sessionID_member, username_member, password_memeber);
            proxy.Login(sessionID_member,username_member, password_memeber);

            proxy.createShop(sessionID_owner, "Regev's Shop");
            proxy.Appoint(sessionID_owner, username_manager, shopID, 3, ((int)ATPermission.All));
            proxy.Appoint(sessionID_owner, username_member, shopID, 3, ((int)ATPermission.All));
        }

        [TestCleanup]
        public void Cleanup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void RemoveAppointment_GoodCase()
        {
            Assert.IsTrue(proxy.RemoveAppoint(sessionID_owner, username_manager, shopID));
            Assert.IsTrue(proxy.RemoveAppoint(sessionID_owner, username_member, shopID));
            //Afer the appointments were removed reAppoint will be excuted succsessfully
            Assert.IsTrue(proxy.Appoint(sessionID_owner, username_member, shopID, 3, ((int)ATPermission.All)));
            Assert.IsFalse(proxy.Appoint(sessionID_owner, username_member, shopID, 3, ((int)ATPermission.All)));


        }

        [TestMethod]
        public void RemoveAppointment_BadCase()
        {
            Assert.IsFalse(proxy.RemoveAppoint(sessionID_member, username_manager, shopID));
            Assert.IsFalse(proxy.RemoveAppoint(sessionID_manager, username_member, shopID));
            Assert.IsFalse(proxy.RemoveAppoint(sessionID_member, username_owner, shopID));
            Assert.IsFalse(proxy.RemoveAppoint(sessionID_manager, username_owner, shopID));

            Assert.IsTrue(proxy.RemoveAppoint(sessionID_owner, username_manager, shopID));
            Assert.IsFalse(proxy.RemoveAppoint(sessionID_manager, username_owner, shopID));
        }
    }

}
