using Market.DomainLayer;
using System;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SMember : SUser
    {
        private string _username;

        public string Username { get => _username; set => _username = value; }

        public SMember(int id, string username) : base(id)
        {
            _username = username;
        }

        public SMember(Member member) : base(member.Id)
        {
            _username = member.UserName;
        }
    }
}