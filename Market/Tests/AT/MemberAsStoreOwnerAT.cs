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
    [TestClass]
    public class MemberAsStoreOwnerAT
    {
        Proxy proxy;
        string sessionID;
        int shopID;
        int productPrice;
        int productQuantity;
        int productID;
        int FounderRole;
        int OwnerRole;
        int ManagerRole;
        int GoodPermission;
        int BadPermission;



        public MemberAsStoreOwnerAT()
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
            shopID = 1;
            sessionID = proxy.getSessionId();
            proxy.EnterAsGuest(sessionID);
            proxy.Register(sessionID, "user", "password");
            proxy.Login(sessionID, "user", "password");
            proxy.createShop(sessionID, "myFirstShop");
            productPrice = 50;
            productQuantity = 100;
            productID = 11;
            FounderRole = 1;
            OwnerRole = 2;
            ManagerRole = 3;
            GoodPermission = 2;
            BadPermission = 1;

        }

        [TestCleanup]
        public void Cleanup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void AddProductToShop_GoodCase()
        {
            Assert.IsTrue(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "Food", new List<string>()));
        }

        [TestMethod]
        public void AddProductToShop_BadCase()
        {
            Assert.IsFalse(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", -productPrice, productQuantity, "Food", new List<string>()));
        }

        [TestMethod]
        public void RemoveProductFromShop_GoodCase()
        {
            Assert.IsTrue(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "food", new List<string>()));
            Assert.IsTrue(proxy.RemoveProduct(sessionID, shopID, productID));


        }

        [TestMethod]
        public void RemoveProductFromShop_BadCase()
        {
            Assert.IsTrue(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "food", new List<string>()));
            int badProductID = productID + 1;
            Assert.IsFalse(proxy.RemoveProduct(sessionID, shopID, badProductID));
        }

        [TestMethod]
        public void ChangeProductDetails_GoodCase()
        {
            Assert.IsTrue(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "food", new List<string>()));
            int price = 100;
            Assert.IsTrue(proxy.UpdateProductPrice(sessionID, shopID, productID, price));


        }

        [TestMethod]
        public void ChangeProductDetails_BadCase()
        {
            Assert.IsTrue(proxy.AddProduct(sessionID, shopID, "Apple", "green apple", productPrice, productQuantity, "food", new List<string>()));
            int price = -100;
            Assert.IsFalse(proxy.UpdateProductPrice(sessionID, shopID, productID, price));

        }

        [TestMethod]
        public void ShopOwnerAppointment_GoodCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, OwnerRole, GoodPermission));

        }

        [TestMethod]
        public void ShopOwnerAppointment_BadCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(appointedseesionID, "usertoappoint", "password"));
            Assert.IsFalse(proxy.Appoint(sessionID, "usertoappoint", shopID, FounderRole, GoodPermission));

        }

        [TestMethod]
        public void ShopManagerAppointment_GoodCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));

        }

        [TestMethod]
        public void ShopManagerAppointment_BadCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            int badShopID = -1;
            Assert.IsFalse(proxy.Appoint(sessionID, "usertoappoint", badShopID, ManagerRole, GoodPermission));
        }

        [TestMethod]
        public void ShopManagerPermissionChange_GoodCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));
            int permissionToAddSupply = 4;
            Assert.IsTrue(proxy.changePermission(sessionID, "usertoappoint", shopID, permissionToAddSupply));

        }

        [TestMethod]
        public void ShopManagerPermissionChange_BadCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));
            int badShopID = -1;
            int permissionToAddSupply = 4;
            Assert.IsFalse(proxy.changePermission(sessionID, "usertoappoint", badShopID, permissionToAddSupply));

        }

        [TestMethod]
        public void CloseShop_GoodCase()
        {
            Assert.IsTrue(proxy.closeShop(sessionID, shopID));

        }

        [TestMethod]
        public void CloseShop_BadCase()
        {
            Assert.IsTrue(proxy.closeShop(sessionID, shopID));
            Assert.IsFalse(proxy.closeShop(sessionID, shopID));

        }
        [TestMethod]
        public void ShopEmployeesInfo_GoodCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));
            Assert.IsTrue(proxy.GetShopPositions(sessionID, shopID));

        }

        [TestMethod]
        public void ShopEmployeesInfo_BadCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.EnterAsGuest(appointedseesionID));
            Assert.IsTrue(proxy.Register(appointedseesionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));
            int badShopID = -1;
            Assert.IsFalse(proxy.GetShopPositions(sessionID, badShopID));
        }

        [TestMethod]
        public void PurchaseHistoryInfo_GoodCase()
        {

            Assert.IsTrue(proxy.ShowPurchaseHistory(sessionID));

        }

        [TestMethod]
        public void PurchaseHistoryInfo_BadCase()
        {
            string appointedseesionID = proxy.getSessionId();
            Assert.IsTrue(proxy.Register(sessionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Login(sessionID, "usertoappoint", "password"));
            Assert.IsTrue(proxy.Appoint(sessionID, "usertoappoint", shopID, ManagerRole, GoodPermission));
            Assert.IsFalse(proxy.ShowPurchaseHistory(appointedseesionID));

        }


    }
}
