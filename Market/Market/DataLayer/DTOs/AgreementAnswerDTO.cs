using Market.DataLayer.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer
{
    [Table("Agreement answers")]
    public class AgreementAnswerDTO
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Members")]
        public int OwnerId { get; set; }
        public String Answer { get; set; }

        public AgreementAnswerDTO(int ownerId, string answer)
        {
            OwnerId = ownerId;
            Answer = answer;
        }
    }
}
