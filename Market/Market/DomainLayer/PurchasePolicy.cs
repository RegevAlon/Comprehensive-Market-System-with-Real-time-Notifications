using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer.DTOs.Policies;
using Market.DomainLayer.Rules;

namespace Market.DomainLayer
{
    public class PurchasePolicy : IPolicy
    {
        public PurchasePolicy(int id,int shopId, DateTime expirationDate, RuleSubject subject, IRule rule) : base(id,shopId, expirationDate, subject, rule)
        {
        }

        public PurchasePolicy(PurchasePolicyDTO purchasePolicyDTO) : base(purchasePolicyDTO)
        {
        }

        public override void Apply(Basket basket)
        {
            if (!IsExpired())
            {
                if (!IsValidForBasket(basket))
                    throw new Exception("Basket does not stand with purchase policy constraints");
            }
        }
        public override string GetInfo()
        {
            return $"Purchase Policy on {Rule.Subject.GetInfo()}\nRule - {Rule.GetInfo()}";
        }

        public override bool IsValidForBasket(Basket basket)
        {
            return Rule.Predicate(basket);
        }
        public override PurchasePolicyDTO CloneDTO()
        {
            return new PurchasePolicyDTO(this);
        }
    }
}
