using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SPurchasePolicy
    {
        public int id { get; set; }
        public string expirationDate { get; set; }
        public string description { get; set; }
        public SPurchasePolicy(PurchasePolicy policy)
        {
            id = policy.Id;
            expirationDate = policy.ExpirationDate.ToString();
            description = policy.GetInfo();
        }
    }
}
