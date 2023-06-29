using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using System.Security.Cryptography;
using Market.ServiceLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Purchases")]
    public class PurchaseDTO
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ShopId { get; set; }
        public List<PurchasedItemDTO> PurchasedItems { get; set; }
        public int BuyerId { get; set; }
        public double Price { get; set; }
        public string PurchaseStatus { get; set; }

       
        public PurchaseDTO() { }
        public PurchaseDTO(Purchase purchase) {
            Id = purchase.Id;
            ShopId = purchase.ShopId;
            PurchasedItems = new List<PurchasedItemDTO>();
            foreach (BasketItem item in purchase.Basket.BasketItems)
                PurchasedItems.Add(new PurchasedItemDTO(item));
            BuyerId = purchase.BuyerId;
            Price = purchase.Price;
            PurchaseStatus = purchase.PurchaseStatus.ToString();
        }

        public PurchaseDTO(int id, int shopId, List<BasketItem> items, int buyerId, double price, string purchaseStatus)
        {
            Id = id;
            ShopId = shopId;
            PurchasedItems = new List<PurchasedItemDTO>();
            foreach (BasketItem item in items)
                PurchasedItems.Add(new PurchasedItemDTO(item));
            BuyerId = buyerId;
            Price = price;
            PurchaseStatus = purchaseStatus;
        }

        public PurchaseDTO(int id, int shopId, List<BasketItemDTO> items, int buyerId, double price, string purchaseStatus)
        {
            Id = id;
            ShopId = shopId;
            PurchasedItems = new List<PurchasedItemDTO>();
            foreach (BasketItemDTO item in items)
                PurchasedItems.Add(new PurchasedItemDTO(item, shopId));
            BuyerId = buyerId;
            Price = price;
            PurchaseStatus = purchaseStatus;
        }
    }
}
