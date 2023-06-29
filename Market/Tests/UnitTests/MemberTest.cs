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
using Market.ServiceLayer;
using Market.IntegrationTests;
using Moq;

namespace Market.DomainLayer.Tests
{
    [TestClass()]
    public class MemberTest
    {
        private Member _founder;
        private Member _member;
        private Member _manager1;
        private Member _manager2;
        private Member _manager3;
        private Member _manager4;
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

            s.Register("2", "benalvo", "12345");
            s.Login("2", "benalvo", "12345");
            s.CreateShop("2", "shop1");
            _founder = UM.GetMember("2");
            _shop = SM.GetShopByName("shop1");
            s.AddProduct("2", _shop.Id, "Ball",0, "this is a ball", 52.6, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop.Id, "Ball1",0, "this is a ball1", 52.6, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop.Id, "Ball2", 0,"this is a ball2", 52.6, 80, Category.None.ToString(), new List<string> ());
            s.AddProduct("2", _shop.Id, "Ball3", 0,"this is a ball3", 52.6, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p1 = _shop.Products.ToList().Find((p) => p.Id == 11);
            _p2 = _shop.Products.ToList().Find((p) => p.Id == 12);
            _p3 = _shop.Products.ToList().Find((p) => p.Id == 13);
            _p4 = _shop.Products.ToList().Find((p) => p.Id == 14);
            s.Register("3", "tamuzgindes", "54321");
            s.Register("4", "gal", "111111");
            s.Register("5", "gigi", "22222");
            s.Register("6", "regevon", "111111");
            s.Register("7", "dummy", "432432");

            s.Login("3","tamuzgindes", "54321");
            s.Login("4", "gal", "111111");
            s.Login("5", "gigi", "22222");
            s.Login("6", "regevon", "111111");
            s.Login("7", "dummy", "432432");

            _member = UM.GetMember("3"); ;
            _manager1 = UM.GetMember("4"); 
            _manager2 = UM.GetMember("5"); 
            _manager3 = UM.GetMember("6");
            _manager4 = UM.GetMember("7");
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
        public void CircularAppoints()
        {
            _founder.Appoint(_member, _shop, Role.Manager, Permission.All);
            Appointment appointment1;
            _member.Appointments.TryGetValue(1, out appointment1);
            Assert.AreEqual(appointment1.Role, Role.Manager);
            Assert.AreEqual(appointment1.Appointer, _founder);
            string errMessage = Assert.ThrowsException<ArgumentException>(() => _member.Appoint(_founder, _shop, Role.Manager, Permission.All)).Message;
            Assert.AreEqual("Circular appointments are NOT allowed!", errMessage);

            _member.Appoint(_manager1, _shop, Role.Manager, Permission.Appoint);
            _manager1.Appoint(_manager2, _shop, Role.Manager, Permission.Appoint);

            Appointment appointment2;
            _manager1.Appointments.TryGetValue(1, out appointment2);
            Assert.AreEqual(appointment2.Role, Role.Manager);
            Assert.AreEqual(appointment2.Appointer, _member);

            Appointment appointment3;
            _manager2.Appointments.TryGetValue(1, out appointment3);
            Assert.AreEqual(appointment3.Role, Role.Manager);
            Assert.AreEqual(appointment3.Appointer, _manager1);

            errMessage = Assert.ThrowsException<ArgumentException>(() => _manager2.Appoint(_founder, _shop, Role.Manager, Permission.All)).Message;
            Assert.AreEqual("Circular appointments are NOT allowed!", errMessage);
            errMessage = Assert.ThrowsException<ArgumentException>(() => _manager2.Appoint(_member, _shop, Role.Manager, Permission.All)).Message;
            Assert.AreEqual("Circular appointments are NOT allowed!", errMessage);
        }

        [TestMethod()]
        public void AppointOwner()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            _founder.Appoint(_manager1, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager1.UserName);
            //case 1: already an owner so exception will be thrown
            Assert.ThrowsException<ArgumentException>(() => _manager1.Appoint(_member, _shop, Role.Manager, Permission.All));
            //case 2: manager1 getting promoted to a owner and the appointment of a manger get overridden
            _founder.Appoint(_manager2, _shop, Role.Manager, Permission.All);
            _manager1.Appoint(_manager2, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("2", _shop.Id, _manager2.UserName);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager2.UserName);

            MemberRepo.GetInstance().GetById(_manager2.Id);
            Assert.AreEqual(Role.Owner, _manager2.Appointments[_shop.Id].Role);
        }
        //====================================================================
        /// --------------- new for version 4 functionality-------------------
        //====================================================================
        [TestMethod()]
        public void AppointOwnerWithAgreement()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            // _member is owner now
            Assert.AreEqual(Role.Owner, _member.Appointments[_shop.Id].Role);
            _founder.Appoint(_manager1, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager1.UserName);
            // _manager1 is owner now
            Assert.AreEqual(Role.Owner, _manager1.Appointments[_shop.Id].Role);

