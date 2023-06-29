using Market.DomainLayer.Rules;
using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Market.DomainLayer
{
    public class DiscountPolicyManager : IPolicyManager<DiscountPolicy>
    {
        private long _discountPolicyIdFactory;

        public DiscountPolicyManager(int shopId):base(shopId)
        {
        }
        

        public override void UpdatePolicy(int ruleId)
        {
            throw new NotImplementedException();
        }

        public override DiscountPolicy GetPolicy(int policyId)
        {
            if (!Policies.TryGetValue(policyId, out IPolicy policy))
            {
                if (PolicyRepo.GetInstance().ContainsID(policyId))
                    return (DiscountPolicy)PolicyRepo.GetInstance().GetById(policyId);
                throw new Exception("Policy was not found");
            }
            return (DiscountPolicy)policy;
        }
        public void AddCompositePolicy(int id, DateTime expirationDate, RuleSubject subject, NumericOperator Operator, List<int> policies)
        {
            List<IPolicy> policiesToAdd = new List<IPolicy>();
            foreach(int policyId in policies)
            {
                policiesToAdd.Add(GetPolicy(policyId));
                Policies.TryRemove(policyId, out IPolicy dummy);
                PolicyRepo.GetInstance().Delete(policyId);
            }
            int unicId = int.Parse($"{_shopId}{id}");
            DiscountCompositePolicy policy = new DiscountCompositePolicy(unicId,ShopId, expirationDate, subject, Operator, policiesToAdd);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepo.GetInstance().Add(policy);
        }
        public void AddPolicy(int id, DateTime expirationDate, RuleSubject subject, IRule rule, double precentage)
        {
            int unicId = int.Parse($"{_shopId}{id}");
            DiscountPolicy policy = new DiscountPolicy(unicId, ShopId, expirationDate, subject, rule, precentage);
            Policies.TryAdd(policy.Id, policy);
            PolicyRepo.GetInstance().Add(policy);
        }
    }
}
