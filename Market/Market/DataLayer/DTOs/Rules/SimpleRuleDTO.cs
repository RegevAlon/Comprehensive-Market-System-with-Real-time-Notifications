using Market.DomainLayer.Rules;
using Microsoft.SqlServer.Management.Smo;

namespace Market.DataLayer.DTOs.Rules
{
    public class SimpleRuleDTO: RuleDTO
    {
        public SimpleRuleDTO() { }
        public SimpleRuleDTO(RuleSubjectDTO subject) : base(subject)
        {
        }
        public SimpleRuleDTO(SimpleRule rule) : base(rule){
        }
    }
}
