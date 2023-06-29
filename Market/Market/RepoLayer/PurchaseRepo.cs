using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Market.RepoLayer
{
    public class PurchaseRepo : IRepo<Purchase>
    {
        private static Dictionary<int, Purchase> _purchaseById;

        private static PurchaseRepo _purchaseRepo = null;
        private object _lock = new object();


        private PurchaseRepo()
        {
            _purchaseById = new Dictionary<int, Purchase>();
        }
        public static PurchaseRepo GetInstance()
        {
            if (_purchaseRepo == null)
                _purchaseRepo = new PurchaseRepo();
            return _purchaseRepo;
        }
        public void Add(Purchase item)
        {
            _purchaseById.Add(item.Id, item);
            MarketContext context = MarketContext.GetInstance();
            ShopDTO shop = context.Shops.Include(s => s.Purchases).FirstOrDefault(s => s.Id == item.ShopId);
            shop.Purchases.Add(new PurchaseDTO(item));
            MarketContext.GetInstance().SaveChanges();
        }

        public bool ContainsID(int id)
        {
            throw new NotImplementedException();
        }

        public bool ContainsValue(Purchase item)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Purchase> GetAll()
        {
            UploadPurchasesFromContext();
            return _purchaseById.Values.ToList();
        }
        private void UploadPurchasesFromContext()
        {
            lock (_lock)
            {
                foreach (PurchaseDTO purchaseDTO in MarketContext.GetInstance().Purchases)
                {
                    _purchaseById.TryAdd(purchaseDTO.Id, new Purchase(purchaseDTO));
                }
            }
        }

        public Purchase GetById(int id)
        {
            if (_purchaseById.ContainsKey(id))
                return _purchaseById[id];
            else
            {
                lock (_lock)
                {
                    PurchaseDTO purchaseDTO = MarketContext.GetInstance().Purchases.Find(id);
                    if (purchaseDTO != null)
                    {
                        _purchaseById.Add(id, new Purchase(purchaseDTO));
                    }
                    return _purchaseById[id];
                }
            }
        }

        public void Update(Purchase item)
        {
            _purchaseById[item.Id] = item;
            lock (_lock)
            {
                PurchaseDTO purchaseDTO = MarketContext.GetInstance().Purchases.Find(item.Id);
                purchaseDTO.PurchaseStatus = item.PurchaseStatus.ToString();
                purchaseDTO.Price = item.Price;
                MarketContext.GetInstance().SaveChanges();
            }
        }
        public SynchronizedCollection<Purchase> GetUserShopPurchaseHistory(int userId, int shopId)
        {
            SynchronizedCollection<Purchase> shopPurchases = GetShopPurchaseHistory(shopId);
            SynchronizedCollection<Purchase> result = new SynchronizedCollection<Purchase>();
            foreach (Purchase purchase in shopPurchases)
            {
                if (purchase.Id == userId) result.Add(purchase);
            }
            return result;
        }
        public SynchronizedCollection<Purchase> GetShopPurchaseHistory(int shopId)
        {
            UploadPurchasesFromContext(shopId);
            SynchronizedCollection<Purchase> result = new SynchronizedCollection<Purchase>();
            foreach (Purchase purchase in _purchaseById.Values)
            {
                if (purchase.ShopId == shopId) result.Add(purchase);
            }
            return result;
        }
        private void UploadPurchasesFromContext(int shopId)
        {
            lock (_lock)
            {
                List<PurchaseDTO> lp = MarketContext.GetInstance().Purchases.Where((p) => p.ShopId == shopId).ToList();
                foreach (PurchaseDTO purchaseDTO in lp)
                {
                    _purchaseById.TryAdd(purchaseDTO.Id, new Purchase(purchaseDTO));
                }
            }
        }


        public void Clear()
        {
            _purchaseById.Clear();
        }
        public void ResetDomainData()
        {
            _purchaseById = new Dictionary<int, Purchase>();
        }
    }
}