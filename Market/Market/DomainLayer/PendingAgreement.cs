using Market.DataLayer;
using Market.RepoLayer;

namespace Market.DomainLayer
{
    public class PendingAgreement
    {
        public int ShopId { get; set; }
        public Member Appointer { get; set; }
        public Member Appointee { get; set; }
        public List<Member> Approved { get; set; }
        public List<Member> Declined { get; set; }
        public List<Member> Pendings { get; set; }

        public PendingAgreement(PendingAgreementDTO pendingDTO)
        {
            ShopId = pendingDTO.ShopId;
            Appointer = MemberRepo.GetInstance().GetById(pendingDTO.AppointerId);
            Appointee = MemberRepo.GetInstance().GetById(pendingDTO.AppointeeId);
            Approved = new List<Member>();
            Declined = new List<Member>();
            Pendings = new List<Member>();

            foreach (AgreementAnswerDTO ans in pendingDTO.Answers)
            {
                Member owner = MemberRepo.GetInstance().GetById(ans.OwnerId);
                if (ans.Answer.Equals("Pending"))
                    Pendings.Add(owner);
                if (ans.Answer.Equals("Approved"))
                    Approved.Add(owner);
                if (ans.Answer.Equals("Declined"))
                    Declined.Add(owner);
            }
        }
        public PendingAgreement(int shopId, Member appointer, Member appointee, List<Member> pendings)
        {
            ShopId = shopId;
            Appointer = appointer;
            Appointee = appointee;
            Approved = new List<Member>();
            Declined = new List<Member>();
            Pendings = pendings;
        }
        public PendingAgreement(int shopId, Member appointer, Member apointee)
        {
            ShopId = shopId;
            Appointer = appointer;
            Appointee = apointee;
            List<int> pendingsIds = MarketContext.GetInstance().Appointments.Where((a) => a.ShopId == shopId && (a.Role == Role.Owner.ToString() || a.Role == Role.Founder.ToString())).Select((a) => a.MemberId).ToList();
            Pendings = new List<Member>();
            foreach (int id in pendingsIds)
                Pendings.Add(MarketContext.GetInstance().Find<Member>(id));
            Declined = new List<Member>();
            Approved = new List<Member>();
        }

        public bool CheckIfApproved()
        {
            return Declined.Count() == 0 && Pendings.Count() == 0;
        }
        public bool CheckIfDeclined()
        {
            return Approved.Count() == 0 && Pendings.Count() == 0;
        }
        public void AddApproval(Member member)
        {
            if (Approved.Contains(member))
                return;    
            Approved.Add(member);
            Pendings.Remove(member);
            Declined.Remove(member);
        }
        public void AddDecline(Member member)
        {
            if (Declined.Contains(member))
                return;    
            Declined.Add(member);
            Approved.Remove(member);
            Pendings.Remove(member);
        }
    }
}
