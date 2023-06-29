using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer.Rules;
using Market.RepoLayer;

namespace Market.DataLayer.DTOs.Rules
{
    [Table("Rule Subjects")]
    public class RuleSubjectDTO
    {
        [Key]
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public string Category { get; set; }

        public RuleSubjectDTO()
        {
        }

        public RuleSubjectDTO(ProductDTO product, string category)
        {
            Product = product;
            Category = category;
        }
        public RuleSubjectDTO(RuleSubject subject)
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
            ProductDTO dummyProductDTO = MarketContext.GetInstance().Products.Find(-1);
            if (dummyProductDTO != null)
                return dummyProductDTO;
            else
            {
                dummyProductDTO = new ProductDTO(-1, "null", 1, 1, "None", "", "", new List<ReviewDTO>(), "RegularSell", new List<BidDTO>());
                MarketContext.GetInstance().Products.Add(dummyProductDTO);
                MarketContext.GetInstance().SaveChanges();
                return dummyProductDTO;
            }

        }

    }
}
