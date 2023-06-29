using Market.DataLayer.DTOs.Rules;
using Market.RepoLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public class CompositeRule : IRule
    {
        private List<IRule> _rules;
        private LogicalOperator _operator;
        public List<IRule> Rules { get => _rules; set => _rules = value; }
        public LogicalOperator Operator { get => _operator; set => _operator = value; }

        public CompositeRule(int id, int shopId, List<IRule> rules, LogicalOperator op) : base(id, shopId)
        {
            Subject = new RuleSubject();
            _rules = rules;
            _operator = op;
        }

        public CompositeRule(CompositeRuleDTO ruleDTO,List<IRule> rules) : base(ruleDTO)
        {
            Subject = new RuleSubject();
            _rules = rules;
            _operator = CastOperator(ruleDTO.Operator);
        }

        private LogicalOperator CastOperator(string operatorName)
        {
            try
            {
                return (LogicalOperator)Enum.Parse(typeof(LogicalOperator), operatorName);
            }
            catch { throw new Exception("Invalid operator name"); }
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Composite Rule: ");
            sb.AppendLine(ParseOpertor());
            foreach (IRule rule in _rules)
            {
                sb.AppendLine(rule.GetInfo());
            }
            return sb.ToString();
        }


        public override bool Predicate(Basket basket)
        {
            switch (_operator)
            {
                case LogicalOperator.Or: return PredicateOr(basket);
                case LogicalOperator.And: return PredicateAnd(basket);
                case LogicalOperator.Xor: return PredicateXor(basket);
                default: return false;
            }
        }
        public bool PredicateOr(Basket basket)
        {
            foreach (IRule discountRule in Rules)
            {
                try
                {
                    if (discountRule.Predicate(basket))
                    {
                        Subject = discountRule.Subject;
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
        public bool PredicateXor(Basket basket)
        {
            int trueCount = 0;

            foreach (IRule discountRule in Rules)
            {
                try
                {
                    if (discountRule.Predicate(basket))
                    {
                        trueCount++;
                        if (trueCount > 1)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return trueCount == 1;
        }
        public bool PredicateAnd(Basket basket)
        {
            foreach (IRule discountRule in Rules)
            {
                if (!discountRule.Predicate(basket))
                    return false;
            }
            return true;
        }
        public void AddRule(IRule rule)
        {
            _rules.Add(rule);
        }
        public void RemoveRule(IRule rule)
        {
            _rules.Remove(rule);
        }
        private string ParseOpertor()
        {
            switch (_operator)
            {
                case LogicalOperator.Or: return "With Operator: OR, ";
                case LogicalOperator.Xor: return "With Operator: Xor, ";
                case LogicalOperator.And: return "With Operator: And, ";
                default: return "";
            }

        }
        public override CompositeRuleDTO CloneDTO()
        {

            return new CompositeRuleDTO(this);
        }

        public override void Update()
        {
            RuleRepo.GetInstance().Update(this);
        }
    }
}
