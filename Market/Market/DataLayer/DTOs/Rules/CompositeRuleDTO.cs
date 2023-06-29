using Market.DomainLayer;
using Market.DomainLayer.Rules;
using Microsoft.SqlServer.Management.Smo;

namespace Market.DataLayer.DTOs.Rules
{
    public class CompositeRuleDTO : RuleDTO
    {
        public List<RuleDTO> Rules { get; set; }
        public string Operator { get; set; }
        public CompositeRuleDTO() { }
        public CompositeRuleDTO(RuleSubjectDTO subject, List<RuleDTO> rules, string op) : base(subject) {
            Rules = rules;
            Operator = op;
        }
        public CompositeRuleDTO(CompositeRule rule) : base(rule) {
            Rules = new List<RuleDTO>();
            foreach(IRule subRule in rule.Rules) {
                Rules.Add(MarketContext.GetInstance().Rules.Find(subRule.Id));
            }
            Operator = rule.Operator.ToString();
        }
    }
}
