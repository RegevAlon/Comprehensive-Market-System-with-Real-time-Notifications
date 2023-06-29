using Market.DomainLayer;
using Market.ServiceLayer;
using ServerMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Tests.InitAndConfig
{
    [TestClass]
    public class InitAndConfigTest
    {
        UserManager UM;
        ShopManager SM;
        string PrimarysessionID;
        string sessionID_Owner;
        int shopID;
        int productID;
        int quantity;
        int productID2;
        int price;
        SynchronizedCollection<String> keywords;
        MarketManager MM;
        private MarketService service;
        [TestInitialize]
        public void Setup()
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            UpdateInitFileName("InitFileName", "InitFile1.json");
            MarketService.GetInstance().Dispose();
            Task task = Task.Run(() =>
            {
                // Code before disposing the MarketContext instance
                MarketContext.GetInstance().Dispose();
                // Code after disposing the MarketContext instance
            });
            task.Wait();
            UM = UserManager.GetInstance();
            SM = ShopManager.GetInstance();
            MM = MarketManager.GetInstance();

            var mockDeliverySystem = new Mock<IDeliverySystem>();
            var mockPaymentSystem = new Mock<IPaymentSystem>();
            mockDeliverySystem.Setup(d => d.Connect())
                .Returns(true);
            mockPaymentSystem.Setup(d => d.Connect())
                .Returns(true);
            MM.DeliverySystem = mockDeliverySystem.Object;
            MM.PaymentSystem = mockPaymentSystem.Object;

        }
        [TestMethod]
        public void tryToDoRegularOperationsAfterInit()
        {
            new HandleConfigurationFile().Parse();
            service = MarketService.GetInstance();
            Assert.IsFalse(service.GetShopInfo("2", 1).ErrorOccured);
            Assert.IsFalse(service.GetShopInfo("2", 1).ErrorOccured);
            Assert.IsFalse(service.Register("100", "nadavking", "pass123123").ErrorOccured);
            Assert.IsFalse(service.Login("100", "nadavking", "pass123123").ErrorOccured);
            Assert.IsFalse(service.CreateShop("100", "shop3").ErrorOccured);
            Assert.IsFalse(service.GetMarketInfo("100").ErrorOccured);


        }

        [TestCleanup]
        public void Cleanup()
        {
            UpdateInitFileName("ShouldRunInitFile", "false");
        }
        [TestMethod]
        public void tryToInitialzeWithWrongDetailsRegisterWithExistUserName()
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            new HandleConfigurationFile().Parse();
            service = MarketService.GetInstance();
            Assert.IsTrue(service.Register("15", "u3", "password15").ErrorOccured);
            Assert.IsTrue(service.Register("16", "u4", "password4").ErrorOccured);
            service.Dispose();

        }
        [TestMethod]
        public void GoOverAllUsersAndCheckIfTheyInTheSystem()//this test need to be changed if we change InitFile1
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            new HandleConfigurationFile().Parse();
            service = MarketService.GetInstance();
            Assert.IsFalse(service.Login("1", "MasterAdmin", "MasterAdmin").ErrorOccured);
            List<string> ls = service.GetAllMembers("1").Value;
            List<string> listofNames = new List<string>() { "u2", "u3", "u4", "u5", "MasterAdmin" };
            foreach (string memName in ls)
            {
                bool flag = false;
                foreach (string name in listofNames)
                {
                    if (name == memName)
                        flag = true;
                }

                if (!flag)
                {
                    Assert.Fail();
                    break;
                }
            }

        }
        [TestMethod]
        public void FailureTestForInit()//for this test to be succesfull we need to change the config file
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            bool excatch = false;
            //TRY TO CRATE A SHOP WITH INVALID SESSIONid
            UpdateInitFileName("InitFileName", "InitFile2.json");
            try
            {
                new HandleConfigurationFile().Parse();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("CreateShop"))
                {
                    Assert.IsTrue(true);
                    excatch = true;
                }

                else
                    Assert.Fail("Unable to chatch the right error");
            }
            finally
            {
                if (!excatch)
                    Assert.Fail("Unable to catch error with parsing wrong init");
                UpdateInitFileName("InitFileName", "InitFile1.json");
            }


        }
        [TestMethod]
        public void checkConfigFileSucces()
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            bool excatch = false;
            UpdatekeyName("InitFileName", "InitFileNamee");
            try
            {
                new HandleConfigurationFile().Parse();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Wrong Config File structure"))
                {
                    Assert.IsTrue(true);
                    excatch = true;
                }

                else
                    Assert.Fail("Unable to chatch the right error" + ex.Message);
            }
            finally
            {
                if (!excatch)
                    Assert.Fail("Unable to catch error with parsing wrong init");
                UpdatekeyName("InitFileNamee", "InitFileName");
            }




        }
        [TestMethod]
        public void checkConfigFileFailure()
        {
            UpdateInitFileName("ShouldRunInitFile", "true");
            bool excatch = false;
            UpdateInitFileName("InitFileName", "notexist.json");
            try
            {
                new HandleConfigurationFile().Parse();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Could not find file "))
                {
                    Assert.IsTrue(true);
                    excatch = true;
                }

                else
                    Assert.Fail("Unable to chatch the right error" + ex.Message);
            }
            finally
            {
                if (!excatch)
                    Assert.Fail("Unable to catch error with parsing wrong init");
                UpdateInitFileName("InitFileName", "InitFile1.json");
            }

        }

        public void UpdateInitFileName(string key, string newValue)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "ConfigurationAndInit\\MarketConfig.json");
            try
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(filePath);

                // Parse the JSON content
                JObject jsonObject = JObject.Parse(jsonString);

                // Find the property with the specified key
                JToken token = jsonObject.SelectToken(key);

                if (token != null)
                {
                    if (newValue.Equals("true"))
                        // Update the value
                        token.Replace(true);
                    else if (newValue.Equals("false"))
                        token.Replace(false);
                    else
                        token.Replace(newValue);
                    // Write the modified JSON back to the file
                    File.WriteAllText(filePath, jsonObject.ToString());
                    Console.WriteLine("Value updated successfully.");
                }
                else
                {
                    Console.WriteLine("Key not found in the JSON file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public void UpdatekeyName(string key, string newKey)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "ConfigurationAndInit\\MarketConfig.json");
            try
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(filePath);

                // Parse the JSON content
                JObject jsonObject = JObject.Parse(jsonString);

                // Find the property with the specified key
                JToken token = jsonObject.SelectToken(key);

                if (token != null)
                {
                    // Replace the key with the new key
                    jsonObject[newKey] = token;
                    jsonObject.Remove(key);

                    // Write the modified JSON back to the file
                    File.WriteAllText(filePath, jsonObject.ToString());
                    Console.WriteLine("Key updated successfully.");
                }
                else
                {
                    Console.WriteLine("Key not found in the JSON file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


    }
}
