using Market.DataLayer.DTOs;
using Market.DataLayer;
using Market.DomainLayer;
using System.Collections.Concurrent;
using System.Reflection;

namespace Market.RepoLayer
{
    public class PendingAgreementsRepo
    {
        private static PendingAgreementsRepo _agreementsRepo = null;
        private object _lock;


        private PendingAgreementsRepo()
        {
            _lock = new object();
        }
        public static PendingAgreementsRepo GetInstance()
        {
            if (_agreementsRepo == null)
                _agreementsRepo = new PendingAgreementsRepo();
            return _agreementsRepo;
        }
        public ConcurrentDictionary<string, PendingAgreement> GetShopPendingAgreements(int shopId)
        {
            ConcurrentDictionary<string, PendingAgreement> agreements = new ConcurrentDictionary<string, PendingAgreement>();
            List<PendingAgreementDTO> dtoAgreements = MarketContext.GetInstance().Find<ShopDTO>(shopId).PendingAgreements.ToList<PendingAgreementDTO>();
            foreach (PendingAgreementDTO dto in dtoAgreements)
            {
                Member apointee = MemberRepo.GetInstance().GetById(dto.AppointeeId);
                agreements.TryAdd(apointee.UserName, new PendingAgreement(dto));
            }
            return agreements;
        }
        public void DeletePendingAgreement(PendingAgreement item, int shopId)
        {
            if (item == null) throw new ArgumentNullException("Can't delete the pending agreement because it is null");
            lock (_lock)
            {
                if (ShopRepo.GetInstance().ContainsID(shopId))
                    ShopRepo.GetInstance().GetById(shopId).DeletePendingAgreement(item.Appointee.UserName);
                ShopDTO shopdto = MarketContext.GetInstance().Shops.Find(shopId);
                PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where((p) => p.AppointeeId == item.Appointee.Id && p.ShopId == item.ShopId).FirstOrDefault();
                shopdto.PendingAgreements.Remove(pendingDTO);
                MarketContext.GetInstance().Remove<PendingAgreementDTO>(pendingDTO);
                MarketContext.GetInstance().SaveChanges();
            }
        }

        public void AddPendingAgreement(PendingAgreement pendingAgreement, Shop shop)
        {
            ShopDTO shopdto = MarketContext.GetInstance().Find<ShopDTO>(shop.Id);
            shopdto.PendingAgreements.Add(new PendingAgreementDTO(pendingAgreement));
            MarketContext.GetInstance().Update<ShopDTO>(shopdto);
            MarketContext.GetInstance().SaveChanges();
        }

        public void AddApproval(Member client, PendingAgreement pendingAgreement, Shop shop)
        {
            PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where<PendingAgreementDTO>((p) => p.ShopId == shop.Id && p.AppointeeId == pendingAgreement.Appointee.Id).FirstOrDefault();
            AgreementAnswerDTO existAnswer = pendingDTO.Answers.Where<AgreementAnswerDTO>((a) => a.OwnerId == client.Id).FirstOrDefault();
            if (existAnswer != null)
            {
                existAnswer.Answer = "Approved";
                MarketContext.GetInstance().AgreementsAnswers.Update(existAnswer);
                MarketContext.GetInstance().SaveChanges();
            }
        }

        public void AddDeclined(Member client, PendingAgreement pendingAgreement, Shop shop)
        {
            PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where<PendingAgreementDTO>((p) => p.ShopId == shop.Id && p.AppointeeId == pendingAgreement.Appointee.Id).FirstOrDefault();
            AgreementAnswerDTO existAnswer = pendingDTO.Answers.Where<AgreementAnswerDTO>((a) => a.OwnerId == client.Id).FirstOrDefault();
            if (existAnswer != null)
            {
                existAnswer.Answer = "Decliend";
                MarketContext.GetInstance().AgreementsAnswers.Update(existAnswer);
                MarketContext.GetInstance().SaveChanges();
            }
            if (pendingAgreement.CheckIfDeclined())
                DeletePendingAgreement(pendingAgreement, shop.Id);
        }

        public void DeleteOwnerAndCheck(Member appointee, Shop shop)
        {
            foreach (PendingAgreement pend in shop.PendingAgreements.Values)
            {
                pend.Pendings.Remove(appointee);
                pend.Approved.Remove(appointee);
                pend.Declined.Remove(appointee);
                ShopDTO shopdto = MarketContext.GetInstance().Shops.Find(shop.Id);
                PendingAgreementDTO penddto = shopdto.PendingAgreements.Where<PendingAgreementDTO>((p) => p.AppointeeId == pend.Appointee.Id).FirstOrDefault();
                List<AgreementAnswerDTO> existAnswers = penddto.Answers.Where(a => a.OwnerId == appointee.Id).ToList<AgreementAnswerDTO>();
                foreach (AgreementAnswerDTO ansdto in existAnswers)
                {
                    penddto.Answers.Remove(ansdto);
                    MarketContext.GetInstance().AgreementsAnswers.Remove(ansdto);
                }
                MarketContext.GetInstance().SaveChanges();
                if (pend.CheckIfApproved())
                    pend.Appointer.AddOwnerAppointment(shop, new Appointment(pend.Appointee, shop, pend.Appointer, Role.Owner, Permission.All));
            }
        }

        public void AddOwner(Shop shop, Appointment appointment)
        {
            foreach (PendingAgreement pend in shop.PendingAgreements.Values)
            {
                pend.Pendings.Add(appointment.Member);
                ShopDTO shopdto = MarketContext.GetInstance().Shops.Find(shop.Id);
                PendingAgreementDTO penddto = shopdto.PendingAgreements.Where<PendingAgreementDTO>((p) => p.AppointeeId == pend.Appointee.Id).FirstOrDefault();
                penddto.Answers.Add(new AgreementAnswerDTO(appointment.Member.Id, "Pending"));
                MarketContext.GetInstance().SaveChanges();
            }
        }

        public bool AppointeeHasPendings(int appointeeId)
        {
            List<PendingAgreementDTO> pendingsDTO = MarketContext.GetInstance().PendingAgreements.Where((p) => p.AppointeeId == appointeeId).ToList<PendingAgreementDTO>();
            return pendingsDTO.Any();
        }
    }
}
