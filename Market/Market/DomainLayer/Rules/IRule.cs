using Market.DataLayer;
using Market.DataLayer.DTOs.Rules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public abstract class IRule
    {
        private int _id;
        private RuleSubject _subject;
        private int _shopId;

        public int Id { get => _id; set => _id = value; }
        public int ShopId { get => _shopId; set => _shopId = value; }
        public RuleSubject Subject { get => _subject; set => _subject = value; }

        public IRule(int id, RuleSubject subject)
        {
            _id = id;
            _subject = subject;
        }
        public IRule(int id, int shopId)
        {
            _id = id;
            _shopId = shopId;
        }
        public IRule(RuleDTO ruleDTO) {
            _subject = new RuleSubject(ruleDTO.Subject);
            _id = ruleDTO.Id;
            _shopId = MarketContext.GetInstance().Shops
                .Include(s => s.Rules)
                .FirstOrDefault(s => s.Rules.Any(rule => rule.Id == ruleDTO.Id))
                .Id;
        }
        

        public abstract string GetInfo();
       // public abstract IRule AddRuleToDataBase();
        public abstract bool Predicate(Basket basket);
        public abstract RuleDTO CloneDTO();

        public abstract void Update();
    }
}
