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
    public class AppointmentIT
    {
        UserManager UM;
        ShopManager SM;
        MarketManager MM;
        MarketContext MC;
        string PrimarysessionID;
        string sessionID_Owner;
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

        public AppointmentIT()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();
            MC = MarketContext.GetInstance();
        }


        [TestInitialize]
        public void Setup()
        {
            MC.Dispose();
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
            UM.EnterAsGuest(PrimarysessionID);
            UM.Register("regev", "password");
            UM.Login(PrimarysessionID, "regev", "password");
            Member member = UM.GetMember(PrimarysessionID);
            MM.CreateShop(PrimarysessionID, "Regev Yohananof");
            


        }

        [TestCleanup]
        public void Cleanup()
        {
            MC.Dispose();
            MarketManager.GetInstance().Dispose();
        }

        [TestMethod]
        public void AppointManagerbyUser()
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
            Assert.IsTrue(app.Permissions == Permission.Appoint);
        }
        [TestMethod]
        public void AppointManagerbyShop()
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
            Assert.IsTrue(app.Permissions == Permission.Appoint);
        }

        [TestMethod]
        public void AppointAppointManagerbyUser()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "4";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string appointeeApointeeSessionID = "5";
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
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.Appoint(apointeeSessionID, "tamuz", myshop, Role.Manager, Permission.Appoint);
            if (!appointeeAppointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "ben");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);


        }
        [TestMethod]
        public void AppointAppointManagerbyShop()
        {
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "6";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string appointeeApointeeSessionID = "7";
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
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);
            UM.Appoint(apointeeSessionID, "tamuz", myshop, Role.Manager, Permission.Appoint);
            if (!myshop.Appointments.TryGetValue(appointeeAppointeeMember.Id, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "ben");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Appoint);

        }
    }


}
