using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public abstract class FilterSearchType
    {
        public void Filter(HashSet<Product> products)
        {
            HashSet<Product> filteredProducts = new HashSet<Product>();
            foreach (Product product in products)
            {
                if (Predicate(product))
                {
                    filteredProducts.Add(product);
                }
            }
            products.Clear();
            products.UnionWith(filteredProducts);
        }
        protected abstract bool Predicate(Product product);
    }
}
