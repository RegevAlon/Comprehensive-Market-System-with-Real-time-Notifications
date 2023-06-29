using Market.DataLayer;
using Market.DomainLayer;
using Market.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Market.AT
{
    [TestClass()]
    public class UserGuestAT
    {
        //user1
        string username1 = "user1";
        string userpass1 = "pass1";
        string sessid1;
        //user2
        string username2 = "user2";
        string userpass2 = "pass2";
        string sessid2;
        //user3
        string username3 = "user3";
        string userpass3 = "pass3";
        string sessid3;
        //shop1
        string shop1 = "shop1";
        int shopId1 = 1;
        //shop2
        string shop2 = "shop2";
        int shopId2 = 2;
        //product1
        string productname1 = "product1";
        int productid1 = 1;
        string productdescription1 = "important product";
        double productprice1 = 10;
        int productquantity1 = 2;
        string productcategory1 = "Furnitures";
        List<string> productkeyWords1 = new List<string>() { "pro", "duct" };
        //product2
        string product2 = "product2";
        int productid2 = 2;
        string productdescription2 = "important product";
        double productprice2 = 5;
        int productquantity2 = 3;
        string productcategory2 = "Furnitures";
        List<string> productkeyWords2 = new List<string>() { "pro", "duct" };
        //proxy
        Proxy proxy;
        [TestInitialize()]
        public void Setup()
        {
            
            MarketContext.GetInstance().Dispose();
            proxy = new Proxy();
            sessid1 = proxy.getSessionId();
            sessid2 = proxy.getSessionId();
            sessid3 = proxy.getSessionId();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
        }
        [TestCleanup]
        public void Tearup()
        {
            proxy.Dispose();
            MarketContext.GetInstance().Dispose();
        }


        [TestMethod()]
        public void EnterAsUserGuestSucces()
        {
            Assert.IsFalse(proxy.Login(sessid1, username1, userpass1));
        }

        [TestMethod]
        public void RegisterTestWithValidDetails()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
        }
        [TestMethod]
        public void RegisterTestWithInvalidDetailsRegisteringTwice()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsFalse(proxy.Register(sessid1, username1, userpass1));
        }
        [TestMethod]
        public void LoginTestWithValidDetails()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
        }
        [TestMethod]
        public void LoginTestWithInvalidpassword()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsFalse(proxy.Login(sessid1, username1, userpass2));
        }
        [TestMethod]
        public void LogoutTestWithValid()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Logout(sessid1));

        }
        [TestMethod]
        public void LogoutTestWithInValidDetails_userNotRegistered()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid3));
            Assert.IsFalse(proxy.Login(sessid1, username3, userpass3));
        }
        [TestMethod]
        public void getInfoAboutMarketTestSucces()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.createShop(sessid1, "newShop"));
            List<SShop> shops =  proxy.GetMarketInfo(sessid1);
            Assert.AreEqual(shops.Count, 1);
            Assert.AreEqual(shops.First().name, "newShop");

        }
        [TestMethod]
        public void SearchTestSucces()
        {
            //arrange
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.createShop(sessid1, shop1));
            Assert.IsTrue(proxy.AddProduct(sessid1, shopId1, productname1, productdescription1, productprice1, productquantity1, productcategory1, productkeyWords1));
            List<int> filters = new List<int>();
            int searchByCategory = 2;
            int minPrice = 0;
            int maxPrice = int.MaxValue;
            int minRate = 0;
            int maxRate = 5;
            filters.Add(searchByCategory);
            List<SProduct> products = proxy.SearchProducts(sessid1, productcategory1, filters, filters, minPrice, maxPrice, minRate, maxRate, productcategory1);
            Assert.AreEqual(products.Count, 1);
            Assert.AreEqual(products.First().name, productname1);

        }
        [TestMethod]
        public void AddToCartSucces()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.createShop(sessid1, shop1));
            Assert.IsTrue(proxy.AddProduct(sessid1, shopId1, productname1, productdescription1, productprice1, productquantity1, productcategory1, productkeyWords1));
            Assert.IsTrue(proxy.AddToCart(sessid1, 1, 11, 1));
            SShoppingCart cart = proxy.GetShoppingCartInfo(sessid1);
            Assert.IsNotNull(cart);

        }
        [TestMethod]
        public void AddToCartFailBecauseOfUsingWrongSessionID()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.createShop(sessid1, shop1));
            Assert.IsTrue(proxy.AddProduct(sessid1, shopId1, productname1, productdescription1, productprice1, productquantity1, productcategory1, productkeyWords1));
            SShoppingCart cart = proxy.GetShoppingCartInfo(sessid2);
            Assert.IsNull(cart);

        }
        [TestMethod]
        public void AddToCartFailWithWrongSessionId()
        {
            Assert.IsTrue(proxy.EnterAsGuest(sessid1));
            Assert.IsTrue(proxy.Register(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.Login(sessid1, username1, userpass1));
            Assert.IsTrue(proxy.createShop(sessid1, shop1));
            Assert.IsTrue(proxy.AddProduct(sessid1, shopId1, productname1, productdescription1, productprice1, productquantity1, productcategory1, productkeyWords1));
            SShoppingCart cart = proxy.GetShoppingCartInfo(sessid2);
            Assert.IsNull(cart);

        }
    }
}