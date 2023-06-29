using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ReportEvent : Event
    {
        private string _username;
        private string _message;

        public ReportEvent(string username, string message) : base("Report Event")
        {
            _username = username;
            _message = message;
        }

        public string Username { get => _username; set => _username = value; }

        public override string GenerateMsg()
        {
            return $"{Name}: From: {_username}, Msg: {_message}";
        }
    }
}
