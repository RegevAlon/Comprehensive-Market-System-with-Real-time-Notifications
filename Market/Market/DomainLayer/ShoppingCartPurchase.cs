using Market.DataLayer;
using Market.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ShoppingCartPurchase
    {
        int _id;
        private int _buyerId;
        private SynchronizedCollection<Purchase> _shopPurchaseObjects;
        double _price;
        private PurchaseStatus _purchaseStatus;
        int _paymentId;
        int _deliveryId;
        public ShoppingCartPurchase(int id, int buyerId, SynchronizedCollection<Purchase> shopPurchaseObjects)
        {
            _id = id;
            _buyerId = buyerId;
            _shopPurchaseObjects = shopPurchaseObjects;
            _price = GetPrice();
            _purchaseStatus = PurchaseStatus.Pending;
            _paymentId = -1;
            _deliveryId = -1;
        }

        private double GetPrice()
        {
            double price = 0;
            foreach (Purchase purchase in _shopPurchaseObjects)
                price += purchase.Price;
            return price;
        }

        public ShoppingCartPurchase(int id,Purchase shopPurchaseObjects)
        {
            _id = id;
            _buyerId = shopPurchaseObjects.BuyerId;
            _shopPurchaseObjects = new SynchronizedCollection<Purchase>() { shopPurchaseObjects };
            _price = shopPurchaseObjects.Price;
        }

        public ShoppingCartPurchase(ShoppingCartPurchaseDTO spDTO)
        {
            _id = spDTO.Id;
            _buyerId = MarketContext.GetInstance().Members.Where((m) => m.ShoppingCartPurchases
                .Any(sp => sp.Id == spDTO.Id)).FirstOrDefault().Id;
            _deliveryId = spDTO.DeliveryId;
            _shopPurchaseObjects = new SynchronizedCollection<Purchase>();
            List<PurchaseDTO> shopPurchases = MarketContext.GetInstance().ShoppingCartPurchases.Find(spDTO.Id).ShopsPurchases;
            if (shopPurchases == null) { shopPurchases = new List<PurchaseDTO>(); }
            foreach (PurchaseDTO purchaseDTO in shopPurchases) _shopPurchaseObjects.Add(new Purchase(purchaseDTO));
            _paymentId = spDTO.PaymentId;
            _price = spDTO.Price;
            Enum.TryParse<PurchaseStatus>(spDTO.PurchaseStatus, out _purchaseStatus);

        }

        public int Id { get => _id; }
        public double Price { get => _price; }

        public SynchronizedCollection<Purchase> ShopPurchaseObjects { get => _shopPurchaseObjects; set => _shopPurchaseObjects = value; }
        public int BuyerId { get => _buyerId; set => _buyerId = value; }
        public PurchaseStatus PurchaseStatus { get => _purchaseStatus; set => _purchaseStatus = value; }
        public int PaymentId { get => _paymentId; set => _paymentId = value; }
        public int DeliveryId { get => _deliveryId; set => _deliveryId = value; }
    }
}