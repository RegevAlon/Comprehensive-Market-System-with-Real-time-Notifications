using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class RealDeliverSystem : IDeliverySystem
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
        private readonly string _webAddress = "https://external-systems.000webhostapp.com/";

        public int CancelOrder(int orderNum)
        {
            return RunWithTimeout(() =>
            {
                var postContent = new Dictionary<string, string>
                {
                    { "action_type", "cancel_supply" },
                    { "transaction_id", $"{orderNum}" }
                };

                using (HttpClient client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(postContent);
                    HttpResponseMessage response = client.PostAsync(_webAddress, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        int cancellationStatus;
                        if (int.TryParse(responseBody, out cancellationStatus))
                        {
                            if (cancellationStatus == 1)
                            {
                                return cancellationStatus;
                            }
                        }
                    }

                    return -1;
                }
            });
        }

        public bool Connect()
        {
            return RunWithTimeout(() =>
            {
                using (HttpClient client = new HttpClient())
                {
                    var postContent = new Dictionary<string, string>
                    {
                        { "action_type", "handshake" }
                    };

                    var content = new FormUrlEncodedContent(postContent);
                    HttpResponseMessage response = client.PostAsync(_webAddress, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        if (responseBody.Equals("OK"))
                            return true;
                    }

                    return false;
                }
            });
        }

        public int OrderDelivery(ShoppingCartPurchase basket, DeliveryDetails deliveryDetails)
        {
            return RunWithTimeout(() =>
            {
                var postContent = new Dictionary<string, string>
                {
                    { "action_type", "supply" },
                    { "name", $"{deliveryDetails.Name}" },
                    { "address", $"{deliveryDetails.Address}" },
                    { "city", $"{deliveryDetails.City}" },
                    { "country", $"{deliveryDetails.Country}" },
                    { "zip", $"{deliveryDetails.Zip}" }
                };

                using (HttpClient client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(postContent);
                    HttpResponseMessage response = client.PostAsync(_webAddress, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        int transactionId;
                        if (int.TryParse(responseBody, out transactionId))
                        {
                            if (transactionId >= 10000 && transactionId <= 100000)
                                return transactionId;
                        }
                    }

                    return -1;
                }
            });
        }

        private T RunWithTimeout<T>(Func<T> function, TimeSpan timeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Task<T> task = Task.Run(function, cancellationTokenSource.Token);
                Task completedTask = Task.WhenAny(task, Task.Delay(timeout, cancellationTokenSource.Token)).Result;

                if (completedTask != task)
                {
                    cancellationTokenSource.Cancel();
                    throw new TimeoutException("Connection to Delivery System exceeded the specified timeout.");
                }

                return task.Result;
            }
        }

        private T RunWithTimeout<T>(Func<T> function)
        {
            return RunWithTimeout(function, DefaultTimeout);
        }
    }
}