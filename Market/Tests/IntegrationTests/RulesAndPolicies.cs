using Market.DataLayer;
using Market.DomainLayer;
using Market.DomainLayer.Rules;
using Market.RepoLayer;
using Market.ServiceLayer;
using Market.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.IntegrationTests
{
    [TestClass]
    public class RulesAndPolicies
    {
        UserManager UM;
        ShopManager SM;
        MarketManager MM;
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
        string FoodCategory;
        string pockemonCategory;


        public RulesAndPolicies()
        {
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<DomainLayer.IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
        }

        [TestInitialize]
        public void Setup()
        {
            MarketContext.GetInstance().Dispose();

            PrimarysessionID = "1";
            shopID = 1;
            productID = 11;
            price = 20;
            quantity = 100;
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            FoodCategory = "Food";
            pockemonCategory = "pockemon";
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
            MarketManager.GetInstance().Dispose();
            MarketContext.GetInstance().Dispose();
            MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<DomainLayer.IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;
        }

        [TestMethod]
        public void PurchasePolicy_badCase()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string buyerSessionID = "3";
            UM.Register("tamuz", "password");
            UM.Login(buyerSessionID, "tamuz", "password");
            Member buyerMember = UM.GetMember(buyerSessionID);
            Shop myshop = SM.GetShop(shopID);
            SM.AddProduct(Appointer.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Policy);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Policy);
            SM.AddQuantityRule(shopID, appointeeMember.Id, FoodCategory, minQuantity, maxQuantity);
            QuantityRule r = (QuantityRule)RuleRepo.GetInstance().GetById(11);
            Assert.IsTrue(r.MinQuantity == 5 && r.MaxQuantity == 20);
            SM.AddPurchasePolicy(shopID, appointeeMember.Id, "01.01.2030", FoodCategory, 11);
            IPolicy policy;
            if (!myshop.PurchasePolicyManager.Policies.TryGetValue(11, out policy))
            {
                Assert.IsTrue(false);
            }
            UM.AddToCart(buyerSessionID, myshop, productID, maxQuantity + 1);
            try
            {
                UM.Purchase(buyerSessionID, shopID);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Basket does not stand with purchase policy constraints");
            }

        }
        [TestMethod]
        public void DiscountPolicy_GoodCase()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string buyerSessionID = "3";
            UM.Register("tamuz", "password");
            UM.Login(buyerSessionID, "tamuz", "password");
            Member buyerMember = UM.GetMember(buyerSessionID);
            Shop myshop = SM.GetShop(shopID);
            SM.AddProduct(Appointer.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Policy);
            Product myprod = myshop.Products.ElementAt(0);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Policy);
            SM.AddQuantityRule(shopID, appointeeMember.Id, myprod.Name, minQuantity, maxQuantity);
            int ruleId = 11;
            double discountPrecentage = 0.1;
            QuantityRule r = (QuantityRule)RuleRepo.GetInstance().GetById(ruleId);
            Assert.IsTrue(r.MinQuantity == 5 && r.MaxQuantity == 20);
            SM.AddDiscountPolicy(shopID, appointeeMember.Id, "01.01.2030", myprod.Name, ruleId, discountPrecentage);
            int policyId = 11;
            IPolicy policy;
            myshop.DiscountPolicyManager.GetPolicy(policyId);
            int quantityToCart = maxQuantity - 1; // good quantity
            UM.AddToCart(buyerSessionID, myshop, productID, quantityToCart);
            Basket basket;
            if (!buyerMember.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = quantityToCart * (myprod.Price);
            double priceAfterDiscount = (totalprice) * (1 - discountPrecentage);
            double basketPrice = basket.GetBasketPrice();
            Assert.AreEqual(basketPrice, priceAfterDiscount);
            buyerMember.RemoveFromCart(shopID, productID);
            quantityToCart = maxQuantity + 1; // bad quantity
            UM.AddToCart(buyerSessionID, myshop, productID, quantityToCart);
            totalprice = quantityToCart * (myprod.Price);
            if (!buyerMember.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.AreEqual(basket.GetBasketPrice(), totalprice);

        }

        [TestMethod]
        public void DiscountPolicyWithCompositeRule_GoodCase()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string buyerSessionID = "3";
            UM.Register("tamuz", "password");
            UM.Login(buyerSessionID, "tamuz", "password");
            Member buyerMember = UM.GetMember(buyerSessionID);
            Shop myshop = SM.GetShop(shopID);
            SM.AddProduct(Appointer.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Policy);
            Product myprod = myshop.Products.ElementAt(0);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Policy);
            int goodTargetPrice = 100;
            int firstRuleId = 11;
            SM.AddQuantityRule(shopID, appointeeMember.Id, myprod.Name, minQuantity, maxQuantity);
            QuantityRule r = (QuantityRule)RuleRepo.GetInstance().GetById(firstRuleId);
            Assert.IsTrue(r.MinQuantity==5&&r.MaxQuantity==20);
            int secondRuleId = 12;
            SM.AddTotalPriceRule(shopID, appointeeMember.Id, myprod.Name, goodTargetPrice);
            TotalPriceRule r2 = (TotalPriceRule)RuleRepo.GetInstance().GetById(secondRuleId);
            Assert.IsTrue(r2.TotalPrice==100);
            List<int> rules = new List<int> { firstRuleId, secondRuleId };
            int xorOperator = 1;
            SM.AddCompositeRule(shopID, appointeeMember.Id, xorOperator, rules);
            int thirdRuleId = 13;
            double discountPrecentage = 0.1;
            SM.AddDiscountPolicy(shopID, appointeeMember.Id, "01.01.2030", myprod.Name, thirdRuleId, discountPrecentage);
            int policyId = 11;
            myshop.DiscountPolicyManager.GetPolicy(policyId);
            int quantityToCart = maxQuantity + 1; // bad quantity
            UM.AddToCart(buyerSessionID, myshop, productID, quantityToCart);
            Basket basket;
            if (!buyerMember.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = quantityToCart * (myprod.Price);
            double priceAfterDiscount = basket.GetBasketPriceBeforeDiscounts();
            double basketPrice = basket.GetBasketPrice();
            Assert.AreEqual(basketPrice, priceAfterDiscount);
        }
        [TestMethod]
        public void CompositePolicy_GoodCase()
        {
            int minQuantity = 5;
            int maxQuantity = 20;
            Member Appointer = UM.GetMember(PrimarysessionID);
            string apointeeSessionID = "2";
            UM.Register("ben", "password");
            UM.Login(apointeeSessionID, "ben", "password");
            Member appointeeMember = UM.GetMember(apointeeSessionID);
            string buyerSessionID = "3";
            UM.Register("tamuz", "password");
            UM.Login(buyerSessionID, "tamuz", "password");
            Member buyerMember = UM.GetMember(buyerSessionID);
            Shop myshop = SM.GetShop(shopID);
            SM.AddProduct(Appointer.Id, shopID, "apple", new RegularSell(), "green apple from the field", price, quantity, "Food", keywords);
            UM.Appoint(PrimarysessionID, "ben", myshop, Role.Manager, Permission.Policy);
            Product myprod = myshop.Products.ElementAt(0);
            Appointment app;
            if (!appointeeMember.Appointments.TryGetValue(shopID, out app))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(app.Appointer.UserName == "regev");
            Assert.IsTrue(app.Role == Role.Manager);
            Assert.IsTrue(app.Permissions == Permission.Policy);
            int goodTargetPrice = 1000;
            int firstRuleId = 11;
            SM.AddQuantityRule(shopID, appointeeMember.Id, myprod.Name, minQuantity, maxQuantity);
            int secondRuleId = 12;
            SM.AddTotalPriceRule(shopID, appointeeMember.Id, myprod.Name, goodTargetPrice);
            double firstdiscount = 0.1;
            double secondDiscount = 0.5;
            SM.AddDiscountPolicy(shopID, appointeeMember.Id, "01.01.2030", myprod.Name, firstRuleId, firstdiscount);
            int firstPolicyId = 11;
            SM.AddDiscountPolicy(shopID, appointeeMember.Id, "01.01.2030", myprod.Name, secondRuleId, secondDiscount);
            int secondPolicyId = 12;
            myshop.DiscountPolicyManager.GetPolicy(firstPolicyId);
            myshop.DiscountPolicyManager.GetPolicy(secondPolicyId);
            List<int> policies = new List<int> { firstPolicyId, secondPolicyId };
            int addOperator = 0;
            SM.AddCompositePolicy(shopID, appointeeMember.Id, "01.01.2030", myprod.Name, addOperator, policies);
            int quantityToCart = maxQuantity - 1; // good quantity
            UM.AddToCart(buyerSessionID, myshop, productID, quantityToCart);
            Basket basket;
            if (!buyerMember.ShoppingCart.BasketbyShop.TryGetValue(shopID, out basket))
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(basket.HasProduct(myprod));
            double totalprice = quantityToCart * (myprod.Price);
            double priceAfterDiscount = 
                quantityToCart * (myprod.Price *(1- firstdiscount)*(1- secondDiscount));
            double basketPrice = basket.GetBasketPrice();
            Assert.AreEqual(basketPrice, priceAfterDiscount);

        }
    }
}
