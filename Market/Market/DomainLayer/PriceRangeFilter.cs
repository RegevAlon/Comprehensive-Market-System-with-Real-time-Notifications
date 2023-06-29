using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class PriceRangeFilter : FilterSearchType
    {
        private int _lowPrice;
        private int _highPrice;

        public PriceRangeFilter(int lowPrice, int highPrice)
        {
            _lowPrice = lowPrice;
            _highPrice = highPrice;
        }

        protected override bool Predicate(Product product)
        {
            return (product.Price >= _lowPrice) && (product.Price <= _highPrice);
        }
    }
}
