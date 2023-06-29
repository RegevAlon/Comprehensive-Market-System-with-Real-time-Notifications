using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProxyDeliverySystem : IDeliverySystem
    {
        private int _orderId;
        public ProxyDeliverySystem()
        {
            _orderId = 0;
        }

        public int CancelOrder(int orderNum)
        {
            return 0;
        }

        public bool Connect()
        {
            return true;
        }

        public int OrderDelivery(ShoppingCartPurchase purchase, DeliveryDetails deliveryDetails)
        {
            return _orderId++;
        }
    }
}
