using Market.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SReview
    {
        private string _username;
        private string _comment;
        private double _rate;

        public SReview(Review review)
        {
            _username = review.User;
            _comment = review.Comment;
            _rate = review.Rate;
        }

        public SReview(string username, string comment, double rate)
        {
            _username = username;
            _comment = comment;
            _rate = rate;
        }

        public string Username { get => _username; set => _username = value; }
        public string Comment { get => _comment; set => _comment = value; }
        public double Rate { get => _rate; set => _rate = value; }
    }
}
