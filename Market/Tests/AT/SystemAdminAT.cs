using Market.AT;
using Market.DomainLayer;
using Market.RepoLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.ServiceLayer;
using Market.DataLayer;
using Moq;

namespace Market.AT
{
    [TestClass]
    public class SystemAdminAT
    {
        Proxy proxy;
        MarketManager MM = MarketManager.GetInstance();
        Member MasterAdmin = MemberRepo.GetInstance().GetByUserName("MasterAdmin");
        private const string ADMIN_SESSION_ID = "1";
        private const string ADMIN_USER_NAME = "admin";
        private const string ADMIN_PASSWORD = "admin";
        string _founder_sessionID = "2";
        string _founder_username = "gal";
        string _founder_password = "gal123";

        string _member_sessionID = "3";
        string _member_username = "gigi";
        string _member_password = "gigi321123";

        string _shopName = "Tamuz's shop";


        public SystemAdminAT()
        {
            proxy = new Proxy();
        }

        [TestInitialize]
        public void Setup()
        {
            MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            proxy.EnterAsGuest(ADMIN_SESSION_ID);
            proxy.Register(ADMIN_SESSION_ID, ADMIN_USER_NAME, ADMIN_PASSWORD);
            proxy.AddSystemAdmin(ADMIN_USER_NAME);
            proxy.Login(ADMIN_SESSION_ID, ADMIN_USER_NAME, ADMIN_PASSWORD);

            proxy.EnterAsGuest(_founder_sessionID);
            proxy.Register(_founder_sessionID, _founder_username, _founder_password);
            proxy.Login(_founder_sessionID, _founder_username, _founder_password);
            proxy.createShop(_founder_sessionID, _shopName);


            proxy.EnterAsGuest(_member_sessionID);
            proxy.Register(_member_sessionID, _member_username, _member_password);
            proxy.Login(_member_sessionID, _member_username, _member_password);
        }

        [TestCleanup]
        public void Cleanup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();

        }

        [TestMethod]
        public void GetAllMembersInfoSuccess()
        {
            List<string> members = proxy.GetAllMembers(ADMIN_SESSION_ID);
            Assert.IsNotNull(members);
            List<string> names = new List<string>() { MasterAdmin.UserName, ADMIN_USER_NAME, _founder_username, _member_username };
            Assert.AreEqual(names.Count, members.Count);
            foreach (string memberName in members)
            {
                Assert.IsTrue(names.Contains(memberName));
            }
        }

        [TestMethod]
        public void GetAllMembersInfoFail()
        {
            Assert.IsNull(proxy.GetAllMembers(_founder_sessionID));
        }


        [TestMethod]
        public void GetActiveMembersInfoSuccess()
        {
            proxy.Logout(_founder_sessionID);
            List<string> members = proxy.GetActiveMembers(ADMIN_SESSION_ID);
            Assert.IsNotNull(members);
            List<string> names = new List<string>() { ADMIN_USER_NAME, _member_username };
            Assert.AreEqual(names.Count, members.Count);
            foreach (string memberName in members)
            {
                Assert.IsTrue(names.Contains(memberName));
            }
        }

        [TestMethod]
        public void GetActiveMembersInfoFail()
        {
            Assert.IsNull(proxy.GetAllMembers(_member_sessionID));
        }

        [TestMethod]
        public void CancelMembershipSuccess()
        {
            proxy.Logout(_member_sessionID);
            Assert.IsTrue(proxy.CancelMembership(ADMIN_SESSION_ID, _member_username));
            Assert.IsFalse(proxy.Login(_member_sessionID, _member_username, _member_password));
        }

        [TestMethod]
        public void CancelMembershipFail()
        {
            Assert.IsFalse(proxy.CancelMembership(ADMIN_SESSION_ID, _founder_username));
            Assert.IsFalse(proxy.CancelMembership(_founder_sessionID, _member_username));
            Assert.IsFalse(proxy.CancelMembership(ADMIN_SESSION_ID, ADMIN_USER_NAME));
        }
    }
}
