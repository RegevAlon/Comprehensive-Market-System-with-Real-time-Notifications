using Microsoft.VisualStudio.TestTools.UnitTesting;
using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Market.RepoLayer;
using Market.DataLayer;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Market.IntegrationTests
{
    [TestClass()]
    public class SystemAdminTest
    {
        private const string ADMIN_SESSION_ID = "1";
        private const string ADMIN_USER_NAME = "admin";
        private const string ADMIN_PASSWORD = "admin";

        private Member _masterAdmin;
        private Member _admin;
        private Member _owner;
        private Member _member;
        private Member _manager1;
        private Member _manager2;
        private Member _manager3;
        private Shop _shop;


        [TestInitialize]
        public void Initialize()
        {
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
            _masterAdmin = MemberRepo.GetInstance().GetByUserName("MasterAdmin");
            _admin = new Member(2, "admin", "admin");
            UserManager.GetInstance().Register("admin", "admin");
            MarketManager.GetInstance().AppointSystemAdmin(_admin.UserName);
            UserManager.GetInstance().Login(ADMIN_SESSION_ID, ADMIN_USER_NAME, ADMIN_PASSWORD);
            _owner = new Member(3, "benalvo", "12345");
            _member = new Member(4, "tamuzgindes", "54321");
            _manager1 = new Member(5, "gal", "111111");
            _manager2 = new Member(6, "gigi", "22222");
            _manager3 = new Member(7, "regevon", "111111");
            Member[] members = { _owner, _member, _manager1, _manager2, _manager3};

            foreach (Member member in members)
            {
                MarketManager.GetInstance().Register(member.Id.ToString(), member.UserName, member.Password);
                MarketManager.GetInstance().Login(member.Id.ToString(), member.UserName, member.Password);

            }
            MarketManager.GetInstance().CreateShop(_owner.Id.ToString(), "shop1");
            _shop = ShopRepo.GetInstance().GetByName("shop1");
        }
        [TestCleanup]
        public void CleanUp()
        {
            MarketManager.GetInstance().Dispose();
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
        public void Dispose()
        {
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
        }
        [TestMethod()]
        public void GetAllMembersInfoSuccess()
        {
            List<string> membersNames = new List<string>() { _masterAdmin.UserName, _admin.UserName, _owner.UserName, _member.UserName, _manager1.UserName, _manager2.UserName , _manager3.UserName};
            List<Member> outMembers = MarketManager.GetInstance().GetAllMembers(ADMIN_SESSION_ID);
            Assert.AreEqual(membersNames.Count, outMembers.Count);
            for (int i = 0; i < outMembers.Count; i++)
            {
                Assert.IsTrue(membersNames.Contains(outMembers[i].UserName));
            }
        }
        [TestMethod()]
        public void GetAllMembersInfoFail()
        {
            Assert.ThrowsException<ArgumentException>(() => MarketManager.GetInstance().GetAllMembers(ADMIN_SESSION_ID + 10));
        }
        [TestMethod()]
        public void GetActiveMembersInfoSuccess()
        {
            List<string> activeMembersNames = new List<string>(){ _admin.UserName, _owner.UserName, _member.UserName, _manager1.UserName, _manager2.UserName };
            MarketManager.GetInstance().Logout(_manager3.Id.ToString());
            List<Member> outMembers = MarketManager.GetInstance().GetActiveMembers(ADMIN_SESSION_ID);
            Assert.AreEqual(activeMembersNames.Count, outMembers.Count);
            for (int i = 0; i < outMembers.Count; i++)
            {
                Assert.IsTrue(activeMembersNames.Contains(outMembers[i].UserName));
            }
        }

        [TestMethod()]
        public void CancelMembershipFail()
        {
            _owner = MemberRepo.GetInstance().GetById(_owner.Id);
            _owner.Appoint(_member, _shop, Role.Owner, Permission.Policy);
            int count = _member.Appointments.Count;
            MemberRepo.GetInstance().Update(_member);
            int countCheck = MemberRepo.GetInstance().GetById(_member.Id).Appointments.Count;
            Assert.AreEqual(1, count);

            Assert.ThrowsException<ArgumentException>(() => MarketManager.GetInstance().CancelMembership(ADMIN_SESSION_ID, _member.UserName));
            Assert.ThrowsException<ArgumentException>(() => MarketManager.GetInstance().CancelMembership(ADMIN_SESSION_ID + 2, _owner.UserName));
            _owner.Appoint(_manager1, _shop, Role.Owner, Permission.Policy);
            Assert.ThrowsException<ArgumentException>(() => MarketManager.GetInstance().CancelMembership(ADMIN_SESSION_ID, _manager1.UserName));


        }

        [TestMethod()]
        public void CancelMembershipSuccess()
        {
            MarketManager.GetInstance().CancelMembership(ADMIN_SESSION_ID, _member.UserName);
            Assert.IsFalse(MemberRepo.GetInstance().ContainsUserName(_member.UserName));
        }
    }
}
