using Market.DomainLayer.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SRule
    {
        public int id { get; set; }
        public string description { get; set; }
        public SRule(IRule rule)
        {
            id = rule.Id;
            description = rule.GetInfo();
        }
    }
}