            _manager1.Appoint(_manager2, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("2", _shop.Id, _manager2.UserName);
            MarketManager.GetInstance().AddDecline("3", _shop.Id, _manager2.UserName);
            // _manager2 is not owner
            Assert.IsFalse(_manager2.Appointments.ContainsKey(_shop.Id));
            // won't open another pending request
            Assert.ThrowsException<ArgumentException>(() => _member.Appoint(_manager1, _shop, Role.Owner, Permission.All));
            // will open another one
            _manager1.Appoint(_manager4, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager2.UserName);
            // _manager2 is owner now
            Assert.AreEqual(Role.Owner, _manager2.Appointments[_shop.Id].Role);
            // appointer in manager 1
            Assert.AreEqual(_manager1, _manager2.Appointments[_shop.Id].Appointer);
            Assert.IsTrue(_manager1.Appointments[_shop.Id].Apointees.Contains(_manager2));


            // approved manager 4 but manager 2 also needs to approve now!
            MarketManager.GetInstance().AddApproval("2", _shop.Id, _manager4.UserName);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager4.UserName);
            // needs manager 2 approval
            Assert.IsFalse(_manager4.Appointments.ContainsKey(_shop.Id));
            //no longer needs manager 2 approvak becuse is not an owner anymore
            _manager1.RemoveAppoint(_manager2, _shop);
            Assert.IsTrue(_manager4.Appointments.ContainsKey(_shop.Id));
            Assert.AreEqual(Role.Owner, _manager4.Appointments[_shop.Id].Role);
        }

