using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class RealPaymentSystem : IPaymentSystem
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
        private readonly string _webAddress = "https://external-systems.000webhostapp.com/";

        public int CancelPayment(int paymentID)
        {
            return RunWithTimeout(() =>
            {
                var postContent = new Dictionary<string, string>
                {
                    { "action_type", "cancel_pay" },
                    { "transaction_id", "12345" }
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

        public int Pay(ShoppingCartPurchase purchase, PaymentDetails paymentDetails)
        {
            return RunWithTimeout(() =>
            {
                var postContent = new Dictionary<string, string>
                {
                    { "action_type", "pay" },
                    { "card_number", $"{paymentDetails.CardNumber}" },
                    { "month", $"{paymentDetails.Month}" },
                    { "year", $"{paymentDetails.Year}" },
                    { "holder", $"{paymentDetails.Holder}" },
                    { "ccv", $"{paymentDetails.Ccv}" },
                    { "id", $"{paymentDetails.Id}" }
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
                    throw new TimeoutException("Connection to Payment System exceeded the specified timeout.");
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