using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer;
using Market.DomainLayer.Rules;

namespace Market.DataLayer.DTOs.Policies
{
    public class DiscountPolicyDTO : PolicyDTO
    {
        public double Precentage { get; set; }
        public DiscountPolicyDTO() { }
        public DiscountPolicyDTO(int id,DateTime expirationDate, int ruleId, PolicySubjectDTO subject, double percentage): base(id,expirationDate, ruleId, subject)
        {
            Precentage = percentage;
        }
        public DiscountPolicyDTO(DiscountPolicy policy): base(policy)
        {
            Precentage = policy.Precentage;
        }
    }
}
