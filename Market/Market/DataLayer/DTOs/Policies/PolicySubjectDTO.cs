using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer.Rules;
using Market.RepoLayer;

namespace Market.DataLayer.DTOs.Policies
{
    [Table("Policy Subjects")]
    public class PolicySubjectDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public string Category { get; set; }
        public PolicySubjectDTO() { }
        public PolicySubjectDTO(ProductDTO product, string category)
        {
            Product = product;
            Category = category;
        }
        public PolicySubjectDTO(RuleSubject subject)
        {
            if (subject.Product != null)
            {
                Product = MarketContext.GetInstance().Products.Find(subject.Product.Id);
            }
            else
            {
                Product = GenerateDummyProduct();
            }
            Category = subject.Category.ToString();
        }
        private ProductDTO GenerateDummyProduct()
        {
            if (ProductRepo.GetInstance().ContainsID(-1))
                return MarketContext.GetInstance().Products.Find(-1);
            return new ProductDTO(-1, "null", 1, 1, "None", "", "", new List<ReviewDTO>(),"RegularSell",new List<BidDTO>());
        }
    }
}