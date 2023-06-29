using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class RemoveAppointmentEvent : Event
    {
        private Member _member;
        private Member _removedMember;
        private Shop _shop;
        public RemoveAppointmentEvent(Shop shop,Member member,Member removedMember) : base("Remove Appointment Event")
        {
            _shop = shop;
            _member = member;
            _removedMember = removedMember;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: \'{_member.UserName}\' removed \'{_removedMember.UserName}\' appointment " +
                $"from shop: {_shop.Name}";
        }
    }
}
