using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Market.DomainLayer;
using Market.DomainLayer.Rules;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DataLayer.DTOs
{
    [Table("Events")]
    public class EventDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Shops")]
        public int ShopId { get; set; }
        public MemberDTO Listener { get; set; }
        public EventDTO() { }
        public EventDTO(string name, int shopId, MemberDTO listener)
        {
            Name = name;
            ShopId = shopId;
            Listener = listener;
        }
    }
}
