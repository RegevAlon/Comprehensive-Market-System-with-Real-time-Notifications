using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Market.DomainLayer
{
    public class Search
    {
        public Search()
        {
        }
        public HashSet<Product> ApplySearch(string wordToSearch, SearchType searchType, List<FilterSearchType> filterSearchType, List<Shop> shops)
        {
            HashSet<Product> products = new HashSet<Product>();
            if (NameSearch(searchType))
            {
                products = SearchByName(wordToSearch, shops);
            }
            if (CategorySearch(searchType))
            {
                Category category = TryCastCategory(wordToSearch);
                if (category != Category.None)
                    products.UnionWith(SearchByCatagory(category, shops));
            }
            if (KeywordSearch(searchType))
            {
                products.UnionWith(SearchByKeywords(wordToSearch, shops));
            }

            //Filter the products
            //------------------------------------------------
            foreach (FilterSearchType fst in filterSearchType)
            {
                fst.Filter(products);
            }
            //------------------------------------------------

            return products;
        }
        private HashSet<Product> SearchByName(string name, List<Shop> shops)
        {
            HashSet<Product> result = new HashSet<Product>();
            foreach (Shop shop in shops)
            {
                result.UnionWith(shop.SearchByName(name));
            }
            return result;
        }
        private HashSet<Product> SearchByCatagory(Category category, List<Shop> shops)
        {
            HashSet<Product> result = new HashSet<Product>();
            foreach (Shop shop in shops)
            {
                result.UnionWith(shop.SearchByCategory(category));
            }
            return result; ;
        }

        private HashSet<Product> SearchByKeywords(string keywords, List<Shop> shops)
        {
            HashSet<Product> result = new HashSet<Product>();
            foreach (Shop shop in shops)
            {
                result.UnionWith(shop.SearchByKeywords(keywords));
            }
            return result;
        }
        private bool NameSearch(SearchType searchType)
        {
            return (searchType & SearchType.Name) == SearchType.Name;
        }
        private bool CategorySearch(SearchType searchType)
        {
            return (searchType & SearchType.Category) == SearchType.Category;
        }
        private bool KeywordSearch(SearchType searchType)
        {
            return (searchType & SearchType.Keywords) == SearchType.Keywords;
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
