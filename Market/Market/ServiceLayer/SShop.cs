using Market.DomainLayer;
using Market.DomainLayer.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SShop
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isOpen { get; set; }
        public List<SAppointment> appointments { get; set; }
        public List<SProduct> products { get; set; }
        public List<SPurchase> purchases { get; set; }
        public List<SDiscountPolicy> discountPolicies { get; set; }
        public List<SPurchasePolicy> purchasePolicies { get; set; }
        public List<SRule> rules { get; set; }

        public double rating { get; set; }

        public List<SPendingAgreement> pendingAgreements { get; set; }


        public SShop(int id, string name, List<SAppointment> appointments, List<SProduct> products, List<SPurchase> purchase, double rating)
        {
            this.id = id;
            this.name = name;
            this.appointments = appointments;
            this.products = products;
            this.purchases = purchase;
            this.rating = rating;
        }
        public SShop(Shop shop)
        {
            id = shop.Id;
            name = shop.Name;
            isOpen = shop.Active;
            appointments = new List<SAppointment>();
            products = new List<SProduct>();
            discountPolicies = new List<SDiscountPolicy>();
            purchasePolicies = new List<SPurchasePolicy>();
            purchases = new List<SPurchase>();
            rules = new List<SRule>();
            pendingAgreements = new List<SPendingAgreement>();
            foreach (Appointment appointment in shop.Appointments.Values)
            {
                appointments.Add(new SAppointment(appointment));
            }
            foreach (Product product in shop.Products)
            {
                products.Add(new SProduct(product));
            }
            foreach (Purchase purchase in shop.Purchases)
            {
                purchases.Add(new SPurchase(purchase));
            }
            foreach (PurchasePolicy p in shop.PurchasePolicyManager.Policies.Values)
            {
                purchasePolicies.Add(new SPurchasePolicy(p));
            }
            foreach (DiscountPolicy p in shop.DiscountPolicyManager.Policies.Values)
            {
                discountPolicies.Add(new SDiscountPolicy(p));
            }
            foreach (IRule r in shop.Rules.Values)
            {
                rules.Add(new SRule(r));
            }
            foreach (PendingAgreement pa in shop.PendingAgreements.Values)
            {
                pendingAgreements.Add(new SPendingAgreement(pa));
            }
            rating = shop.CalculateShopRating();
        }
    }
}
