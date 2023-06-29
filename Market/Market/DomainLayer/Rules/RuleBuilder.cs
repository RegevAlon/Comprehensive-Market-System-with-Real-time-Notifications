using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer.Rules
{
    public class RuleBuilder : Builder<IRule>
    {
        private IRule _restult;
        private RuleSubject _subject;
        private LogicalOperator _operator;
        private double _targetPrice;
        private int _minQuantity;
        private int _maxQuantity;
        private List<IRule> _rules;
        private int _ruleidFactory;
        int shopId;

        public int RuleidFactory { get => _ruleidFactory; set => _ruleidFactory = value; }

        public RuleBuilder(int shopID)
        {
            _ruleidFactory = int.Parse($"{shopId}{0}");
            shopId = shopID;
        }
        public RuleBuilder(int shopID, int ruleidFactory)
        {
            _ruleidFactory = ruleidFactory;
            shopId = shopID;
        }
        public void buildFeatures(RuleSubject productOrCategory)
        {
            _subject = productOrCategory;
        }

        public void buildFeatures(RuleSubject productOrCategory, int minQuantity, int maxQuantity)
        {
            _subject = productOrCategory;
            _minQuantity = minQuantity;
            _maxQuantity = maxQuantity;
        }

        public void buildFeatures(RuleSubject productOrCategory, int targetPrice)
        {
            _subject = productOrCategory;
            _targetPrice = targetPrice;
        }
        public void buildFeatures(LogicalOperator Operator, List<IRule> rules)
        {
            _operator = Operator;
            _rules = rules;
        }
        public void makeSimpleRule()
        {
            _restult = new SimpleRule(GenerateUniqueId(), shopId, _subject);
        }

        public void makeQuantityRule()
        {

            _restult = new QuantityRule(GenerateUniqueId(), shopId, _subject, _minQuantity, _maxQuantity);
        }

        public void makeTotalPriceRule()
        {
            _restult = new TotalPriceRule(GenerateUniqueId(), shopId, _subject, _targetPrice);
        }

        public void makeCompositeRule()
        {
            _restult = new CompositeRule(GenerateUniqueId(), shopId, _rules, _operator);
        }

        public void reset()
        {
            _restult = null;
            _subject = null;
            _targetPrice = 0;
            _minQuantity = 0;
            _maxQuantity = int.MaxValue;
            _rules = null;
        }

        public IRule Build()
        {
            return _restult;
        }

        public void BuildID()
        {
            _ruleidFactory++;
        }
        private int GenerateUniqueId()
        {
            return int.Parse($"{shopId}{_ruleidFactory}");
        }
    }
}
