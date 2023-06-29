using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer.Rules;

namespace Market.DomainLayer
{
    public class ShopDirector
    {
        private int _shopId;
        RuleBuilder ruleBuilder;
        public ShopDirector(int shopId)
        {
            _shopId = shopId;
            ruleBuilder = new RuleBuilder(shopId);
        }
        public ShopDirector(int shopId,int ruleIdFactory)
        {
            _shopId = shopId;
            ruleBuilder = new RuleBuilder(shopId, ruleIdFactory);
        }

        public IRule makeRule(Type type)
        {
            ruleBuilder.BuildID();
            switch (type.Name)
            {
                case "SimpleRule": ruleBuilder.makeSimpleRule(); break;
                case "QuantityRule": ruleBuilder.makeQuantityRule(); break;
                case "TotalPriceRule": ruleBuilder.makeTotalPriceRule(); break;
                case "CompositeRule": ruleBuilder.makeCompositeRule(); break;
            }
            return ruleBuilder.Build();
        }

        public void setFeatures(RuleSubject productOrCategory)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory);
        }

        public void setFeatures(RuleSubject productOrCategory, int minQuantity, int maxQuantity)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory, minQuantity, maxQuantity);
        }

        public void setFeatures(RuleSubject productOrCategory, int targetPrice)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(productOrCategory, targetPrice);        }

        public void setFeatures(LogicalOperator Operator, List<IRule> rules)
        {
            ruleBuilder.reset();
            ruleBuilder.buildFeatures(Operator, rules);
        }
    }
}
