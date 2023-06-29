using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using Market.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Market.RepoLayer
{
    public class ShopRepo : IRepo<Shop>
    {
        private static ConcurrentDictionary<int, Shop> _shops;
        private static ShopRepo _shopRepo = null;
        private object _lock;

        public ConcurrentDictionary<int, Shop> Shops { get => _shops; set => _shops = value; }

        private ShopRepo()
        {
            _shops = new ConcurrentDictionary<int, Shop>();
            _lock = new object();
        }
        public static ShopRepo GetInstance()
        {
            if (_shopRepo == null)
                _shopRepo = new ShopRepo();
            return _shopRepo;
        }

        public void Add(Shop item)
        {
            _shops.TryAdd(item.Id, item);
            MarketContext.GetInstance().Shops.Add(new ShopDTO(item));
            MarketContext.GetInstance().SaveChanges();
        }

        public bool ContainsID(int id)
        {
            if (!_shops.ContainsKey(id))
            {
                return MarketContext.GetInstance().Shops.Find(id) != null;
            }
            return true;
        }

        public bool ContainsValue(Shop item)
        {
            if (!_shops.ContainsKey(item.Id))
            {
                return MarketContext.GetInstance().Shops.Find(item.Id) != null;
            }
            return true;
        }

        public void Delete(int id)
        {
            lock (_lock)
            {
                bool shopInDomain = _shops.TryRemove(id, out _);
                MarketContext context = MarketContext.GetInstance();
                ShopDTO shopDTO = context.Shops.Find(id);
                if (shopInDomain)
                {
                    context.Shops.Remove(shopDTO);
                    context.SaveChanges();
                }
                else if (shopDTO != null)
                {
                    context.Shops.Remove(shopDTO);
                    context.SaveChanges();
                }
                else throw new Exception("Shop ID does not exist");
            }
        }

        public List<Shop> GetAll()
        {
            UploadShopsFromContext();
            return _shops.Values.ToList();
        }
        private void UploadShopsFromContext()
        {
            lock (_lock)
            {
                List<ShopDTO> shopList = MarketContext.GetInstance().Shops.ToList();
                foreach (ShopDTO shopDto in shopList)
                {
                    _shops.TryAdd(shopDto.Id, new Shop(shopDto));
                }
            }
        }

        public Shop GetById(int id)
        {
            if (_shops.ContainsKey(id))
            {
                return _shops[id];
            }
            else
            {
                lock (_lock)
                {
                    ShopDTO shopDTO = MarketContext.GetInstance().Shops.Find(id);
                    if (shopDTO != null)
                    {
                        Shop shop = new Shop(shopDTO);
                        _shops.TryAdd(id, shop);
                        shop.InitializeComplexfeilds(shopDTO);
                        return shop;
                    }
                    else
                    {
                        throw new Exception("Shop Id does not exist");
                    }
                }
            }
        }

        public void Update(Shop item)
        {
            _shops[item.Id] = item;
            ShopDTO shopdto = MarketContext.GetInstance().Shops.Find(item.Id);
            ShopDTO newShopdto = new ShopDTO(item);
            if (shopdto != null)
            {
                shopdto.Active = newShopdto.Active;
                shopdto.Purchases = newShopdto.Purchases;
                shopdto.Products = newShopdto.Products;
                shopdto.Rules = newShopdto.Rules;
                shopdto.Name = newShopdto.Name;
                shopdto.Rating = newShopdto.Rating;
            }
            else MarketContext.GetInstance().Shops.Add(newShopdto);
            MarketContext.GetInstance().SaveChanges();
        }

        public List<Shop> GetAllActiveShops()
        {
            UploadShopsFromContext();
            List<Shop> shops = new List<Shop>();
            foreach (Shop shop in _shops.Values)
            {
                if (shop.Active) shops.Add(shop);
            }
            return shops;
        }

        public Shop GetByName(string name)
        {
            Shop shop = _shops.Values.ToList().Find(x => x.Name.ToLower().Equals(name.ToLower()));
            if (shop != null) return shop;
            UploadShopsFromContext();
            shop = _shops.Values.ToList().Find(x => x.Name.ToLower().Equals(name.ToLower()));
            if (shop != null) return shop;
            else throw new Exception($"No shop with name {name}.");
        }

        public void Clear()
        {
            _shops.Clear();
        }
        public void ResetDomainData()
        {
            _shops.Clear();
        }
        //public ConcurrentDictionary<string, PendingAgreement> GetShopPendingAgreements(int shopId)
        //{
        //    ConcurrentDictionary<string, PendingAgreement> agreements = new ConcurrentDictionary<string, PendingAgreement>();
        //    List<PendingAgreementDTO> dtoAgreements = MarketContext.GetInstance().Find<ShopDTO>(shopId).PendingAgreements.ToList<PendingAgreementDTO>();
        //    foreach (PendingAgreementDTO dto in dtoAgreements)
        //    {
        //        Member apointee = MemberRepo.GetInstance().GetById(dto.AppointeeId);
        //        agreements.TryAdd(apointee.UserName, new PendingAgreement(dto));
        //    }
        //    return agreements;
        //}
        //public void DeletePendingAgreement(PendingAgreement item, int shopId)
        //{
        //    if (item == null) throw new ArgumentNullException("Can't delete the pending agreement because it is null");
        //    lock (_lock)
        //    {
        //        if (_shops.ContainsKey(shopId))
        //            _shops[shopId].DeletePendingAgreement(item.Appointee.UserName);
        //        ShopDTO shopdto = MarketContext.GetInstance().Shops.Find(shopId);
        //        PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where((p) => p.AppointeeId == item.Appointee.Id && p.ShopId == item.ShopId).FirstOrDefault();

        //        List<AgreementAnswerDTO> pendings = MarketContext.GetInstance().AgreementsAnswers.Where<AgreementAnswerDTO>((a) => a.ShopId == pendingDTO.ShopId && a.AppointeeId == pendingDTO.AppointeeId && a.Answer == "Pending").ToList<AgreementAnswerDTO>();
        //        List<AgreementAnswerDTO> approved = MarketContext.GetInstance().AgreementsAnswers.Where<AgreementAnswerDTO>((a) => a.ShopId == pendingDTO.ShopId && a.AppointeeId == pendingDTO.AppointeeId && a.Answer == "Approved").ToList<AgreementAnswerDTO>();
        //        List<AgreementAnswerDTO> declined = MarketContext.GetInstance().AgreementsAnswers.Where<AgreementAnswerDTO>((a) => a.ShopId == pendingDTO.ShopId && a.AppointeeId == pendingDTO.AppointeeId && a.Answer == "Declined").ToList<AgreementAnswerDTO>();

        //        foreach (AgreementAnswerDTO ans in pendings)
        //            MarketContext.GetInstance().AgreementsAnswers.Remove(ans);
        //        foreach (AgreementAnswerDTO ans in approved)
        //            MarketContext.GetInstance().AgreementsAnswers.Remove(ans);
        //        foreach (AgreementAnswerDTO ans in declined)
        //            MarketContext.GetInstance().AgreementsAnswers.Remove(ans);

        //        shopdto.PendingAgreements.Remove(pendingDTO);
        //        MarketContext.GetInstance().SaveChanges();
        //    }
        //}

        //public void AddPendingAgreement(PendingAgreement pendingAgreement, Shop shop)
        //{
        //    ShopDTO shopdto = MarketContext.GetInstance().Find<ShopDTO>(shop.Id);
        //    shopdto.PendingAgreements.Add(new PendingAgreementDTO(pendingAgreement));
        //    MarketContext.GetInstance().Update<ShopDTO>(shopdto);
        //    MarketContext.GetInstance().SaveChanges();
        //}

        //public static void AddApproval(Member client, PendingAgreement pendingAgreement, Shop shop)
        //{
        //    PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where<PendingAgreementDTO>((p)=> p.ShopId == shop.Id && p.AppointeeId == pendingAgreement.Appointee.Id).FirstOrDefault();
        //    AgreementAnswerDTO existAnswer = MarketContext.GetInstance().Find<AgreementAnswerDTO>(client.Id, shop.Id, pendingAgreement.Appointee.Id);
        //    if (existAnswer != null)
        //    {
        //        existAnswer.Answer = "Approved";
        //        MarketContext.GetInstance().AgreementsAnswers.Update(existAnswer);
        //    }
        //}

        //public static void AddDeclined(Member client, PendingAgreement pendingAgreement, Shop shop)
        //{
        //    PendingAgreementDTO pendingDTO = MarketContext.GetInstance().PendingAgreements.Where<PendingAgreementDTO>((p) => p.ShopId == shop.Id && p.AppointeeId == pendingAgreement.Appointee.Id).FirstOrDefault();
        //    AgreementAnswerDTO existAnswer = MarketContext.GetInstance().Find<AgreementAnswerDTO>(client.Id, shop.Id, pendingAgreement.Appointee.Id);
        //    if (existAnswer != null)
        //    {
        //        existAnswer.Answer = "Decliend";
        //        MarketContext.GetInstance().AgreementsAnswers.Update(existAnswer);
        //    }
        //}
    }
}