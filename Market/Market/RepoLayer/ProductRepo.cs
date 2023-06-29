using Market.DataLayer.DTOs;
using Market.DataLayer;
using Market.DomainLayer;
using Market.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Market.RepoLayer
{
    class ProductRepo : IRepo<Product>
    {
        private static ConcurrentDictionary<int, Product> _productById;

        private static ProductRepo _productRepo = null;
        private object _lock;

        private ProductRepo()
        {
            _productById = new ConcurrentDictionary<int, Product>();
            _lock = new object();
        }
        public static ProductRepo GetInstance()
        {
            if (_productRepo == null)
                _productRepo = new ProductRepo();
            return _productRepo;
        }

        public void Add(Product item)
        {
            _productById.TryAdd(item.Id, item);
            lock (_lock)
            {
                ShopDTO shop = MarketContext.GetInstance().Shops.Include(s => s.Products).FirstOrDefault(s => s.Id == item.ShopId);
                shop.Products.Add(new ProductDTO(item));
                MarketContext.GetInstance().SaveChanges();
            }
        }

        public bool ContainsID(int id)
        {
            if (!_productById.ContainsKey(id))
            {
                lock (_lock)
                {
                    return MarketContext.GetInstance().Products.Find(id) != null;
                }
            }
            return true;
        }

        public bool ContainsValue(Product item)
        {
            if (!_productById.ContainsKey(item.Id))
            {
                lock (_lock)
                {
                    return MarketContext.GetInstance().Products.Find(item.Id) != null;
                }
            }
            return true;
        }

        public void Delete(int id)
        {
            if (!_productById.TryRemove(id, out Product _))
            {
                lock (_lock)
                {
                    ProductDTO productdto = MarketContext.GetInstance().Products.Find(id);
                    MarketContext.GetInstance().Products.Remove(productdto);
                    MarketContext.GetInstance().SaveChanges();
                }
            }
        }

        public List<Product> GetAll()
        {
            List<Shop> shops = ShopRepo.GetInstance().GetAll();
            foreach (Shop s in shops) UploadShopProductsFromContext(s.Id);
            return _productById.Values.ToList();
        }

        public Product GetById(int id)
        {
            if (_productById.ContainsKey(id))
            {
                return _productById[id];
            }
            else
            {
                lock (_lock)
                {
                    ProductDTO productDTO = MarketContext.GetInstance().Products.Find(id);
                    if (productDTO != null)
                    {
                        Product product = new Product(productDTO);
                        _productById.TryAdd(id, product);
                        return product;
                    }
                    else
                    {
                        throw new Exception("Invalid product Id.");
                    }
                }
            }
        }

        public void Update(Product item)
        {
            _productById[item.Id] = item;
            lock (_lock)
            {
                ProductDTO p = MarketContext.GetInstance().Products.Find(item.Id);
                if (p != null)
                {
                    if (item.Description != null) p.Description = item.Description;
                    if (item.Category != null) p.Category = item.Category.ToString();
                    if (item.Keywords != null) p.Keywords = string.Join(", ", item.Keywords);
                    p.Reviews = new List<ReviewDTO>();
                    p.Quantity = item.Quantity;
                    p.Price = item.Price;
                    foreach (Review review in item.Reviews)
                    {
                        ReviewDTO rDto = MarketContext.GetInstance().Reviews.Find(review.Id);
                        if (rDto != null)
                        {
                            rDto.ReviewerUsername = review.User;
                            rDto.Comment = review.Comment;
                            rDto.Rate = review.Rate;
                        }
                        else p.Reviews.Add(new ReviewDTO(review));
                    }
                    MarketContext.GetInstance().SaveChanges();
                }
            }
        }
        /// <summary>
        /// returns all product of a given shop 
        /// </summary>
        /// <param name="shopId"></param> the Id of the shop
        /// <returns></returns>
        public SynchronizedCollection<Product> GetShopProducts(int shopId)
        {
            UploadShopProductsFromContext(shopId);
            SynchronizedCollection<Product> products = new SynchronizedCollection<Product>();
            foreach(Product p in _productById.Values)
            {
                if (p.ShopId == shopId) products.Add(p);
            }
            return products;
        }
        private void UploadShopProductsFromContext(int shopId)
        {
            lock (_lock)
            {
                ShopDTO shop = MarketContext.GetInstance().Shops.Find(shopId);
                if (shop != null)
                {
                    List<ProductDTO> products = MarketContext.GetInstance().Shops.Find(shopId).Products;
                    if (products != null)
                    {
                        foreach (ProductDTO product in products)
                        {
                            _productById.TryAdd(product.Id, new Product(product));
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            _productById.Clear();
        }
        public void ResetDomainData()
        {
            _productById = new ConcurrentDictionary<int, Product>();
        }
    }
}
