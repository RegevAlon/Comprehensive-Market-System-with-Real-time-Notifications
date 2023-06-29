using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class RegularSell : ISell
    {
        public bool CanAddToCart()
        {
            return true;
        }

        public bool CanBid()
        {
            throw new Exception("Cannot bid on this product.");
        }
    }
}
