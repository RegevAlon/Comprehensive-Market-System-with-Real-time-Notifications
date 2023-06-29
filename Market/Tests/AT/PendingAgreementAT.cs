using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer;
using Moq;
using Market.DataLayer.DTOs;
using Market.ServiceLayer;

namespace Tests.AT
{
    [TestClass]
    public class PendingAgreementAT
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
        private MarketService s;
        MarketContext _context = MarketContext.GetInstance();
        [TestInitialize]
        public void Initialize()
        {
            _context.Dispose();
            s = MarketService.GetInstance();
            MarketContext.GetInstance().Dispose();
            MarketContext context = MarketContext.GetInstance();
            UserManager UM = UserManager.GetInstance();
            ShopManager SM = ShopManager.GetInstance();

            s.Register("2", "benalvo", "12345");
            s.Login("2", "benalvo", "12345");
            s.CreateShop("2", "shop1");
            _founder = UM.GetMember("2");
            _shop = SM.GetShopByName("shop1");
            s.AddProduct("2", _shop.Id, "Ball", 0, "this is a ball", 52.6, 80, Category.None.ToString(), new List<string> { "soccer", "basketball", "round" });
            s.AddProduct("2", _shop.Id, "Ball1", 0, "this is a ball1", 52.6, 80, Category.Pockemon.ToString(), new List<string> { "basketball", "round", "Pockemon" });
            s.AddProduct("2", _shop.Id, "Ball2", 0, "this is a ball2", 52.6, 80, Category.None.ToString(), new List<string>());
            s.AddProduct("2", _shop.Id, "Ball3", 0, "this is a ball3", 52.6, 80, Category.Furnitures.ToString(), new List<string> { "table" });
            _p1 = _shop.Products.ToList().Find((p) => p.Id == 11);
            _p2 = _shop.Products.ToList().Find((p) => p.Id == 12);
            _p3 = _shop.Products.ToList().Find((p) => p.Id == 13);
            _p4 = _shop.Products.ToList().Find((p) => p.Id == 14);
            s.Register("3", "tamuzgindes", "54321");
            s.Register("4", "gal", "111111");
            s.Register("5", "gigi", "22222");
            s.Register("6", "regevon", "111111");
            s.Register("7", "dummy", "432432");

            s.Login("3", "tamuzgindes", "54321");
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
        public void CleanUp()
        {
            _context.Dispose();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
                .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
                .Returns(true);
        }
        [TestMethod]
        public void AppointOwnerWithAgreement()
        {
            SMember _founder = (SMember)s.GetUser("2").Value;
            s.Appoint("2", "tamuzgindes", _shop.Id, 2, 127);
            SShop sshop=s.GetShopInfo("2", _shop.Id).Value;
            // _member is owner now
            Assert.AreEqual(Role.Owner, _member.Appointments[_shop.Id].Role);
            s.Appoint("2", "gal", _shop.Id, 2, 127);

            s.ApproveAppointment("3", _shop.Id, _manager1.UserName);
            // _manager1 is owner now

            Assert.AreEqual(Role.Owner, _manager1.Appointments[_shop.Id].Role);
            s.Appoint("4", "gigi", _shop.Id, 2, 127);

            s.ApproveAppointment("2", _shop.Id, _manager2.UserName);
            s.DeclineAppointment("3", _shop.Id, _manager2.UserName);
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
        [TestMethod]
        public void PendingAgreements()
        {
            MemberDTO member1 = new MemberDTO(1, "Tamuz", "123", true);
            MemberDTO member2 = new MemberDTO(2, "Gal", "321", true);
            MemberDTO member3 = new MemberDTO(3, "Ben", "111", true);


            ShopDTO shop = new ShopDTO(1, "Tamuz's Shop", true, 5.0);
            AppointmentDTO appointment1 = new AppointmentDTO(member1.Id, shop.Id, null, new List<MemberDTO>(), "Founder", ((int)Market.DomainLayer.Permission.All));

            _context.Add<ShopDTO>(shop);
            _context.Add<MemberDTO>(member1);
            _context.Add<MemberDTO>(member2);
            _context.Add<MemberDTO>(member3);

            _context.SaveChanges();

            _context.Add<AppointmentDTO>(appointment1);
            _context.SaveChanges();

            AppointmentDTO queryAppoint = _context.Find<AppointmentDTO>(appointment1.MemberId, appointment1.ShopId);
            Assert.AreEqual(appointment1, queryAppoint);

            //The appoint will be added sucssefully because there are no owners to give their approve
            AppointmentDTO appointment2 = new AppointmentDTO(member2.Id, shop.Id, member1, new List<MemberDTO>(), "Owner", ((int)Market.DomainLayer.Permission.All));

            appointment1.Appointees.Add(new AppointeesDTO(member2));
            _context.Add<AppointmentDTO>(appointment2);
            _context.SaveChanges();

            AppointmentDTO queryAppoint2 = _context.Find<AppointmentDTO>(appointment2.MemberId, appointment2.ShopId);
            Assert.AreEqual(appointment2, queryAppoint2);

            //The appoint will not be added sucssefully because member2 needs to approve
            PendingAgreementDTO pending1 = new PendingAgreementDTO(shop.Id, member1.Id, member3.Id);
            AgreementAnswerDTO ans1 = new AgreementAnswerDTO(member2.Id, "Pending");
            pending1.Answers.Add(ans1);
            shop.PendingAgreements.Add(pending1);
            _context.Update<ShopDTO>(shop);
            _context.SaveChanges();
            ans1.Answer = "Approved";
            _context.SaveChanges();

            PendingAgreementDTO queryPending1 = _context.Find<PendingAgreementDTO>(pending1.ShopId, pending1.AppointeeId);
            Assert.AreEqual(pending1, queryPending1);
            Assert.AreEqual(ans1, queryPending1.Answers[0]);
            Assert.AreEqual("Approved", queryPending1.Answers[0].Answer);


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

    }
}
