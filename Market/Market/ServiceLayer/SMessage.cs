using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    public class SMessage
    {
        private string comment { get; set; }
        private bool seen { get; set; }
        public SMessage(Message msg)
        {
            comment = msg.Comment;
            seen = msg.Seen;
        }
    }
}