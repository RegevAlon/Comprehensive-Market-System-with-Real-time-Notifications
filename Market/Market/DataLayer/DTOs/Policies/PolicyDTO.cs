using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer.Rules;
using System.Security.Cryptography;
using Market.DomainLayer;
using System.Data;
using Market.DataLayer.DTOs.Rules;

namespace Market.DataLayer.DTOs.Policies
{
    [Table("Policies")]
    public class PolicyDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public DateTime ExpirationDate { get; set; }
        [ForeignKey("RuleDTO")]
        public int RuleId { get; set; }
        public PolicySubjectDTO PolicySubject { get; set; }


        public PolicyDTO() { }

        public PolicyDTO(int id,DateTime expirationDate, int ruleId, PolicySubjectDTO subject)
        {
            Id = id;
            ExpirationDate = expirationDate;
            RuleId = ruleId;
            PolicySubject = subject;
        }
        public PolicyDTO(IPolicy policy)
        {
            Id = policy.Id;
            ExpirationDate = policy.ExpirationDate;
            RuleId = policy.Rule.Id;
            PolicySubject = new PolicySubjectDTO(policy.Subject);
        }
    }
}
