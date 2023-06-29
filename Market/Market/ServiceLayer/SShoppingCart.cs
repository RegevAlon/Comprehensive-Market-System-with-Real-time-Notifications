using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SShoppingCart
    {
        public double totalPrice { get; set; }
        public List<SBasket> cart { get; set; }

        public SShoppingCart(ShoppingCart shoppingcart)
        {
            totalPrice = shoppingcart.GetPrice();
            cart = new List<SBasket>();
            foreach (Basket b in shoppingcart.BasketbyShop.Values)
            {
                this.cart.Add(new SBasket(b));
            }
        }
    }
}
