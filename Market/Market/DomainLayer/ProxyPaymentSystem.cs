using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProxyPaymentSystem : IPaymentSystem
    {
        private int _receiptNumebr;
        public ProxyPaymentSystem()
        {
            _receiptNumebr = 1;
        }
        public bool Connect()
        {
            return true;
        }

        public int CancelPayment(int paymentID)
        {
            return 1;
        }

        public int Pay(ShoppingCartPurchase purchase, PaymentDetails paymentDetails)
        {
            return _receiptNumebr++;
        }
    }
}
