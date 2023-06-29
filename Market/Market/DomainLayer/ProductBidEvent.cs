using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProductBidEvent : Event
    {
        private Bid _bid;
        private Product _product;
        private Shop _shop;
        public ProductBidEvent(Shop shop, Product product, Bid bid) : base("Product Bid Event")
        {
            _shop = shop;
            _product = product;
            _bid = bid;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Shop: \'{_shop.Name}\', Product name: \'{_product.Name}\', " +
                $"Product ID: '{_product.Id}', Quantity: {_product.Quantity}, " +
                $"Original Price: {_product.Price}, Suggested price per one: " +
                $"{_bid.SuggestedPrice}, Bidding member: {_bid.BiddingMember.UserName}";
        }


    }
}
