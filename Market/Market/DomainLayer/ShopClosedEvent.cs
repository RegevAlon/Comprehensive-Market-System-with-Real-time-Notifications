using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ShopClosedEvent : Event
    {
        private Member _member;

        public ShopClosedEvent(Member member) : base("Shop Open Event")
        {
            _member = member;
        }

        public override string GenerateMsg()
        {
            return $"{Name}: Member: {_member.UserName} Closed the shop.";
        }
    }
}
