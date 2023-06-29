using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Guest : User
    {
        public Guest(int id) : base(id)
        {
        }
    }
}
