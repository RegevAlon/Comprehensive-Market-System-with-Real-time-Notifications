using Market.DomainLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SProduct
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public string category { get; set; }
        public int shopId { get; set; }
        public List<string> keywords { get; set; }
        public List<string> reviews { get; set; }
        public double rate { get; set; }
        public List<SBid> bids { get; set; }
        public int sellType { get; set; } // regular or bid

        public SProduct(Product product)
        {
            id = product.Id;
            name = product.Name;
            description = product.Description;
            price = product.Price;
            shopId = product.ShopId;
            quantity = product.Quantity;
            category = product.Category.ToString();
            keywords = product.Keywords.ToList();
            reviews = new List<string>();
            foreach (Review review in product.Reviews)
            {
                reviews.Add(review.Comment);
            }
            
            rate = product.GetRate();
            bids = new List<SBid>();
            if (product.SellMethod is BidSell)
            {
                this.sellType = 1;
                BidSell bidSell = (BidSell)product.SellMethod;
                foreach (Bid bid in bidSell.Bids.Values)
                {
                    bids.Add(new SBid(bid));
                }
            }
            else
            {
                this.sellType = 0;
            }
        }

        public SProduct(int id, string name, string description, double price, int quantity, string category, List<string> keywords)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.price = price;
            this.quantity = quantity;
            this.category = category;
            this.keywords = keywords;
        }

    }
}