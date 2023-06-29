using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Reviews")]
    public class ReviewDTO
    {
        [Key]
        public int Id { get; set; }
        public string ReviewerUsername { get; set; }
        public double Rate { get; set; }
        public string Comment { get; set; }
        public ReviewDTO(string reviewerUsername, double rate, string comment)
        {
            ReviewerUsername = reviewerUsername;
            Rate = rate;
            Comment = comment;
        }
        public ReviewDTO() { }
        public ReviewDTO(Review review) {
            ReviewerUsername = review.User;
            Rate = review.Rate;
            Comment = review.Comment;
        }
    }
}
