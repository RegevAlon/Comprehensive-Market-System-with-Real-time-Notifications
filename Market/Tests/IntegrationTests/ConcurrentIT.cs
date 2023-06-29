using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Market.AT;
using Market.DataLayer;
using Market.DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Market.IntegrationTests
{
    [TestClass]

    public class ConcurrentIT
    {
        string username1 = "user1";
        string userpass1 = "pass1";
        string sessid1 = "1";
        //user2
        string username2 = "user2";
        string userpass2 = "pass2";
        string sessid2 = "2";
        //user3
        string username3 = "user3";
        string userpass3 = "pass3";
        string sessid3 = "3";
        //shop1
        string shop1 = "shop1";
        int shopId1 = 1;
        //shop2
        string shop2 = "shop2";
        int shopId2 = 2;
        //product1
        string productname1 = "product1";
        int productid1 = 11;
        string productdescription1 = "important product";
        double productprice1 = 10;
        int productquantity1 = 2;
        string productcategory1 = "Furnitures";
        SynchronizedCollection<string> productkeyWords1 = new SynchronizedCollection<string>() { "pro", "duct" };
        //product2
        private const int NumThreads = 10;
        private const int NumIterations = 100;

        private ShopManager _shopManager;
        private UserManager _userManager;
        private MarketManager _marketManager;
        [TestInitialize]
        public void Setup()
        {
            // Initialize the managers
            MarketContext.GetInstance().Dispose();
            _marketManager = MarketManager.GetInstance();
            _shopManager = ShopManager.GetInstance();
            _userManager = UserManager.GetInstance();
            MarketManager MM = MarketManager.GetInstance();
            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
             .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
             .Returns(true);
        }
        [TestCleanup]
        public void Cleanup()
        {
            _marketManager.Dispose();
            MarketContext.GetInstance().Dispose();
        }

        [TestMethod]
        public void TestConcurrentShopManager()
        {
            _marketManager.Register(sessid1, username1, userpass1);
            _marketManager.Login(sessid1, username1, userpass1);
            Member mem = _userManager.GetMember(sessid1);
            _marketManager.CreateShop(sessid1, shop1);
            // Create multiple threads that add and remove products from the shop
            var threads = new List<Thread>();
            for (int i = 0; i < NumThreads; i++)
            {
                string pName = $"{productname1}-{i}-";
                threads.Add(new Thread(() =>
                {
                    for (int j = 0; j < NumIterations; j++)
                    {
                        Product p = _shopManager.AddProduct(mem.Id, shopId1,pName+j.ToString() , new RegularSell(), productdescription1, productprice1, 1, productcategory1, productkeyWords1);
                        _marketManager.RemoveProduct(sessid1, shopId1, p.Id);
                    }
                }));
            }

            // Start the threads and wait for them to finish
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            // Assert that the shop has the correct number of products
            Assert.AreEqual(0, _shopManager.GetShop(shopId1).Products.Count);
        }
        [TestMethod]
        public void TestConcurrentShopCreation()
        {

            _marketManager.Register(sessid2,username2, userpass2);
            _marketManager.Login(sessid2, username2, userpass2);
            Member mem = _userManager.GetMember(sessid2);
            int numThreads = 10;
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                string name = $"shop{i}";
                threads.Add(new Thread(() => _marketManager.CreateShop(sessid2, string.Copy(name))));
            }
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            for (int i = 0; i < numThreads; i++)
            {
                try
                {
                    _shopManager.Shops.GetByName($"shop{i}");
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            //Assert.AreEqual(10, _shopManager.Shops.GetAll().Count);
        }

        //[TestMethod]
        //public void TestConcurrentUserManager()
        //{
        //    // Create multiple threads that add and remove users from the shop
        //    var threads = new List<Thread>();
        //    for (int i = 0; i < NumThreads; i++)
        //    {
        //        threads.Add(new Thread(() =>
        //        {
        //            for (int j = 0; j < NumIterations; j++)
        //            {

        //                _userManager.Register(username3, userpass3);
        //                _userManager.Login(sessid3, username3, userpass3);
        //            }
        //        }));
        //    }

        //    // Start the threads and wait for them to finish
        //    threads.ForEach(t => t.Start());
        //    threads.ForEach(t => t.Join());

        //    // Assert that the shop has the correct number of users
        //    //Assert.AreEqual(false, _userManager.GetUser(sessid3) == null);
        //}

        //[TestMethod]
        //public void TestConcurrentShopAndUserCreation()
        //{
        //    _userManager.Register(username1, userpass1);
        //    _userManager.Login(sessid1, username1, userpass1);
        //    Member mem = _userManager.GetMember(sessid1);
        //    var tasks = new List<Task>();
        //    int ThreadCount = 10;
        //    for (int i = 2; i < ThreadCount + 2; i++)
        //    {
        //        string sessionId = $"{i}";
        //        string shopname = $"Shop{i}";
        //        string username = $"User{i}";
        //        string passwordUser = $"pass{i}";

        //        var task = Task.Run(() =>
        //        {
        //            _shopManager.CreateShop(mem, string.Copy(shopname));
        //            Shop s = _shopManager.Shops.GetByName(shopname);
        //            mem.AppointFounder(s);
        //            _userManager.Register(string.Copy(username), string.Copy(passwordUser));
        //            _userManager.Login(string.Copy(sessionId), string.Copy(username), string.Copy(passwordUser));
        //        });

        //        tasks.Add(task);
        //    }

        //    Task.WaitAll(tasks.ToArray());

        //    int shops = _shopManager.Shops.GetAll().Count;
        //    Assert.AreEqual(ThreadCount, shops);
        //    for (int i = 2; i < ThreadCount + 2; i++)
        //    {
        //        Assert.IsTrue(_shopManager.Shops.GetByName($"Shop{i}") != null);
        //        Assert.IsTrue(_shopManager.Shops.GetByName($"Shop{i}").Appointments[mem.Id] != null);
        //    }
        //}

        [TestMethod]
        public void ConcurrentTwoUsersTryingToAppointSameMember()
        {
            // Create multiple threads that add and remove users from the shop
            _userManager.Register(username1, userpass1);
            _userManager.Login(sessid1, username1, userpass1);
            Member mem1 = _userManager.GetMember(sessid1);
            _userManager.Register(username2, userpass2);
            _userManager.Login(sessid2, username2, userpass2);
            Member mem2 = _userManager.GetMember(sessid2);
            _userManager.Register(username3, userpass3);
            _userManager.Login(sessid3, username3, userpass3);
            Member mem3 = _userManager.GetMember(sessid3);
            _shopManager.CreateShop(mem1, shop1);
            Shop s = _shopManager.Shops.GetByName(shop1);
            mem1.AppointFounder(s);
            mem1.Appoint(mem2, s, Role.Manager, Permission.Appoint);
            var successEvent = new ManualResetEvent(false);
            var exceptionEvent = new ManualResetEvent(false);
            bool exceptionThrown = false;

            var threads = new List<Thread>
            {
                new Thread(() =>
                {
                    try
                    {
                        mem1.Appoint(mem3, s, Role.Manager, Permission.Appoint);
                        if (!exceptionThrown)
                        {
                            successEvent.Set();
                        }
                    }
                    catch (Exception)
                    {
                        if (!exceptionThrown)
                        {
                            exceptionThrown = true;
                            exceptionEvent.Set();
                        }
                    }
                }),
                new Thread(() =>
                {
                    try
                    {
                        mem2.Appoint(mem3, s, Role.Manager, Permission.Appoint);
                        if (!exceptionThrown)
                        {
                            successEvent.Set();
                        }
                    }
                    catch (Exception)
                    {
                        if (!exceptionThrown)
                        {
                            exceptionThrown = true;
                            exceptionEvent.Set();
                        }
                    }
                })
            };
            threads.ForEach(t => t.Start());
            WaitHandle.WaitAny(new[] { successEvent, exceptionEvent });
            if (successEvent.WaitOne(0) && exceptionEvent.WaitOne(0))
            {
                Assert.Fail("Both success and exception occurred.");
            }
            else if (successEvent.WaitOne(0))
            {
                Assert.IsTrue(true);
            }
            else if (exceptionEvent.WaitOne(0))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail("Test did not complete successfully.");
            }
            threads.ForEach(t => t.Join());
        }
    }
}