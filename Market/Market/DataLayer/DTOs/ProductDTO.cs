using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Products")]
    public class ProductDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }
        public string SellMethod { get; set; }
        public List<BidDTO>? Bids { get; set; }

        public ProductDTO(int id, string name, double price, int quantity, string category, string description, string keywords, List<ReviewDTO> reviews,string sellMethod, List<BidDTO> bids)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            Description = description;
            Keywords = keywords;
            Reviews = reviews;
            SellMethod = sellMethod;
            Bids = bids;
        }
        public ProductDTO(int id, string name, double price, int quantity, string category, string description, string keywords, List<ReviewDTO> reviews)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
            Description = description;
            Keywords = keywords;
            Reviews = reviews;
            SellMethod = "RegularSell";
            Bids = new List<BidDTO>();
        }

        public ProductDTO() { }
        public ProductDTO(Product product) {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            Quantity = product.Quantity;
            Category = product.Category.ToString();
            Description = product.Description;
            string k = string.Join(", ", product.Keywords.ToArray<string>());
            Keywords = k;
            Reviews = new List<ReviewDTO>();
            foreach(Review review in product.Reviews) { Reviews.Add(new ReviewDTO(review)); }
            Bids = new List<BidDTO>();
            SellMethod = product.SellMethod.GetType().Name;
            if(product.SellMethod is BidSell)
            {
                foreach(Bid bid in ((BidSell)product.SellMethod).Bids.Values)
                {
                    Bids.Add(new BidDTO(bid));
                }
            }

        }
    }

}
