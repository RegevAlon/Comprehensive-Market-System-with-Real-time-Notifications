using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using Market.ServiceLayer;

namespace Market.DataLayer.DTOs
{
    [Table("Appointments")]
    public class AppointmentDTO
    {
        [Key]
        [ForeignKey("MemberDTO")]
        public int MemberId { get; set; }
        [Key]
        [ForeignKey("ShopDTO")]
        public int ShopId { get; set; }
        public MemberDTO? Appointer { get; set; }
        public List<AppointeesDTO> Appointees { get; set; }
        public string Role { get; set; }
        public int Permissions { get; set; }

        public AppointmentDTO() { }
        public AppointmentDTO(Appointment appointment)
        {
            MemberId = appointment.Member.Id;
            ShopId = appointment.Shop.Id;
            if (appointment.Appointer != null)
                Appointer = MarketContext.GetInstance().Members.Find(appointment.Appointer.Id);
            Appointees = new List<AppointeesDTO>();
            foreach (Member member in appointment.Apointees)
                Appointees.Add(new AppointeesDTO(MarketContext.GetInstance().Members.Find(member.Id)));
            Role = appointment.Role.ToString();
            Permissions = (int)appointment.Permissions;
        }

        public AppointmentDTO(int memberId, int shopId, MemberDTO appointer, List<MemberDTO> appointees, string role, int permissions)
        {
            MemberId = memberId;
            ShopId = shopId;
            Appointer = appointer;
            Appointees = new List<AppointeesDTO>();
            foreach (MemberDTO member in appointees)
                Appointees.Add(new AppointeesDTO(member));
            Role = role;
            Permissions = permissions;
        }
    }
}
