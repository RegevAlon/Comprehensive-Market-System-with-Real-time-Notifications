using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Basket
    {
        private SynchronizedCollection<BasketItem> _basketItems;
        private Shop _shop;
        private int _shoppingCartId;
        private double _totalPrice;

        public SynchronizedCollection<BasketItem> BasketItems { get => _basketItems; set => _basketItems = value; }

        public int ShoppingCartId { get => _shoppingCartId; set => _shoppingCartId = value; }
        public double TotalPrice { get => _totalPrice; set => _totalPrice = value; }
        public Shop Shop { get => _shop;}

        public Basket(int shoppingCartId, Shop shop)
        {
            _shop = shop;
            _shoppingCartId = shoppingCartId;
            _basketItems = new SynchronizedCollection<BasketItem>();
        }
        public Basket(int shoppingCartId, Shop shop, SynchronizedCollection<BasketItem> productAmount)
        {
            _shop = shop;
            _shoppingCartId = shoppingCartId;
            _basketItems = productAmount;
        }

        public Basket(BasketDTO basketDTO,int shoppingCartId)
        {
            _shop = ShopRepo.GetInstance().GetById(basketDTO.ShopId);
            _shoppingCartId = shoppingCartId;
            _basketItems = new SynchronizedCollection<BasketItem>();
            foreach(BasketItemDTO item in basketDTO.BasketItems)
            {
                _basketItems.Add(new BasketItem(item));
            }
        }

        /// <summary>
        /// used by the shop, add the product to the basket
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        public void AddProduct(Product product, int quantity)
        {
            if (!HasProduct(product))
            {
                _basketItems.Add(new BasketItem(product, quantity));
            }
            else throw new Exception("Product already in shoppingcart.");
        }
        /// <summary>
        /// send request to the relevant shop to add the product and <quantity> amount to the basket</quantity> 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        public void AddProductRequest(int productId, int quantity)
        {
            _shop.AddProductToBasket(this,productId, quantity);
        }
        public void RemoveProduct(int productId)
        {
            BasketItem productToRemove = FindBasketItem(productId);
            if (productToRemove != null)
            {
                _basketItems.Remove(productToRemove);
                if (_shop.IsBidSellProduct(productToRemove.Product))
                {
                    _shop.RemoveBid(_shoppingCartId, productToRemove.Product.Id);
                }
            }
            else throw new Exception("No product with this productId in the basket.");
        }
        /// <summary>
        /// calculate all the product prices and return the total price of the basket.
        /// </summary>
        /// <returns></returns>
        public double GetBasketPrice()
        {
            resetDiscount();
            _totalPrice = 0;
            _shop.ApplyDiscountsOnBasket(this); //Will be implemented in version 2
            foreach(BasketItem basketItem in _basketItems)
            {
                double productPrice = basketItem.PriceAfterDiscount;
                int quantity = basketItem.Quantity;
                _totalPrice += productPrice * quantity;
            }
            return _totalPrice;
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Basket for shop %d:", _shop.Id));
            foreach (BasketItem basketItem in _basketItems)
            {
                Product product = basketItem.Product;
                int quantity = basketItem.Quantity;
                sb.Append(product.GetInfo()+string.Format("\t quantity: %d",quantity));
            }
            return sb.ToString();
        }

        public Purchase Purchase(int userId)
        {
           return _shop.PurchaseBasket(userId,this);
        }
        public bool HasProduct(Product p)
        {
            foreach(BasketItem basketItem in _basketItems)
            {
                if(basketItem.Product == p) return true;
            }
            return false;
        }
        public BasketItem FindBasketItem(int productId)
        {
            foreach (BasketItem basketItem in _basketItems)
            {
                if (basketItem.Product.Id == productId) return basketItem;
            }
            return null;
        }

        public Basket Clone()
        {
            SynchronizedCollection<BasketItem> productAmountCopy = new SynchronizedCollection<BasketItem>();
            foreach(BasketItem basketItem in _basketItems)
            {
                productAmountCopy.Add(basketItem.Clone());
            }
            return new Basket(_shoppingCartId, _shop, productAmountCopy);
        }
        public double GetBasketPriceBeforeDiscounts()
        {
            double price = 0;
            foreach (BasketItem basketItem in _basketItems)
                price += basketItem.Product.Price * basketItem.Quantity;
            return price;
        }
        public BasketItem GetBasketItem(Product product)
        {
            foreach(BasketItem basketItem in _basketItems)
            {
                if (basketItem.Product == product)
                {
                    return basketItem;
                }
            }
            return null;
        }
        public void resetDiscount()
        {
            foreach (BasketItem basketItem in _basketItems)
            {
                if(IsRegularSellProduct(basketItem))
                    basketItem.PriceAfterDiscount = basketItem.Product.Price;
            }
        }

        public void UpdateBasketItemQuantity(int productID, int quantity)
        {
            BasketItem basketItem = FindBasketItem(productID);
            if(!IsRegularSellProduct(basketItem))
                throw new Exception("Cannot update basketItem details of Bid product");

            if (basketItem != null && ValidQuantity(quantity) && _shop.CheckInSupply(productID, quantity))
            {
                basketItem.Quantity = quantity;
            }
            else throw new Exception($"Your basket does not contain product with ID: {productID}");
        }
        private bool ValidQuantity(int quantity)
        {
            if (quantity < 0)
                throw new Exception("Invalid Qunatity");
            return true;
        }

        public void AddBidProductToCart(Bid bid, Product product)
        {
            if (!HasProduct(product))
            {
                _basketItems.Add(new BasketItem(bid,product));
            }
            else throw new Exception("Product already in shoppingcart.");
        }
        private bool IsRegularSellProduct(BasketItem basketItem)
        {
            return basketItem.Product.SellMethod is RegularSell;
        }

        public void Clean()
        {
            foreach(BasketItem bi in _basketItems)
            {
                if (_shop.IsBidSellProduct(bi.Product))
                {
                    _shop.RemoveBid(_shoppingCartId, bi.Product.Id);
                }
            }
        }
    }
}
