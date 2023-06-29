using Market.DataLayer;
using Market.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Message
    {
        private string _comment;
        private bool _seen;
        public Message(string comment)
        {
            _comment = comment;
            _seen = false;
        }
        public Message(string comment,bool seen)
        {
            _comment = comment;
            _seen = seen;
        }
        public Message(MessageDTO messageDTO)
        {
            _comment = messageDTO.MessageContent;
            _seen = messageDTO.Seen;
        }

        public string Comment { get => _comment; set => _comment = value; }
        public bool Seen { get => _seen; set => _seen = value; }
    }
}