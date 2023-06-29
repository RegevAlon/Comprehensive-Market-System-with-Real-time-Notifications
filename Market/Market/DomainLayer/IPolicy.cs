using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer;
using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer.Rules;
using Market.RepoLayer;
using Microsoft.EntityFrameworkCore;

namespace Market.DomainLayer
{
    public abstract class IPolicy
    {
        private int _id;
        private DateTime _expirationDate;
        private IRule _rule;
        private RuleSubject _subject;
        private int _shopId;

        public int Id { get => _id; set => _id = value; }
        public DateTime ExpirationDate { get => _expirationDate; set => _expirationDate = value; }
        public IRule Rule { get => _rule; set => _rule = value; }
        public RuleSubject Subject { get => _subject; set => _subject = value; }
        public int ShopId { get => _shopId; set => _shopId = value; }

        public IPolicy(int id, int shopId, DateTime expirationDate, RuleSubject subject, IRule rule)
        {
            _id = id;
            _rule = rule;
            _expirationDate = expirationDate;
            _subject = subject;
            _shopId = shopId;
        }
        public IPolicy(int id ,int shopId, DateTime expirationDate, RuleSubject subject)
        {
            _id = id;
            _expirationDate = expirationDate;
            _subject = subject;
            _shopId = shopId;
        }

        protected IPolicy(DiscountPolicyDTO discountPolicyDTO)
        {
            _id = discountPolicyDTO.Id;
            _expirationDate = discountPolicyDTO.ExpirationDate;
            _subject = new RuleSubject(discountPolicyDTO.PolicySubject);
            _rule = RuleRepo.GetInstance().GetById(discountPolicyDTO.RuleId);
            if (_rule.Id == -1) _rule = null;
            _shopId = MarketContext.GetInstance().Shops
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Policies.Any(policy => policy.Id == policy.Id))
                .Id;
        }
        protected IPolicy(PurchasePolicyDTO purchasePolicyDTO)
        {
            _id = purchasePolicyDTO.Id;
            _expirationDate = purchasePolicyDTO.ExpirationDate;
            _subject = new RuleSubject(purchasePolicyDTO.PolicySubject);
            _rule = RuleRepo.GetInstance().GetById(purchasePolicyDTO.RuleId);
            _shopId = MarketContext.GetInstance().Shops
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Policies.Any(policy => policy.Id == policy.Id))
                .Id;
        }

        public abstract void Apply(Basket basket);
        public abstract string GetInfo();
        public abstract bool IsValidForBasket(Basket basket);
        public bool IsExpired()
        {
            return _expirationDate < DateTime.Now;
        }
        public abstract PolicyDTO CloneDTO();

        
    }
}
