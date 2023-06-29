using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using Market.ServiceLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Baskets")]
    public class BasketDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("ShopDTO")]
        public int ShopId { get; set; }
        public List<BasketItemDTO> BasketItems { get; set; }

        public BasketDTO(int shopId, List<BasketItemDTO> basketItems)
        {
            ShopId = shopId;
            BasketItems = basketItems;
        }

        public double TotalPrice { get; set; }
        public BasketDTO() { }
        public BasketDTO(Basket basket) {
            ShopId = basket.Shop.Id;
            //ShoppingCartId = basket.ShoppingCartId;
            BasketItems = new List<BasketItemDTO>();
            foreach (BasketItem item in basket.BasketItems)
                BasketItems.Add(new BasketItemDTO(item));
            TotalPrice = basket.TotalPrice;
        }

    }
}
