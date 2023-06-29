using Market.DomainLayer;
using Market.RepoLayer;
using Market.ServiceLayer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Market.DataLayer.DTOs
{
    public class BasketItemDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public double PriceBeforeDiscount { get; set; }
        public double PriceAfterDiscount { get; set; }
        public int Quantity { get; set; }

        public BasketItemDTO(ProductDTO product, double priceAfterDiscount, double priceBeforeDiscount, int quantity)
        {
            Product = product;
            PriceBeforeDiscount = priceBeforeDiscount;
            PriceAfterDiscount = priceAfterDiscount;
            Quantity = quantity;
        }
        public BasketItemDTO() { }
        public BasketItemDTO(BasketItem item) {
            Product = MarketContext.GetInstance().Products.Find(item.Product.Id);
            PriceBeforeDiscount = item.Product.Price;
            PriceAfterDiscount = item.PriceAfterDiscount;
            Quantity = item.Quantity;
        }
    }
}
