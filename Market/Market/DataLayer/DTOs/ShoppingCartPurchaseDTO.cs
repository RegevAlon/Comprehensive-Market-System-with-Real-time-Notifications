using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using Market.RepoLayer;

namespace Market.DataLayer.DTOs
{
    [Table("ShoppingCart Purhcase")]
    public class ShoppingCartPurchaseDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<PurchaseDTO> ShopsPurchases { get; set; }
        public double Price { get; set; }
        public string PurchaseStatus { get; set; }
        public int DeliveryId { get; set; }
        public int PaymentId { get; set; }

        public ShoppingCartPurchaseDTO(int id, List<PurchaseDTO> shopsPurchases, double price, string purchaseStatus, int deliveryId, int paymentId)
        {
            Id = id;
            ShopsPurchases = shopsPurchases;
            Price = price;
            PurchaseStatus = purchaseStatus;
            DeliveryId = deliveryId;
            PaymentId = paymentId;
        }
        public ShoppingCartPurchaseDTO() { }
        public ShoppingCartPurchaseDTO(ShoppingCartPurchase purchase) {
            Id = purchase.Id;
            ShopsPurchases = new List<PurchaseDTO>();
            foreach(Purchase purchaseDTO in purchase.ShopPurchaseObjects)
                ShopsPurchases.Add(MarketContext.GetInstance().Purchases.Find(purchaseDTO.Id));
            Price = purchase.Price;
            PurchaseStatus = purchase.PurchaseStatus.ToString();
            DeliveryId = purchase.DeliveryId;
            PaymentId = purchase.PaymentId;
        }
    }
}
