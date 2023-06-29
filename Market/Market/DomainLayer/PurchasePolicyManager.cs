using Market.DomainLayer.Rules;
using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class PurchasePolicyManager : IPolicyManager<PurchasePolicy>
    {
        public PurchasePolicyManager(int shopId):base(shopId)
        {
        }

        /// <summary>
        /// Generates a unic Id for the Purchasepolicy and adding it to policy Repo
        /// </summary>
        /// <param name="policy"></param>
        public void AddPolicy(int id, DateTime expirationDate,RuleSubject subject, IRule rule)
        {
            int unicId = int.Parse($"{_shopId}{id}");
            PurchasePolicy policy = new PurchasePolicy(unicId, ShopId, expirationDate, subject, rule);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepo.GetInstance().Add(policy);
        }
        public override void UpdatePolicy(int policyId)
        {
            throw new NotImplementedException();
        }

        public override PurchasePolicy GetPolicy(int policyId)
        {
            if (!Policies.TryGetValue(policyId, out IPolicy policy))
            {
                if (PolicyRepo.GetInstance().ContainsID(policyId))
                    return (PurchasePolicy)PolicyRepo.GetInstance().GetById(policyId);
                throw new Exception("Policy was not found");
            }
            return (PurchasePolicy)policy;
        }
    }
}
