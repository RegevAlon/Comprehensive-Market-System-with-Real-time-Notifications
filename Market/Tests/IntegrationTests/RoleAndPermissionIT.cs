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

namespace Market.IntegrationTests
{
    [TestClass]
    public class RoleAndPermissionIT
    {
        UserManager UM;
        ShopManager SM;
        string PrimarysessionID;
        int sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int price;
        SynchronizedCollection<String> keywords;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;

        public RoleAndPermissionIT()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();

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
            PrimarysessionID = "1";
            shopID = 1;
            productID = 10;
            price = 20;
            quantity = 100;
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            BadPermission = 1;
            keywords = new SynchronizedCollection<string>();
            keywords.Add("green apple");
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MarketManager.GetInstance().CreateShop(PrimarysessionID, "Regev Yohananof");

        }

        [TestCleanup]
        public void Cleanup()
        {
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void AddSpecificRolebyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
        }
        [TestMethod]
        public void AddSpecificRolebyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "3";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
        }
        [TestMethod]
        public void ChangeRolebyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "4";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            UM.RemoveAppoint(PrimarysessionID, "ben", myshop);
            Assert.IsTrue(myshop.Appointments.Count() == 1);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Owner, Permission.Appoint);
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Owner);
        }

        [TestMethod]
        public void ChangeRolebyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "5";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            UM.RemoveAppoint(PrimarysessionID, "ben", myshop);
            Assert.IsTrue(myshop.Appointments.Count() == 1);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Owner, Permission.Appoint);
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Owner);
        }
        [TestMethod]
        public void AddSpecificPermissionbyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "6";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
        }
        [TestMethod]
        public void AddSpecificPermissionbyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "7";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
        }
        [TestMethod]
        public void ChangePermissionbyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "8";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.ChangePermissions(PrimarysessionID, "ben", shopID, Permission.ManageSupply);
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.ManageSupply);
        }

        [TestMethod]
        public void ChangePermissionbyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "9";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.ChangePermissions(PrimarysessionID, "ben", shopID, Permission.ManageSupply);
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.ManageSupply);
        }

        [TestMethod]
        public void SecondDegreeChangePermissionbyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "10";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string appointeeApointeeSessionID = "11";
            UM.Register("tamuz", "password");
            UM.Login(appointeeApointeeSessionID, "tamuz", "password");
            Member appointeeAppointeeMember = UM.GetMember(appointeeApointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.Appoint(apointeeSessionID, "tamuz", myshop, Role.Manager, Permission.Appoint);
            if (!appointeeAppointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "ben");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            try
            {
                UM.ChangePermissions(PrimarysessionID, "tamuz", shopID, Permission.ManageSupply);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Only the appointer of this member can change it's permissions");
            }

        }

        [TestMethod]
        public void SecondDegreeChangePermissionbyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "12";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string appointeeApointeeSessionID = "13";
            UM.Register("tamuz", "password");
            UM.Login(appointeeApointeeSessionID, "tamuz", "password");
            Member appointeeAppointeeMember = UM.GetMember(appointeeApointeeSessionID);
            Shop myshop = SM.GetShop(shopID);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Appoint);
            Appointment app;
            if (!myshop.Appointments.TryGetValue(appointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.Appoint(apointeeSessionID, "tamuz", myshop, Role.Manager, Permission.Appoint);
            if (!myshop.Appointments.TryGetValue(appointeeAppointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "ben");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            try
            {
                UM.ChangePermissions(PrimarysessionID, "tamuz", shopID, Permission.ManageSupply);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Only the appointer of this member can change it's permissions");
            }
        }
    }
}
