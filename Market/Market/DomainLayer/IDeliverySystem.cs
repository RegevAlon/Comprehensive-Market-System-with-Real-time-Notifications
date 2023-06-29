using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public interface IDeliverySystem
    {
        int OrderDelivery(ShoppingCartPurchase basket,DeliveryDetails deliveryDetails);
        int CancelOrder(int orderNum);
        bool Connect();
    }
}
