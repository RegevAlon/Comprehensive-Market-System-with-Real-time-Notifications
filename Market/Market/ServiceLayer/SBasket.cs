using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Data;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SBasket
    {
        public string shopName { get; set; }
        public List<SBasketItem> productsAmount { get; set; }
        public double totalPrice { get; set; }

        public SBasket(Basket basket)
        {
            shopName = basket.Shop.Name;
            productsAmount = new List<SBasketItem>();
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                productsAmount.Add(new SBasketItem(basketItem));
            }
            totalPrice = basket.GetBasketPrice();
        }

        public SBasket(List<SBasketItem> productsAmount, double totalPrice)
        {
            this.productsAmount = productsAmount;
            this.totalPrice = totalPrice;
        }
    }
}