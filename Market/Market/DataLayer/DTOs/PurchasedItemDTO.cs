using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Purchesed Items")]
    public class PurchasedItemDTO
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ShopId { get; set; }
        public int Quantity { get; set; }
        public double PriceBeforeDiscount { get; set; }
        public double PriceAfterDiscount { get; set; }
        public PurchasedItemDTO(BasketItem basketItem)
        {
            ProductId = basketItem.Product.Id;
            ShopId = basketItem.Product.ShopId;
            ProductName = basketItem.Product.Name;
            Quantity = basketItem.Quantity;
            PriceBeforeDiscount = basketItem.Product.Price;
            PriceAfterDiscount = basketItem.PriceAfterDiscount;
        }
        public PurchasedItemDTO(BasketItemDTO basketItem, int shopId)
        {
            ProductId = basketItem.Product.Id;
            ShopId = shopId;
            ProductName = basketItem.Product.Name;
            Quantity = basketItem.Quantity;
            PriceBeforeDiscount = basketItem.Product.Price * Quantity;
            PriceAfterDiscount = basketItem.PriceAfterDiscount;
        }

        public PurchasedItemDTO(int productId, string productName, int shopId, int quantity, double priceBeforeDiscount, double priceAfterDiscount)
        {
            ProductId = productId;
            ShopId = shopId;
            ProductName = productName;
            Quantity = quantity;
            PriceBeforeDiscount = priceBeforeDiscount;
            PriceAfterDiscount = priceAfterDiscount;
        }
    }
}