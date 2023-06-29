using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Market.DomainLayer.Rules;
using Market.DataLayer.DTOs.Rules;
using Microsoft.AspNetCore.Routing;
using Market.DataLayer;
using Market.DataLayer.DTOs;

namespace Market.RepoLayer
{
    public class RuleRepo : IRepo<IRule>
    {
        private static ConcurrentDictionary<int, IRule> _ruleById;

        private static RuleRepo _ruleRepo = null;

        private RuleRepo()
        {
            _ruleById = new ConcurrentDictionary<int, IRule>();
        }
        public static RuleRepo GetInstance()
        {
            if (_ruleRepo == null)
                _ruleRepo = new RuleRepo();
            return _ruleRepo;
        }

        public void Add(IRule rule)
        {
            _ruleById.TryAdd(rule.Id, rule);
            RuleDTO ruleDTO = rule.CloneDTO();
            MarketContext.GetInstance().Shops.Find(rule.ShopId).Rules.Add(ruleDTO);
            MarketContext.GetInstance().SaveChanges();
        }

        public bool ContainsID(int id)
        {
            if (_ruleById.ContainsKey(id))
                return true;
            else return MarketContext.GetInstance().Shops.Any(s => s.Rules.Any(r => r.Id.Equals(id)));
        }

        public bool ContainsValue(IRule rule)
        {
            if (_ruleById.Contains(new KeyValuePair<int, IRule>(rule.Id, rule)))
                return true;
            else return MarketContext.GetInstance().Shops.Any(s => s.Rules.Any(r => r.Id.Equals(rule.Id)));
        }

        public void Delete(int id)
        {
            if (_ruleById.ContainsKey(id))
            {
                _ruleById.TryRemove(id, out IRule removed);
                ShopDTO shop =  MarketContext.GetInstance().Shops.Find(_ruleById[id].ShopId);
                shop.Rules.Remove(shop.Rules.Find(r=>r.Id==id));
                MarketContext.GetInstance().SaveChanges();
                
            }
            else throw new Exception("Product Id does not exist."); ;
        }

        public List<IRule> GetAll()
        {
            UploadRulesFromContext();
            return _ruleById.Values.ToList();
        }

        public IRule GetById(int id)
        {
            if (_ruleById.ContainsKey(id))
                return _ruleById[id];
            else if (ContainsID(id))
            {
                MarketContext context = MarketContext.GetInstance();
                ShopDTO shopDto = context.Shops.Where(s => s.Rules.Any(r => r.Id == id)).FirstOrDefault();
                RuleDTO ruleDTO = shopDto.Rules.Find(r=>r.Id==id);
                _ruleById.TryAdd(id, makeRule(ruleDTO));
                return _ruleById[id];
            }
            else
                throw new ArgumentException("Invalid Rule Id.");
        }

        public void Update(IRule rule)
        {
            _ruleById[rule.Id] = rule;
            rule.Update();
        }
        public void Update(SimpleRule rule)
        {
            MarketContext context = MarketContext.GetInstance();
            ShopDTO shopDto = context.Shops.Where(s => s.Rules.Any(r => r.Id == rule.Id)).FirstOrDefault();
            SimpleRuleDTO ruleDTO = (SimpleRuleDTO)shopDto.Rules.Find(r => r.Id == rule.Id);
        }
        public void Update(QuantityRule rule)
        {
            
        }
        public void Update(CompositeRule rule)
        {
            
        }
        public void Update(TotalPriceRule rule)
        {

        }
        /// <summary>
        /// returns all product of a given shop 
        /// </summary>
        /// <param name="shopId"></param> the Id of the shop
        /// <returns></returns>
        public ConcurrentDictionary<int, IRule> GetShopRules(int shopId)
        {
            UploadShopRulesFromContext(shopId);
            ConcurrentDictionary<int, IRule> shopRules = new ConcurrentDictionary<int, IRule>();
            foreach (IRule rule in _ruleById.Values)
            {
                if (rule.ShopId == shopId) shopRules.TryAdd(rule.Id, rule);
            }
            return shopRules;
        }

        private void UploadShopRulesFromContext(int shopId)
        {
            ShopDTO shopDto = MarketContext.GetInstance().Shops.Find(shopId);
            if (shopDto != null)
            {
                if (shopDto.Rules != null)
                {
                    List<RuleDTO> rules = shopDto.Rules.ToList();
                    foreach (RuleDTO ruleDTO in rules)
                    {
                        _ruleById.TryAdd(ruleDTO.Id, makeRule(ruleDTO));
                    }
                }
            }
        }

        private void UploadRulesFromContext()
        {
            List<ShopDTO> shops = MarketContext.GetInstance().Shops.ToList();
            foreach(ShopDTO shopDTO in shops)
            {
                UploadShopRulesFromContext(shopDTO.Id);
            }
        }

        public IRule makeRule(RuleDTO ruleDTO)
        {
            Type ruleType = ruleDTO.GetType();
            if (ruleType.Name.Equals("SimpleRuleDTO"))
            {
                return new SimpleRule((SimpleRuleDTO)ruleDTO);
            }
            else if (ruleType.Name.Equals("QuantityRuleDTO"))
            {
                return new QuantityRule((QuantityRuleDTO)ruleDTO);
            }
            else if (ruleType.Name.Equals("TotalPriceRuleDTO"))
            {
                return new TotalPriceRule((TotalPriceRuleDTO)ruleDTO);
            }
            else if (ruleType.Name.Equals("CompositeRuleDTO"))
            {
                List<IRule> rules = new List<IRule>();
                foreach (RuleDTO r in ((CompositeRuleDTO)ruleDTO).Rules)
                {
                    rules.Add(makeRule(r));
                }
                return new CompositeRule((CompositeRuleDTO)ruleDTO, rules);
            }
            return null;
        }

        public void Clear()
        {
            _ruleById.Clear();
        }
        public void ResetDomainData()
        {
            _ruleById.Clear();
        }
    }
}
