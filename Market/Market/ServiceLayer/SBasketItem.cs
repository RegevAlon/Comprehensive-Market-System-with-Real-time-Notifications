using Market.DomainLayer;
using System;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SBasketItem
    {
        public SProduct product { get; set; }
        public double priceAfterDiscount { get; set; }
        public int quantity { get; set; }

        public SBasketItem(BasketItem basketItem)
        {
            product = new SProduct(basketItem.Product);
            priceAfterDiscount = basketItem.PriceAfterDiscount;
            quantity = basketItem.Quantity;
        }
    }
}