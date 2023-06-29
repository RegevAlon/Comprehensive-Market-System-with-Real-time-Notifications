using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public abstract class Event
    {
        private string _name;
        public Event(string name)
        {
            _name = name;
        }

        public string Name { get => _name; set => _name = value; }
        public void Update(Member user)
        {
            user.Notify(GenerateMsg());
        }
        public abstract string GenerateMsg();
    }
}
