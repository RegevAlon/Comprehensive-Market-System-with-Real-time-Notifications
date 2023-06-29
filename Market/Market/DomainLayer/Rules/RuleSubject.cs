using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Market.RepoLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public class RuleSubject
    {
        private Product _product;
        private Category _category;

        public Product Product { get => _product; set => _product = value; }
        public Category Category { get => _category; set => _category = value; }
        public RuleSubject() {
            _category = Category.None;
        }
        public RuleSubject(Product product)
        {
            _product = product;
            _category = Category.None;
        }
        public RuleSubject(Category category)
        {
            _category = category;
        }
        public RuleSubject(RuleSubjectDTO subject)
        {
            if (subject.Category.Equals("None"))
            {
                _category = Category.None;
                if (subject.Product.Id != -1)
                {
                    _product = ProductRepo.GetInstance().GetById(subject.Product.Id);
                }
                else _product = null;
            }
            else
            {
                _category = CastCategory(subject.Category);
            }
        }

        public RuleSubject(PolicySubjectDTO policySubject)
        {
            if (policySubject.Category.Equals("None"))
            {
                _category = Category.None;
                if (policySubject.Product.Id != -1)
                {
                    _product = ProductRepo.GetInstance().GetById(policySubject.Product.Id);
                }
                else _product = null;
            }
            else
            {
                _category = CastCategory(policySubject.Category);
            }
        }

        private Category CastCategory(string categoryName)
        {
            try
            {
                return (Category)Enum.Parse(typeof(Category), categoryName);
            }
            catch (Exception) { return Category.None; }
        }
        public bool IsProduct()
        {
            return _product != null;
        }
        public Product GetProduct()
        {
            return _product;
        }
        public Category GetCategory()
        {
            return _category;
        }
        public string GetInfo()
        {
            if (IsProduct()) { return _product.ToString(); }
            else { return _category.ToString(); }
        }
    }
}
