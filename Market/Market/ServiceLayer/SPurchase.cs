using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SPurchase
    {
        public int id { get; set; }
        public double price { get; set; }
        public string purchaseStatus { get; set; }

        public SPurchase(Purchase purchase)
        {
            id = purchase.Id;
            price = purchase.Price;
            purchaseStatus = purchase.PurchaseStatus.ToString();
        }

        public SPurchase(int id, double price, SBasket basket, string purchaseStatus)
        {
            this.id = id;
            this.price = price;
            this.purchaseStatus = purchaseStatus;
        }
    }
}
