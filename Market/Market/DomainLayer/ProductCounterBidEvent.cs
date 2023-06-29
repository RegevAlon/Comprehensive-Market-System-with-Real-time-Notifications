using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProductCounterBidEvent : Event
    {
        private Bid _bid;
        private Product _product;
        private Shop _shop;
        private double _oldPrice;
        private string _counterBidMember;
        public ProductCounterBidEvent(Shop shop, Product product, Bid bid,string counterBidMember, double oldPrice) : base("Product Counter Bid Event")
        {
            _shop = shop;
            _product = product;
            _bid = bid;
            _oldPrice = oldPrice;
            _counterBidMember = counterBidMember;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Shop: \'{_shop.Name}\', Product name: \'{_product.Name}\', " +
                $"Product ID: '{_product.Id}', Quantity: {_product.Quantity}, " +
                $"Original Price: {_product.Price}, Member Suggested price per one: " +
                $"{_oldPrice}, Bidding member: {_bid.BiddingMember.UserName}, New Counter Price: {_bid.SuggestedPrice}, " +
                $"Bidding Owner: {_counterBidMember}";
        }


    }
}
