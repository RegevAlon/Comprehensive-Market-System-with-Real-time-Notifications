using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer;
using Market.DomainLayer.Rules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DataLayer.DTOs
{
    [Table("Shops")]
    public class ShopDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public double Rating { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<RuleDTO> Rules { get; set; }
        public List<PolicyDTO> Policies { get; set; }
        public List<PurchaseDTO> Purchases { get; set; }
        public List<PendingAgreementDTO> PendingAgreements { get; set; }


        public ShopDTO(int id, string name, bool active, double rating)
        {
            Id = id;
            Name = name;
            Active = active;
            Rating = rating;
            Products = new List<ProductDTO>();
            Rules = new List<RuleDTO>();
            Policies = new List<PolicyDTO>();
            Purchases = new List<PurchaseDTO>();
            PendingAgreements = new List<PendingAgreementDTO>();
        }
        public ShopDTO(Shop shop)
        {
            Id = shop.Id;
            Name = shop.Name;
            Active = shop.Active;
            Rating = shop.Rating;
            Products = new List<ProductDTO>();
            foreach (Product product in shop.Products)
                Products.Add(new ProductDTO(product));

            Rules = new List<RuleDTO>();
            foreach (IRule rule in shop.Rules.Values)
                Rules.Add(rule.CloneDTO());

            Policies = new List<PolicyDTO>();
            foreach (IPolicy policy in shop.PurchasePolicyManager.Policies.Values)
                Policies.Add(policy.CloneDTO());

            foreach (IPolicy policy in shop.DiscountPolicyManager.Policies.Values)
                Policies.Add(policy.CloneDTO());

            Purchases = new List<PurchaseDTO>();
            foreach (Purchase purchase in shop.Purchases.ToList<Purchase>())
                Purchases.Add(new PurchaseDTO(purchase));

            PendingAgreements = new List<PendingAgreementDTO>();
            foreach (PendingAgreement pa in shop.PendingAgreements.Values)
            {
                PendingAgreementDTO pendingDTO = new PendingAgreementDTO(pa);
                PendingAgreements.Add(pendingDTO);
            }

        }
        public ShopDTO() { }

        public ShopDTO(int id, string name, bool active, double rating, List<ProductDTO> products, List<RuleDTO> rules, List<PolicyDTO> policies, List<PurchaseDTO> purchases)
        {
            Id = id;
            Name = name;
            Active = active;
            Rating = rating;
            Products = products;
            Rules = rules;
            Policies = policies;
            Purchases = purchases;
            PendingAgreements = new List<PendingAgreementDTO>();
        }
    }
}
