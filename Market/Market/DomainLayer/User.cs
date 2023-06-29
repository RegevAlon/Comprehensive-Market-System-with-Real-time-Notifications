using Market.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public abstract class User
    {
        private int _id;
        private ShoppingCart _shoppingCart;
        public int Id { get => _id; }
        public ShoppingCart ShoppingCart { get => _shoppingCart; set => _shoppingCart = value; }

        public User(int id)
        {
            _id = id;
            _shoppingCart = new ShoppingCart(_id);
        }

        public ShoppingCartPurchase Purchase(int shopId)
        {
            ShoppingCartPurchase pendingPurchase = _shoppingCart.Purchase(shopId);
            return pendingPurchase;
        }

        public virtual void AddToCart(Shop shop, int productId, int quantity)
        {
            ShoppingCart.AddProduct(shop, productId, quantity);
        }

        public virtual void RemoveFromCart(int shopId, int productId)
        {
            _shoppingCart.RemoveProduct(shopId, productId);
        }

        public ShoppingCart GetShoppingCartInfo()
        {
            return _shoppingCart;
        }

        public ShoppingCartPurchase Purchase(int sessionID, int shopId)
        {
            ShoppingCartPurchase pendingPurchase = ShoppingCart.Purchase(shopId);
            return pendingPurchase;
        }

        public virtual void RemoveBasketFromCart(int shopId)
        {
            _shoppingCart.RemoveBasket(shopId);
        }

        public virtual ShoppingCartPurchase PurchaseShoppingCart()
        {
            return _shoppingCart.PurcaseShoppingCart();
        }
        public virtual void PurchaseFailHandler(ShoppingCartPurchase pendingPurchase)
        {
            pendingPurchase.PurchaseStatus = PurchaseStatus.Failed;
        }
        public virtual void PurchaseSuccessHandler(ShoppingCartPurchase pendingPurchase)
        {
            pendingPurchase.PurchaseStatus = PurchaseStatus.Success;
            _shoppingCart.PurchaseSuccessHandler();
        }

        public virtual void UpdateBasketItemQuantity(int shopId, int productID, int quantity)
        {
            _shoppingCart.UpdateBasketItemQuantity(shopId,productID,quantity); 
        }
    }
}
