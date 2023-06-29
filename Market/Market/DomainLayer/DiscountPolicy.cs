using Market.DataLayer.DTOs.Policies;
using Market.DomainLayer.Rules;
using Market.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class DiscountPolicy : IPolicy
    {
        private double _precentage;

        public double Precentage { get => _precentage; set => _precentage = value; }

        public DiscountPolicy(int id, int shopId, DateTime expirationDate, RuleSubject subject, IRule rule, double percentage) : base(id, shopId, expirationDate, subject, rule)
        {
            Precentage = percentage;
        }
        public DiscountPolicy(int id,int shopId, DateTime expirationDate, RuleSubject subject) : base(id,shopId, expirationDate, subject)
        {
        }
        public DiscountPolicy(DiscountPolicyDTO discountPolicyDTO):base(discountPolicyDTO)
        {
            _precentage = discountPolicyDTO.Precentage;
        }

        public override void Apply(Basket basket)
        {
            if (!IsExpired() && IsValidForBasket(basket))
            {
                RuleSubject subjectToDiscount = Subject;
                if (subjectToDiscount.IsProduct())
                    ApplyOnProduct(basket, subjectToDiscount.GetProduct());
                else ApplyOnCategory(basket, subjectToDiscount.GetCategory());
            }
        }
        private void ApplyOnProduct(Basket basket, Product product)
        {
            BasketItem basketItem = basket.GetBasketItem(product);
            if(basketItem!=null && IsRegularSellProduct(basketItem))
            {
                basketItem.PriceAfterDiscount -= basketItem.PriceAfterDiscount * Precentage;
            }
        }
        private bool IsRegularSellProduct(BasketItem basketItem)
        {
            return basketItem.Product.SellMethod is RegularSell;
        }
        private void ApplyOnCategory(Basket basket, Category category)
        {
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                if (basketItem.Product.HasCategory(category)&&IsRegularSellProduct(basketItem))
                    basketItem.PriceAfterDiscount -= basketItem.Product.Price * Precentage;
            }
        }

        public override string GetInfo()
        {
            return $"Simple Discount: Subject: {Rule.Subject.GetInfo()}, Precentage: {_precentage * 100}";
        }

        public override bool IsValidForBasket(Basket basket)
        {
            return Rule.Predicate(basket);
        }
        public double GetDiscount(Basket basket)
        {
            RuleSubject subjectToDiscount = Rule.Subject;                
            if (!IsValidForBasket(basket))
                return 0;

            if (!subjectToDiscount.IsProduct())
                return GetCategoryDiscount(basket, subjectToDiscount.GetCategory());

            else
                return GetProductDiscount(basket, subjectToDiscount.GetProduct());
        }
        private double GetCategoryDiscount(Basket basket,Category category)
        {
            double priceToReduce = 0;
            foreach (BasketItem basketItem in basket.BasketItems)
            {
                if (basketItem.Product.HasCategory(category))
                priceToReduce += CalculateDiscount(basketItem.Quantity, basketItem.Product.Price);
            }
            return priceToReduce;
        }
        private double GetProductDiscount(Basket basket, Product product)
        {
            double priceToReduce = 0;
            BasketItem basketItem = basket.GetBasketItem(product);
            priceToReduce += CalculateDiscount(basketItem.Quantity, basketItem.Product.Price);
            return priceToReduce;
        }
        private double CalculateDiscount(int quantity, double price)
        {
            return quantity * price * Precentage;
        }
        public override DiscountPolicyDTO CloneDTO()
        {
            return new DiscountPolicyDTO(this);
        }
    }
}
