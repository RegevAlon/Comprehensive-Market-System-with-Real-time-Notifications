using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ProductRatingFilter : FilterSearchType
    {
        private int _lowRate;
        private int _highRate;
        public ProductRatingFilter(int lowRate, int highRate)
        {
            _lowRate = lowRate;
            _highRate = highRate;
        }

        protected override bool Predicate(Product product)
        {
            return (product.Price >= _lowRate) && (product.Price <= _highRate);
        }
    }
}
