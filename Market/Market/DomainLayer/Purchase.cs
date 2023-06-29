using Market.DataLayer.DTOs;
using Market.RepoLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Purchase
    {
        private int _id;
        private int _shopId;
        private int _buyerId;
        private Basket _basket;
        double _price;
        private PurchaseStatus _purchaseStatus;

        public int Id { get => _id; }
        public int ShopId { get => _shopId; }
        public int BuyerId { get => _buyerId; }
        public double Price { get => _price; }
        public Basket Basket { get => _basket; }
        public PurchaseStatus PurchaseStatus { get => _purchaseStatus; set => _purchaseStatus = value; }
        public Purchase(int id, int shopId, int buyerId, Basket basket)
        {
            _id = id;
            _shopId = shopId;
            _buyerId = buyerId;
            _basket = basket;
            _price = basket.GetBasketPrice();
            _purchaseStatus = PurchaseStatus.Pending;
        }

        public Purchase(PurchaseDTO purchaseDto)
        {
            _id = purchaseDto.Id;
            _buyerId = purchaseDto.BuyerId;
            _shopId = purchaseDto.ShopId;
            _basket = new Basket(_buyerId, ShopRepo.GetInstance().GetById(_shopId));
            foreach (PurchasedItemDTO basketItemDTO in purchaseDto.PurchasedItems)
                _basket.BasketItems.Add(new BasketItem(basketItemDTO));
            _price = purchaseDto.Price;
            Enum.TryParse<PurchaseStatus>(purchaseDto.PurchaseStatus, out _purchaseStatus);
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---------------------------");
            sb.AppendLine(string.Format("Purchase Number: %d", _id));
            sb.AppendLine(string.Format("Buyer ID: %d", _buyerId));
            sb.AppendLine(string.Format("Shop ID: %d", _shopId));
            sb.AppendLine(string.Format("Basket: %s", _basket.GetInfo()));
            sb.AppendLine(string.Format("Purchase Status: %s", _purchaseStatus.ToString()));
            sb.AppendLine("---------------------------");
            return sb.ToString();
        }
    }
}
