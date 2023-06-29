using Market.DataLayer;
using Market.DataLayer.DTOs.Rules;
using Market.RepoLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public class SimpleRule : IRule
    {
        public SimpleRule(int id, int shopId, RuleSubject subject) : base(id, shopId)
        {
            Subject = subject;
        }

        public SimpleRule(SimpleRuleDTO ruleDTO):base(ruleDTO)
        {
            
        }

        public override string GetInfo()
        {
            return $"Simple Rule: Basket must contain at least one {Subject.GetInfo()}";
        }

        public override bool Predicate(Basket basket)
        {
            if (Subject.IsProduct())
            {
                return basket.HasProduct(Subject.Product);
            }
            else
            {
                foreach (BasketItem basketItem in basket.BasketItems)
                {
                    if (basketItem.Product.HasCategory(Subject.Category))
                        return true;
                }
            }
            return false;
        }
        public override SimpleRuleDTO CloneDTO()
        {
            return new SimpleRuleDTO(this);
        }

        public override void Update()
        {
            RuleRepo.GetInstance().Update(this);
        }
    }
}
