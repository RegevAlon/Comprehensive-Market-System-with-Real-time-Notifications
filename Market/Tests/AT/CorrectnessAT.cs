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
    public class CorrectnessAT
    {
        Proxy proxy;
        string primarySessionID;
        int productPrice;
        int productQuantity;
        int adminSessionID;
        List<string> keywords;

        public CorrectnessAT()
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
            primarySessionID = proxy.getSessionId();
            productPrice = 50;
            productQuantity = 100;
            adminSessionID = 0;
            keywords = new List<string>();

        }

        [TestCleanup]
        public void Cleanup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void UniqueUsername_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));

        }
        [TestMethod]
        public void UniqueUsername_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername", "password"));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsFalse(proxy.Register(secondaryID, "uniqueUsername", "otherPassword"));
        }

        [TestMethod]
        public void ExistingMarketManager_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername", "password"));
            Assert.IsTrue(!primarySessionID.Equals(adminSessionID));
        }

        [TestMethod]
        public void ActivityAsMember_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            int productID = 11;
            Assert.IsTrue(proxy.AddToCart(secondaryID, shopID, productID, productQuantity));

        }

        [TestMethod]
        public void ActivityAsMember_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Logout(secondaryID));
            Assert.IsFalse(proxy.AddToCart(secondaryID, shopID, productPrice, productQuantity));
        }

        [TestMethod]
        public void ExistingSystemAdmin_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername", "password"));
            Assert.IsFalse(primarySessionID.Equals(adminSessionID));
        }

        [TestMethod]
        public void AddPurchasePolicy_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            Assert.IsTrue(proxy.AddSimpleRule(primarySessionID, shopID, "Food"));
            int ruleId = 11;
            Assert.IsTrue(proxy.AddPurchasePolicy(primarySessionID, shopID, "01/01/2030", "Food",ruleId));
            int policyId = 11;
            ruleId = 12;
            Assert.IsTrue(proxy.AddSimpleRule(primarySessionID, shopID, "Food"));
            Assert.IsTrue(proxy.AddPurchasePolicy(primarySessionID, shopID, "01/01/2030", "Food", ruleId));
            Assert.IsTrue(proxy.RemovePolicy(primarySessionID, shopID, policyId, "PurchasePolicy"));
            policyId = 12;
            Assert.IsTrue(proxy.RemovePolicy(primarySessionID, shopID, policyId, "PurchasePolicy"));

        }

        [TestMethod]
        public void AddPurchasePolicy_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.AddSimpleRule(primarySessionID, shopID, "Food"));
            int ruleId = 11;
            Assert.IsTrue(proxy.AddPurchasePolicy(primarySessionID, shopID, "01/01/2030", "Food", ruleId));
            int policyId = 11;
            int wrongRuleId = -1;
            Assert.IsTrue(proxy.AddSimpleRule(primarySessionID, shopID, "Food"));
            Assert.IsFalse(proxy.AddPurchasePolicy(primarySessionID, shopID, "01/01/2030", "Food", wrongRuleId));
            Assert.IsTrue(proxy.RemovePolicy(primarySessionID, shopID, policyId, "PurchasePolicy"));
            policyId = 12;
            Assert.IsFalse(proxy.RemovePolicy(primarySessionID, shopID, policyId, "PurchasePolicy"));

        }

        [TestMethod]
        public void PurchaseDemand_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            int productID = 11;
            Assert.IsTrue(proxy.AddToCart(secondaryID, shopID, productID, productQuantity));
        }

        [TestMethod]
        public void PurchaseDemand_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            int productID = 10;
            int badProductQuantity = 200;
            Assert.IsFalse(proxy.AddToCart(secondaryID, shopID, productID, badProductQuantity));
        }

        [TestMethod]
        public void SellerPurchaseAcceptance_GoodCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            int productID = 11;
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.AddToCart(secondaryID, shopID, productID, productQuantity));
            Assert.IsTrue(proxy.PurchaseCart(secondaryID));
            Assert.IsTrue(proxy.AcceptPayment(primarySessionID));
        }

        [TestMethod]
        public void SellerPurchaseAcceptance_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            string secondaryID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(secondaryID));
            Assert.IsTrue(proxy.Register(secondaryID, "uniqueUsername2", "otherPassword"));
            Assert.IsTrue(proxy.Login(secondaryID, "uniqueUsername2", "otherPassword"));
            int productID = 11;
            int badProductQuantity = 200;
            Assert.IsFalse(proxy.AddToCart(secondaryID, shopID, productID, badProductQuantity));
            Assert.IsTrue(proxy.PurchaseCart(secondaryID));
            Assert.IsTrue(proxy.AcceptPayment(primarySessionID));
        }

        [TestMethod]
        public void ExistingPaymentMethod_BadCase()
        {
            Assert.IsTrue(proxy.EnterAsGuest(primarySessionID));
            Assert.IsTrue(proxy.Register(primarySessionID, "uniqueUsername1", "password"));
            Assert.IsTrue(proxy.Login(primarySessionID, "uniqueUsername1", "password"));
            int shopID = 1;
            Assert.IsTrue(proxy.createShop(primarySessionID, "A dummy shop"));
            Assert.IsTrue(proxy.AddProduct(primarySessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", keywords));
            Assert.IsFalse(proxy.GetPaymentMethods(primarySessionID,shopID));
        }

    }
}
