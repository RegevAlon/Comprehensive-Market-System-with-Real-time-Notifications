using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class CategoryFilter : FilterSearchType
    {
        private Category _category;
        public CategoryFilter(string category)
        {
            _category = TryCastCategory(category);
        }

        protected override bool Predicate(Product product)
        {
            return (product.Category&_category)==_category;
        }
        private Category TryCastCategory(string category)
        {
            try
            {
                Category categoryToSearch = (Category)Enum.Parse(typeof(Category), category);
                return categoryToSearch;
            }
            catch (Exception)
            {
                return Category.None;
            }
        }
    }
}
