using Market.DomainLayer;
using Market.ServiceLayer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ServiceModel.Channels;
using Message = Market.DomainLayer.Message;

namespace Market.DataLayer.DTOs
{
    [Table("Members")]
    public class MemberDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
        public bool Notification { get; set; }
        public ShoppingCartDTO ShoppingCart { get; set; }
        public List<ShoppingCartPurchaseDTO> ShoppingCartPurchases { get; set; }
        public bool IsSystemAdmin { get; set; }



        public MemberDTO() { }
        public MemberDTO(int id, string userName, string password, List<MessageDTO> messages, bool notification, ShoppingCartDTO shoppingCart, List<ShoppingCartPurchaseDTO> shoppingCartPurchases)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Messages = messages;
            Notification = notification;
            ShoppingCart = shoppingCart;
            ShoppingCartPurchases = shoppingCartPurchases;
            IsSystemAdmin = false;
        }
        public MemberDTO(int id, string userName, string password, List<MessageDTO> messages, bool notification, ShoppingCartDTO shoppingCart)
        {
            Id = id;
            UserName = userName;
            Password = password;
            if (messages != null) Messages = messages; else Messages = new List<MessageDTO>();
            Notification = notification;
            ShoppingCart = shoppingCart;
            ShoppingCartPurchases = new List<ShoppingCartPurchaseDTO>();
            IsSystemAdmin = false;
        }

        public MemberDTO(int id, string userName, string password, List<MessageDTO> messages, bool notification)
        {
            Id = id;
            UserName = userName;
            Password = password;
            if (messages != null) Messages = messages; else Messages = new List<MessageDTO>();
            Notification = notification;
            ShoppingCart = new ShoppingCartDTO(id);
            ShoppingCartPurchases = new List<ShoppingCartPurchaseDTO>();
            IsSystemAdmin = false;
        }
        public MemberDTO(int id, string userName, string password, bool notification)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Messages = new List<MessageDTO>();
            Notification = notification;
            ShoppingCart = new ShoppingCartDTO(id);
            ShoppingCartPurchases = new List<ShoppingCartPurchaseDTO>();
            IsSystemAdmin = false;
        }
        public MemberDTO(Member member)
        {
            Id = member.Id;
            UserName = member.UserName;
            Password = member.Password;
            Messages = new List<MessageDTO>();
            foreach (Message message in member.Messages)
            {
                Messages.Add(new MessageDTO(message));
            }
            Notification = member.Notification;
            ShoppingCart = new ShoppingCartDTO(member.ShoppingCart);
            ShoppingCartPurchases = new List<ShoppingCartPurchaseDTO>();
            foreach (ShoppingCartPurchase purchase in member.UserPurchases.ToList())
                ShoppingCartPurchases.Add(new ShoppingCartPurchaseDTO(purchase));
            IsSystemAdmin = false;
        }
    }
}
