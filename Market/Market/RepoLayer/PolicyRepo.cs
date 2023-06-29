using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Market.DomainLayer.Rules;
using Market.DataLayer.DTOs.Rules;
using Microsoft.AspNetCore.Routing;
using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using Market.DataLayer.DTOs.Policies;

namespace Market.RepoLayer
{
    public class PolicyRepo : IRepo<IPolicy>
    {
        private static ConcurrentDictionary<int, IPolicy> _policyById;

        private static PolicyRepo _policyRepo = null;

        private PolicyRepo()
        {
            _policyById = new ConcurrentDictionary<int, IPolicy>();
        }
        public static PolicyRepo GetInstance()
        {
            if (_policyRepo == null)
                _policyRepo = new PolicyRepo();
            return _policyRepo;
        }

        public void Add(IPolicy policy)
        {
            _policyById.TryAdd(policy.Id, policy);
            MarketContext.GetInstance().Shops.Find(policy.ShopId).Policies.Add(policy.CloneDTO());
            MarketContext.GetInstance().SaveChanges();
        }

        public bool ContainsID(int id)
        {
            if (_policyById.ContainsKey(id))
                return true;
            else return MarketContext.GetInstance().Shops.Any(s => s.Rules.Any(r => r.Id.Equals(id)));
        }

        public bool ContainsValue(IPolicy policy)
        {
            if (_policyById.Contains(new KeyValuePair<int, IPolicy>(policy.Id, policy)))
                return true;
            else return MarketContext.GetInstance().Shops.Any(s => s.Policies.Any(r => r.Id.Equals(policy.Id)));
        }

        public void Delete(int id)
        {
            if (_policyById.ContainsKey(id))
            {
                ShopDTO shop = MarketContext.GetInstance().Shops.Find(_policyById[id].ShopId);
                _policyById.TryRemove(id, out IPolicy removed);
                PolicyDTO p = shop.Policies.Find(p => p.Id == id);
                shop.Policies.Remove(p);
                MarketContext.GetInstance().Policies.Remove(p);
                MarketContext.GetInstance().SaveChanges();
            }
            else throw new Exception("Product Id does not exist."); ;
        }

        public List<IPolicy> GetAll()
        {
            UploadRulesFromContext();
            return _policyById.Values.ToList();
        }

        public IPolicy GetById(int id)
        {
            if (_policyById.ContainsKey(id))
                return _policyById[id];
            else if (ContainsID(id))
            {
                MarketContext context = MarketContext.GetInstance();
                ShopDTO shopDto = context.Shops.Where(s => s.Policies.Any(p => p.Id == id)).FirstOrDefault();
                PolicyDTO policyDTO = shopDto.Policies.Find(p => p.Id == id);
                _policyById.TryAdd(id, makePolicy(policyDTO));
                return _policyById[id];
            }
            else
                throw new ArgumentException("Invalid Rule Id.");
        }

        public void Update(IPolicy rule)
        {
           
        }
       
        /// <summary>
        /// returns all product of a given shop 
        /// </summary>
        /// <param name="shopId"></param> the Id of the shop
        /// <returns></returns>
        public ConcurrentDictionary<int, IPolicy> GetShopRules(int shopId)
        {
            UploadShopPoliciesFromContext(shopId);
            ConcurrentDictionary<int, IPolicy> shopPolicies = new ConcurrentDictionary<int, IPolicy>();
            foreach (IPolicy policy in _policyById.Values)
            {
                if (policy.ShopId == shopId) shopPolicies.TryAdd(policy.Id, policy);
            }
            return shopPolicies;
        }

        private void UploadShopPoliciesFromContext(int shopId)
        {
            ShopDTO shopDto = MarketContext.GetInstance().Shops.Find(shopId);
            if (shopDto != null)
            {
                if (shopDto.Rules != null)
                {
                    List<PolicyDTO> policies = shopDto.Policies.ToList();
                    foreach (PolicyDTO policyDTO in policies)
                    {
                        _policyById.TryAdd(policyDTO.Id, makePolicy(policyDTO));
                    }
                }
            }
        }

        private void UploadRulesFromContext()
        {
            List<ShopDTO> shops = MarketContext.GetInstance().Shops.ToList();
            foreach (ShopDTO shopDTO in shops)
            {
                UploadShopPoliciesFromContext(shopDTO.Id);
            }
        }

        public IPolicy makePolicy(PolicyDTO policyDTO)
        {
            Type policyType = policyDTO.GetType();
            if (policyType.Name.Equals("DiscountPolicyDTO"))
            {
                return new DiscountPolicy((DiscountPolicyDTO)policyDTO);
            }
            else if (policyType.Name.Equals("PurchasePolicyDTO"))
            {
                return new PurchasePolicy((PurchasePolicyDTO)policyDTO);
            }
            else if (policyType.Name.Equals("DiscountCompositePolicyDTO"))
            {
                List<IPolicy> policies = new List<IPolicy>();
                foreach (PolicyDTO p in ((DiscountCompositePolicyDTO)policyDTO).Policies)
                {
                    policies.Add(makePolicy(p));
                }
                return new DiscountCompositePolicy((DiscountCompositePolicyDTO)policyDTO, policies);
            }
            return null;
        }

        public void Clear()
        {
            _policyById.Clear();
        }
        public void ResetDomainData()
        {
            _policyById = new ConcurrentDictionary<int, IPolicy>();
        }
    }
}
