using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProductSellEvent : Event
    {
        private BasketItem _basketItem;
        private Shop _shop;
        public ProductSellEvent(Shop shop,BasketItem product) : base("Product Sell Event")
        {
            _shop = shop;
            _basketItem = product;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Shop: \'{_shop.Name}\', Product name: \'{_basketItem.Product.Name}\', " +
                $"Product ID: '{_basketItem.Product.Id}', Quantity: {_basketItem.Quantity}, Total Price: " +
                $"{_basketItem.PriceAfterDiscount*_basketItem.Quantity}";
        }


    }
}
