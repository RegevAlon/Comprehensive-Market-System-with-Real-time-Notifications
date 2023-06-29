using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("ShoppingCart")]
    public class ShoppingCartDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int PurchaseIdFactory { get; set; }
        public List<BasketDTO> Baskets { get; set; }

        public ShoppingCartDTO(int id, int purchaseIdFactory, List<Basket> baskets)
        {
            Id = id;
            PurchaseIdFactory = purchaseIdFactory;
            Baskets = new List<BasketDTO>();
            foreach (Basket basket in baskets)
                Baskets.Add(new BasketDTO(basket));
        }
        public ShoppingCartDTO(int id)
        {
            Id = id;
            PurchaseIdFactory = 0;
            Baskets = new List<BasketDTO>();
        }
        public ShoppingCartDTO(){ }
        public ShoppingCartDTO(ShoppingCart cart) {
            Id = cart.UserId;
            PurchaseIdFactory = cart.PurchaseIdFactory;
            Baskets = new List<BasketDTO>();
            foreach (Basket basket in cart.BasketbyShop.Values)
                Baskets.Add(new BasketDTO(basket));
        }
    }
}
