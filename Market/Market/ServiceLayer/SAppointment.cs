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
    public class SAppointment
    {
        public int id {  get; set; }
        public string member { get; set; }
        public string appointer { get; set; }
        public List<string> appointees { get; set; }
        public string role { get; set; }
        public string permission { get; set; }
        public SAppointment(Appointment appointment)
        {
            member = appointment.Member.UserName;
            if (appointment.Appointer != null)
                appointer = appointment.Appointer.UserName;
            else
                appointer = null;

            appointees = new List<string>();
            foreach (Member member in appointment.Apointees)
            {
                appointees.Add(member.UserName);
            }
            role = appointment.Role.ToString();
            permission = appointment.Permissions.ToString();
        }

    }
}
