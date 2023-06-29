using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.ServiceLayer;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ShoppingCart
    {
        private int _userId;
        private ConcurrentDictionary<int, Basket> _basketbyShop;
        private int _purchaseIdFactory;

        public int UserId { get => _userId; }

        public int PurchaseIdFactory { get; }
        public ConcurrentDictionary<int, Basket> BasketbyShop { get => _basketbyShop; }

        public ShoppingCart(int userId)
        {
            _basketbyShop = new ConcurrentDictionary<int, Basket>();
            _userId = userId;
            _purchaseIdFactory = 1;
        }

        public ShoppingCart(ShoppingCartDTO shoppingCart)
        {
            _basketbyShop = new ConcurrentDictionary<int, Basket>();
            _userId = shoppingCart.Id;
            _purchaseIdFactory = shoppingCart.PurchaseIdFactory;
            foreach(BasketDTO basketDTO in shoppingCart.Baskets)
            {
                _basketbyShop.TryAdd(basketDTO.ShopId, new Basket(basketDTO, shoppingCart.Id));
            }
        }

        public void AddProduct(Shop shop, int productId, int quantity)
        {
            if (!_basketbyShop.ContainsKey(shop.Id))
            {
                _basketbyShop[shop.Id] = new Basket(_userId, shop);
            }
            _basketbyShop[shop.Id].AddProductRequest(productId, quantity);
        }
        public void RemoveProduct(int shopId, int productId)
        {
            if (_basketbyShop.ContainsKey(shopId))
            {
                _basketbyShop[shopId].RemoveProduct(productId);
                if (_basketbyShop[shopId].BasketItems.Count() == 0)
                    _basketbyShop.TryRemove(shopId, out Basket removed);
            }
            else throw new Exception("No Basket for this shop in your shoppingCart");
        }
        public double GetPrice()
        {
            double totalPrice = 0;
            foreach (Basket b in _basketbyShop.Values)
            {
                totalPrice += b.GetBasketPrice();
            }
            return totalPrice;
        }

        private int GenerateUniqueId()
        {
            return int.Parse($"{_userId}{_purchaseIdFactory++}");
        }

        public ShoppingCartPurchase Purchase(int shopId)
        {
            if (_basketbyShop.ContainsKey(shopId))
            {
                Purchase shopPendingPurchase = _basketbyShop[shopId].Purchase(_userId);
                ShoppingCartPurchase shoppingCartPendingPurchase = new ShoppingCartPurchase(GenerateUniqueId(), shopPendingPurchase);
                return shoppingCartPendingPurchase;
            }
            else throw new Exception("No basket for this shop");

        }

        public void RemoveBasket(int shopId)
        {
            _basketbyShop.TryRemove(shopId, out Basket removed);
            if(removed!=null)
                removed.Clean();
        }

        /// <summary>
        /// create a special basket the contain all the info of the shoppingCart baskets.
        /// create a special purchase the contain the special basket.
        /// </summary>
        /// <returns></returns> A list of all the purchases objects for each shop and the special purchase as the last element
        public ShoppingCartPurchase PurcaseShoppingCart()
        {
            ShoppingCartPurchase shoppingCartPurchase;
            SynchronizedCollection<Purchase> shopPurchaseList = new SynchronizedCollection<Purchase>();
            foreach (Basket basket in _basketbyShop.Values)
            {
                shopPurchaseList.Add(basket.Purchase(_userId));
            }
            if (shopPurchaseList.Count == 0)
                throw new Exception("Youre ShoppinCart is Empty");
            shoppingCartPurchase = new ShoppingCartPurchase(GenerateUniqueId(), _userId, shopPurchaseList);
            return shoppingCartPurchase;
        }
        public void PurchaseSuccessHandler()
        {
            _basketbyShop.Clear();
        }

        public void UpdateBasketItemQuantity(int shopId, int productID, int quantity)
        {
            if (_basketbyShop.ContainsKey(shopId))
            {
                if (quantity == 0)
                {
                   RemoveProduct(shopId,productID);
                }
                else _basketbyShop[shopId].UpdateBasketItemQuantity(productID, quantity);
            }
            else throw new Exception("You do not have a basket to this shop");
        }

        public BasketItem FindBasketItem(int shopId,int productId)
        {
            if (_basketbyShop.ContainsKey(shopId))
            {
                BasketItem basketItem = _basketbyShop[shopId].BasketItems.ToList().Find((bi) => bi.Product.Id == productId);
                if (basketItem == null)
                    throw new Exception($"BasketItem does not exist for product: {productId}");
                return basketItem;
            }
            throw new Exception($"BasketItem does not exist for product: {productId}");
        }

        public bool HasBasketItem(int shopId, int productId)
        {
            if (_basketbyShop.ContainsKey(shopId))
            {
                BasketItem basketItem = _basketbyShop[shopId].BasketItems.ToList().Find((bi) => bi.Product.Id == productId);
                return basketItem != null;
            }
            return false;
        }

        public void AddBidProduct(Shop shop,Bid bid, Product product)
        {
            if (!_basketbyShop.ContainsKey(shop.Id))
            {
                _basketbyShop[shop.Id] = new Basket(_userId, shop);
            }
            _basketbyShop[shop.Id].AddBidProductToCart(bid, product);
        }
        public int getShoppingCartProductAmount()
        {
            int totalamount = 0;
            foreach(Basket basket in  _basketbyShop.Values)
            {
                if(basket.BasketItems!= null)
                {
                    totalamount += basket.BasketItems.Count;

                }
            }
            return totalamount;
        }
    }
}
