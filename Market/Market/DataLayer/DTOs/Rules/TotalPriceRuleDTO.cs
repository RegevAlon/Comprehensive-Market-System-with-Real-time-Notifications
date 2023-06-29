using Market.DomainLayer.Rules;
using Microsoft.SqlServer.Management.Smo;

namespace Market.DataLayer.DTOs.Rules
{
    public class TotalPriceRuleDTO: RuleDTO
    {
        public double TotalPrice { get; set; }
        public TotalPriceRuleDTO() { }
        public TotalPriceRuleDTO(RuleSubjectDTO subject, double totalPrice) : base(subject)
        {
            TotalPrice = totalPrice;
        }
        public TotalPriceRuleDTO(TotalPriceRule rule) : base(rule) 
        {
            TotalPrice = rule.TotalPrice;
        }
    }
}
