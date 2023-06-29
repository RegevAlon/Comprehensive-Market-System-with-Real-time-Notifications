using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Messages")]
    public class MessageDTO
    {
        [Key]
        public int Id { get; set; }
        public string MessageContent { get; set; }
        public bool Seen { get; set; }
        public MessageDTO() { }

        public MessageDTO(int id, string messageContent)
        {
            Id = id;
            MessageContent = messageContent;
        }
        public MessageDTO(Message messageContent)
        {
            MessageContent = messageContent.Comment;
            Seen = messageContent.Seen;
        }
    }

}