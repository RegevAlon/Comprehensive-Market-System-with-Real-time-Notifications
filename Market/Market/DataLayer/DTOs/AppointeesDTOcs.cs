using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Appointees")]
    public class AppointeesDTO
    {
        [Key]
        public int Id { get; set; }
        public MemberDTO Appointee { get; set; }
        public AppointeesDTO() { }
        public AppointeesDTO(MemberDTO appointee)
        {
            Appointee = appointee;
        }
    }
}
