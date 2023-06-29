using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public abstract class IPolicyManager<T> where T : IPolicy
    {
        protected int _shopId;
        private ConcurrentDictionary<int, IPolicy> _policies;

        protected IPolicyManager(int shopId)
        {
            ShopId = shopId;
            _policies = new ConcurrentDictionary<int, IPolicy>();
        }

        public int ShopId { get => _shopId; set => _shopId = value; }
        public ConcurrentDictionary<int, IPolicy> Policies { get => _policies; set => _policies = value; }

        public abstract T GetPolicy(int policyId);
        public void RemovePolicy(int policyId)
        {
            if (!_policies.TryRemove(policyId, out IPolicy removed))
            {
                throw new Exception("Policy was not found");
            }
            PolicyRepo.GetInstance().Delete(policyId);
        }
        public abstract void UpdatePolicy(int policyId);
        public void Apply(Basket basket)
        {
            CleanExpiredPolicies();
            IPolicy[] policies = _policies.Values.ToArray();
            foreach (IPolicy policy in policies)
            {
                policy.Apply(basket);
            }
        }
        public void CleanExpiredPolicies()
        {
            List<IPolicy> policies = _policies.Values.ToList();
            foreach (IPolicy policy in policies)
            {
                if (policy.IsExpired())
                {
                    _policies.TryRemove(policy.Id, out _);
                }
            }
        }
    }
}
