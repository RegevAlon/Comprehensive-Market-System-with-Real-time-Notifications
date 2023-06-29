using Market.DataLayer.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer
{
    [Table("Bids Answers")]
    public class BidAnswerDTO
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Members")]
        public int OwnerId { get; set; }
        public string Answer { get; set; }

        public BidAnswerDTO(int ownerId, string answer)
        {
            OwnerId = ownerId;
            Answer = answer;
        }
    }
}
