using Market.DomainLayer.Rules;

namespace Market.DataLayer.DTOs.Rules
{
    public class QuantityRuleDTO : RuleDTO
    {
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public QuantityRuleDTO() { }
        public QuantityRuleDTO(RuleSubjectDTO subject, int minQuantity, int maxQuantity) : base(subject)
        {
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;

        }
        public QuantityRuleDTO(QuantityRule rule) : base(rule)
        {
            MinQuantity = rule.MinQuantity;
            MaxQuantity = rule.MaxQuantity;
        }
    }
}
