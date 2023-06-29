using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Appointment
    {
        private Shop _shop;
        private Member _member;
        private Member _appointer;
        private SynchronizedCollection<Member> _apointees;
        private Role _role;
        private Permission _permissions;

        public Appointment(Member member, Shop shop, Member appointer, Role role, Permission permissions)
        {
            _member = member;
            _shop = shop;
            _appointer = appointer;
            _apointees = new SynchronizedCollection<Member>();
            _role = role;
            _permissions = permissions;
        }

        public Appointment(AppointmentDTO appDto)
        {
            _role = CastRole(appDto.Role);
            _permissions = CastPermission(appDto.Permissions);
        }
        public void InitializeComplexFeilds(AppointmentDTO appDto)
        {
            if (appDto.ShopId != null && appDto.MemberId != null)
            {
                _shop = ShopRepo.GetInstance().GetById(appDto.ShopId);
                _member = MemberRepo.GetInstance().GetById(appDto.MemberId);
            }
            if (appDto.Appointer != null)
                _appointer = MemberRepo.GetInstance().GetById(appDto.Appointer.Id);
            else _appointer = null;
            _apointees = new SynchronizedCollection<Member>();
            if (appDto.Appointees != null && appDto.Appointees.Count() > 0)
            {
                foreach (AppointeesDTO app in appDto.Appointees)
                {
                    _apointees.Add(MemberRepo.GetInstance().GetById(app.Appointee.Id));
                }
            }
        }
        private Role CastRole(string role)
        {
            switch (role)
            {
                case "Founder": return Role.Founder; break;
                case "Manager": return Role.Manager; break;
                case "Owner": return Role.Owner; break;
                default: throw new Exception("Invalid Role name");
            }
        }
        private Permission CastPermission(int permission)
        {
            Permission compositePermission = 0;
            foreach (Permission p in Enum.GetValues<Permission>())
            {
                if ((p & (Permission)permission) == p)
                    compositePermission |= p;
            }
            return compositePermission;
        }

        public Member Appointer { get => _appointer; }
        public SynchronizedCollection<Member> Apointees { get => _apointees; }
        public Role Role { get => _role; set => _role = value; }
        public Shop Shop { get => _shop; set => _shop = value; }
        public Member Member { get => _member; }
        public Permission Permissions { get => _permissions; set => _permissions = value; }

        public bool HasPermission(Permission permission)
        {
            return (_permissions & permission) == permission;
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---------------------------");
            sb.AppendLine(string.Format("Member: %s", _member.UserName));
            sb.AppendLine(string.Format("Role: %s", _role.ToString()));
            sb.AppendLine("---------------------------");
            return sb.ToString();
        }
    }
}
