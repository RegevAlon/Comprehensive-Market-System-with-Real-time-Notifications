using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class AddAppointmentEvent : Event
    {
        private Member _member;
        private Member _memberToAdd;
        private Shop _shop;
        private Appointment _appointment;
        public AddAppointmentEvent(Shop shop, Member member, Member memberToAdd,Appointment ap) : base("Add Appointment Event")
        {
            _shop = shop;
            _member = member;
            _memberToAdd = memberToAdd;
            _appointment = ap;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: \'{_member.UserName}\' added \'{_memberToAdd.UserName}\' " +
                $"appointment: {_appointment.Role.ToString()} " +
                $"from shop: {_shop.Name}";
        }
    }
}
