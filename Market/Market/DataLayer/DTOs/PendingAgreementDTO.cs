using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using Market.DataLayer.DTOs;
using System.Reflection;

namespace Market.DataLayer
{
    [Table("Pending Agreements")]
    public class PendingAgreementDTO
    {
        [Key]
        [ForeignKey("Shops")]
        public int ShopId { get; set; }
        [Key]
        [ForeignKey("Members")]
        public int AppointeeId { get; set; }

        [ForeignKey("Members")]
        public int AppointerId { get; set; }
        public List<AgreementAnswerDTO> Answers { get; set; }
        public PendingAgreementDTO()
        {
        }

        public PendingAgreementDTO(int shopId, int appointerId, int appointeeId)
        {
            ShopId = shopId;
            AppointerId = appointerId;
            AppointeeId = appointeeId;
            Answers = new List<AgreementAnswerDTO>();
        }


        public PendingAgreementDTO(PendingAgreement pendingAgreement)
        {
            ShopId = pendingAgreement.ShopId;
            AppointerId = pendingAgreement.Appointer.Id;
            AppointeeId = pendingAgreement.Appointee.Id;
            Answers = new List<AgreementAnswerDTO>();
            foreach (Member m in pendingAgreement.Approved)
                Answers.Add(new AgreementAnswerDTO(m.Id, "Approved"));
            foreach (Member m in pendingAgreement.Declined)
                Answers.Add(new AgreementAnswerDTO(m.Id, "Declined"));
            foreach (Member m in pendingAgreement.Pendings)
                Answers.Add(new AgreementAnswerDTO(m.Id, "Pending"));
        }
    }
}
