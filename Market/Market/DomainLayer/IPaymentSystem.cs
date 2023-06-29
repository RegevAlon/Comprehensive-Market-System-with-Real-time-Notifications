using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public interface IPaymentSystem
    {
        int Pay(ShoppingCartPurchase purchase,PaymentDetails paymentDetails);
        int CancelPayment(int paymentID);
        bool Connect();
    }
}
