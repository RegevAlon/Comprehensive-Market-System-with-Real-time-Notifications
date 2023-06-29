using Market.DataLayer;
using Market.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Review
    {
        private int _id;
        private int _productId;
        private string _user;
        private string _comment;
        private double _rate;
        public int Id { get => _id; }
        public int ProductId { get => _productId; }
        public string User { get => _user; }
        public double Rate { get => _rate; set => _rate = value; }
        public string Comment { get => _comment; set => _comment = value; }

        public Review(int id, int productId, string user, string comment, double rate)
        {
            _id = id;
            _productId = productId;
            _user = user;
            _comment = comment;
            _rate = rate;
        }

        public Review(ReviewDTO reviewDTO)
        {
            _id = reviewDTO.Id;
            _user = reviewDTO.ReviewerUsername;
            _comment = reviewDTO.Comment;
            _rate = reviewDTO.Rate;
            MarketContext context = MarketContext.GetInstance();
            List<ProductDTO> products = context.Products.Where((p)=>p.Reviews.Contains(reviewDTO)).ToList();
            if(products.Count > 0)
            {
                _productId = products.First().Id;
            }
        }

        internal Review Clone()
        {
            int id = _id;
            int productId = _productId;
            string user = _user;
            string comment = _comment;
            double rate = _rate;
            return new Review(id, productId, user, comment, rate);
    }
    }
}
