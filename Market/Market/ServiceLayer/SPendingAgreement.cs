using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SPendingAgreement
    {
        public int shopId {  get; set; }
        public string appointer { get; set; }
        public string appointee { get; set; }
        public List<string> approved { get; set; }
        public List<string> declined { get; set; }
        public List<string> pendings { get; set; }

        public SPendingAgreement(PendingAgreement agreement)
        {
            this.shopId = agreement.ShopId;
            this.appointer = agreement.Appointer.UserName;
            this.appointee = agreement.Appointee.UserName;
            this.approved = new List<string>();
            this.declined = new List<string>();
            this.pendings = new List<string>();
            foreach (Member member in agreement.Approved)
            {
                approved.Add(member.UserName);
            }
            foreach (Member member in agreement.Declined)
            {
                declined.Add(member.UserName);
            }
            foreach (Member member in agreement.Pendings)
            {
                pendings.Add(member.UserName);
            }
        }
    }
}
