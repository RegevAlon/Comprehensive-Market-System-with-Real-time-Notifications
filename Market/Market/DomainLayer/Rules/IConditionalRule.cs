using Market.DataLayer.DTOs.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public abstract class IConditionalRule : IRule
    {
        public IConditionalRule(int id, int shopId, RuleSubject subject) : base(id, shopId)
        {
            Subject = subject;
        }

        public IConditionalRule(RuleDTO ruleDTO) : base(ruleDTO)
        {
        }

        public abstract override string GetInfo();

        public abstract override bool Predicate(Basket basket);
    }
}
