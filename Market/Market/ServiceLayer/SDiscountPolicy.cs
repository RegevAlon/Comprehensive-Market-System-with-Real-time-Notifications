using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SDiscountPolicy
    {
        public int id { get; set; }
        public string expirationDate { get; set; }
        public string description { get; set; }
        public SDiscountPolicy(DiscountPolicy policy)
        {
            id = policy.Id;
            expirationDate = policy.ExpirationDate.ToString();
            description = policy.GetInfo();
        }
        public SDiscountPolicy(DiscountCompositePolicy policy)
        {
            id = policy.Id;
            expirationDate = policy.ExpirationDate.ToString();
            description = policy.GetInfo();
        }
    }
}
