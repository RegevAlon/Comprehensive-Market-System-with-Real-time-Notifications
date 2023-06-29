using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.RepoLayer
{
    public class MemberRepo : IRepo<Member>
    {
        private static Dictionary<int, Member> _memberById;
        private static Dictionary<string, Member> _memberByUsername;
        private object _lock;

        private static MemberRepo _memberRepo = null;

        private MemberRepo()
        {
            _memberById = new Dictionary<int, Member>();
            _memberByUsername = new Dictionary<string, Member>();
            _lock = new object();
        }
        public static MemberRepo GetInstance()
        {
            if (_memberRepo == null)
                _memberRepo = new MemberRepo();
            return _memberRepo;
        }
        public void Add(Member item)
        {
            MarketContext context = MarketContext.GetInstance();
            _memberById.Add(item.Id, item);
            _memberByUsername.Add(item.UserName, item);
            lock (_lock)
            {
                context.Members.Add(new MemberDTO(item));
                context.SaveChanges();
            }

        }

        public void Delete(int id)
        {
            lock (_lock)
            {
                MarketContext context = MarketContext.GetInstance();
                MemberDTO m = context.Members.Find(id);
                if (_memberById.ContainsKey(id))
                {
                    Member member = _memberById[id];
                    _memberByUsername.Remove(member.UserName);
                    _memberById.Remove(id);
                    context.Members.Remove(m);
                }
                else if (m != null)
                {
                    context.Members.Remove(m);
                }
                context.SaveChanges();
            }
        }
        public List<Member> GetAll()
        {
            UploadMembersFromContext();
            return _memberById.Values.ToList();
        }

        public Member GetById(int id)
        {
            if (_memberById.ContainsKey(id))
                return _memberById[id];
            else
            {
                MarketContext context = MarketContext.GetInstance();
                MemberDTO mDto = context.Members.Find(id);
                if (mDto != null)
                {
                    AddMemberFromContextToDomain(mDto);
                    return _memberById[id];
                }
                throw new ArgumentException("Invalid user ID.");
            }
        }

        public void Update(Member item)
        {
            UploadMembersFromContext();
            if (ContainsValue(item))
            {
                _memberById[item.Id] = item;
                _memberByUsername[item.UserName] = item;
            }
            MarketContext context = MarketContext.GetInstance();
            lock (_lock)
            {
                MemberDTO mDto = context.Members.Find(item.Id);
                if (mDto != null)
                {
                    mDto.ShoppingCart.PurchaseIdFactory = item.ShoppingCart.PurchaseIdFactory;
                    mDto.Messages = new List<MessageDTO>();
                    foreach (Message message in item.Messages) mDto.Messages.Add(new MessageDTO(message));
                    mDto.Id = item.Id;
                    mDto.UserName = item.UserName;
                    mDto.Password = item.Password;
                    mDto.Notification = item.Notification;
                    context.SaveChanges();
                }
            }
        }
        public void SetAsSystemAdmin(Member item)
        {
            UploadMembersFromContext();
            MarketContext context = MarketContext.GetInstance();
            MemberDTO mDto = context.Members.Find(item.Id);
            lock (_lock)
            {
                if (mDto != null)
                {
                    mDto.IsSystemAdmin = true;
                    context.SaveChanges();
                }
            }
        }

        public Member GetByUserName(string userName)
        {
            if (_memberByUsername.ContainsKey(userName))
                return _memberByUsername[userName];
            else
            {
                List<MemberDTO> m = MarketContext.GetInstance().Members.Where((mem) => mem.UserName.ToLower() == userName.ToLower()).ToList();
                if (m.Count() > 0)
                {
                    AddMemberFromContextToDomain(m.First());
                    return _memberByUsername[userName];
                }
                throw new ArgumentException("Invalid user name.");
            }
        }

        public Boolean ContainsUserName(string userName)
        {
            if (!_memberByUsername.ContainsKey(userName))
            {
                List<MemberDTO> m = MarketContext.GetInstance().Members.Where((m) => m.UserName.Equals(userName)).ToList();
                if (m.Count() > 0)
                {
                    AddMemberFromContextToDomain(m.First());
                }
                return m.Count > 0;
            }
            return _memberByUsername.ContainsKey(userName);
        }
        private void UploadMembersFromContext()
        {
            MarketContext context = MarketContext.GetInstance();
            List<MemberDTO> members = context.Members.ToList();
            foreach (MemberDTO member in members)
            {
                _memberById.TryAdd(member.Id, new Member(member));
            }
        }

        public bool ContainsID(int id)
        {
            if (!_memberById.ContainsKey(id))
            {
                MemberDTO m = MarketContext.GetInstance().Members.Find(id);
                if (m != null)
                {
                    AddMemberFromContextToDomain(m);
                }
                return m != null;
            }
            return true;
        }

        public bool ContainsValue(Member item)
        {
            if (!_memberById.ContainsKey(item.Id))
            {
                MemberDTO m = MarketContext.GetInstance().Members.Find(item.Id);
                if (m != null)
                {
                    AddMemberFromContextToDomain(m);
                }
                return m != null;
            }
            return true;
        }

        public void Clear()
        {
            _memberById.Clear();
            _memberByUsername.Clear();
        }

        private void AddMemberFromContextToDomain(MemberDTO memberDto)
        {
            Member member = new Member(memberDto);
            _memberById[member.Id] = member;
            _memberByUsername[member.UserName] = member;
            member.InitializeComplexFeilds(memberDto);
        }

        public void ResetDomainData()
        {
            _memberById = new Dictionary<int, Member>();
            _memberByUsername = new Dictionary<string, Member>();
        }

        public List<Member> GetOwners(int shopId)
        {
            List<int> ownersIds = MarketContext.GetInstance().Appointments.Where<AppointmentDTO>((a) => a.ShopId == shopId && (a.Role == "Owner" || a.Role == "Founder")).Select((a) => a.MemberId).ToList<int>();
            List<Member> owners = new List<Member>();
            foreach (int id in ownersIds)
                owners.Add(GetById(id));
            return owners;
        }
    }
}