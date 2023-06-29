using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer.Rules;
using System.Security.Cryptography;
using Market.DomainLayer;
using System.Data;
using Market.DataLayer.DTOs.Rules;

namespace Market.DataLayer.DTOs.Policies
{
    public class PurchasePolicyDTO: PolicyDTO
    {
        public PurchasePolicyDTO() { }
        public PurchasePolicyDTO(int id,DateTime expirationDate,int ruleId, PolicySubjectDTO subject): base(id, expirationDate, ruleId, subject) { }

        public PurchasePolicyDTO(PurchasePolicy policy): base(policy) { }
    }
}
