using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SUser
    {
        private int _id;
        public int Id { get { return _id; } }

        public SUser(User user)
        {
            _id = user.Id;
        }
        public SUser(int id)
        {
            _id = id;
        }
    }
}