        [TestMethod()]
        public void AppointManager()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            _founder.Appoint(_manager1, _shop, Role.Manager, Permission.All);
            Assert.AreEqual(Role.Manager, _manager1.Appointments[_shop.Id].Role);
            //case 1: already a manager so exception will be thrown
            Assert.ThrowsException<ArgumentException>(() => _member.Appoint(_manager1, _shop, Role.Manager, Permission.All));
            //case 2: already an owner so exception will be thrown
            _founder.Appoint(_manager2, _shop, Role.Owner, Permission.All);
            MarketManager.GetInstance().AddApproval("3", _shop.Id, _manager2.UserName);
            Assert.ThrowsException<ArgumentException>(() => _member.Appoint(_manager2, _shop, Role.Manager, Permission.All));
        }

        [TestMethod()]
        public void AppointSuccess()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            Assert.IsTrue(_member.Appointments.ContainsKey(1));

        }

        [TestMethod()]
        public void AppointFail()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            Assert.ThrowsException<ArgumentException>(() => _member.Appoint(_founder, _shop, Role.Owner, Permission.All));
        }

        [TestMethod()]
        public void ChangePermissionsSuccess()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            _founder.ChangePermissions(_member, _shop.Id, Permission.ManageSupply);
            Assert.IsTrue(_member.Appointments[_shop.Id].Permissions == Permission.ManageSupply);
        }

        [TestMethod()]
        public void ChangePermissionsFail()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            Assert.ThrowsException<Exception>(() => _member.ChangePermissions(_founder, _shop.Id, Permission.ManageSupply));
        }

        [TestMethod()]
        public void AddPermissionsSuccess()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.ManageSupply);
            _founder.AddPermissions(_member, _shop.Id, Permission.Appoint);
            Assert.IsTrue((_member.Appointments[_shop.Id].Permissions & Permission.ManageSupply) == Permission.ManageSupply);
        }

        [TestMethod()]
        public void AddPermissionsFail()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.Appoint);
            Assert.ThrowsException<Exception>(() => _member.AddPermissions(_member, _shop.Id, Permission.ManageSupply));
        }

        [TestMethod()]
        public void DeletePermissionsSuccess()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.ManageSupply | Permission.Appoint);
            _founder.DeletePermissions(_member, _shop.Id, Permission.Appoint);
            Assert.IsTrue((_member.Appointments[_shop.Id].Permissions & Permission.Appoint) == 0);
        }

        [TestMethod()]
        public void DeletePermissionsFail()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.ManageSupply | Permission.Appoint);
            Assert.ThrowsException<Exception>(() => _member.DeletePermissions(_founder, _shop.Id, Permission.Appoint));
        }

        [TestMethod()]
        public void AddManagerAppointmentSuccess()
        {
            _member.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Manager, Permission.All));
            Assert.IsTrue((_member.Appointments[_shop.Id].Role == Role.Manager));
        }

        [TestMethod()]
        public void AddManagerAppointmentFail()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _founder.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Manager, Permission.All)));
        }

        [TestMethod()]
        public void AddOwnerAppointmentSuccess()
        {
            _member.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Owner, Permission.All));
            Assert.IsTrue((_member.Appointments[_shop.Id].Role == Role.Owner));
        }

        [TestMethod()]
        public void AddOwnerAppointmentFail()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _founder.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Owner, Permission.All)));
        }

        [TestMethod()]
        public void AppointFounderSuccess()
        {
            _member.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Founder, Permission.All));
            Assert.IsTrue((_member.Appointments[_shop.Id].Role == Role.Founder));
        }

        [TestMethod()]
        public void AppointFounderFail()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _founder.AddManagerAppointment(_shop.Id, new Appointment(_member, _shop, _founder, Role.Owner, Permission.All)));
        }
        [TestMethod()]
        public void RemoveAppointSuccess()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            _member.Appoint(_manager1, _shop, Role.Manager, Permission.All);
            _member.Appoint(_manager2, _shop, Role.Manager, Permission.All);
            _manager2.Appoint(_manager3, _shop, Role.Manager, Permission.All);
            _founder.RemoveAppoint(_member, _shop);
    
            Assert.IsFalse(_member.Appointments.ContainsKey(_shop.Id));
            Assert.IsFalse(_manager1.Appointments.ContainsKey(_shop.Id));
            Assert.IsFalse(_manager2.Appointments.ContainsKey(_shop.Id));
            Assert.IsFalse(_manager3.Appointments.ContainsKey(_shop.Id));
            Assert.IsFalse(_founder.Appointments[_shop.Id].Apointees.Contains(_member));
            Assert.IsFalse(_shop.EventManager.Listeners["Pending Agreement Event"].Contains(_member));
        }
        [TestMethod()]
        public void RemoveAppointFail()
        {
            _founder.Appoint(_member, _shop, Role.Owner, Permission.All);
            _founder.Appoint(_manager1, _shop, Role.Manager, Permission.All);
            _manager1.Appoint(_manager2, _shop, Role.Manager, Permission.All);


            Assert.ThrowsException<ArgumentException>(
                () => _manager1.RemoveAppoint(_member, _shop));

            Assert.ThrowsException<ArgumentException>(
                () => _founder.RemoveAppoint(_manager2, _shop));
        }
        [TestMethod()]
        public void AddToCartSuccess()
        {
            _member.AddToCart(_shop, _p1.Id, 5);
            Assert.IsTrue(_member.ShoppingCart.BasketbyShop[_shop.Id].FindBasketItem(_p1.Id).Quantity == 5);
        }

        [TestMethod()]
        public void AddToCartFail()
        {
            Assert.ThrowsException<Exception>(
                () => _member.AddToCart(_shop, _p1.Id, 2000));
        }

        [TestMethod()]
        public void RemoveFromCartSuccess()
        {
            _member.AddToCart(_shop, _p1.Id, 5);
            _member.RemoveFromCart(_shop.Id, _p1.Id);
            Assert.IsTrue(!_member.ShoppingCart.BasketbyShop.ContainsKey(_shop.Id));
        }

        [TestMethod()]
        public void RemoveFromCartFail()
        {
            Assert.ThrowsException<Exception>(
                () => _member.RemoveFromCart(_shop.Id, 2000));
        }

        [TestMethod()]
        public void Notify()
        {
            _member.Notify("Hello");
            Assert.IsTrue(_member.Messages.Any((m)=>m.Comment.Contains("Hello")));
        }
    }
}