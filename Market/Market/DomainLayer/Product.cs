using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.RepoLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class Product
    {
        private int _id;
        private int _shopId;
        private string _name;
        private double _price;
        private int _quantity;
        private Category _category;
        private SynchronizedCollection<string> _keywords;
        private string _description;
        private SynchronizedCollection<Review> _reviews;
        private ISell _sellMethod;

        public int Id { get => _id; }
        public int ShopId { get => _shopId; }
        public string Name { get => _name; set => _name = value; }
        public double Price { get => _price; set => _price = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public Category Category { get => _category; set => _category = value; }
        public string Description { get => _description; set => _description = value; }
        public SynchronizedCollection<string> Keywords { get => _keywords; set => _keywords = value; }
        public SynchronizedCollection<Review> Reviews { get => _reviews; set => _reviews = value; }
        public ISell SellMethod { get => _sellMethod; set => _sellMethod = value; }

        public Product(int id, int shopId, string name,ISell sellMethod, string description, double price, Category category, int quantity, SynchronizedCollection<string> keywords)
        {
            _id = id;
            _shopId = shopId;
            _name = name;
            _description = description;
            _price = price;
            _quantity = quantity;
            _category = category;
            _keywords = keywords;
            _reviews = ReviewRepo.GetInstance().GetProductReviews(_id);
            _sellMethod = sellMethod;
        }

        //Constructure for tests - Default Sell Method
        public Product(int id, int shopId, string name, string description, double price, Category category, int quantity, SynchronizedCollection<string> keywords)
        {
            _id = id;
            _shopId = shopId;
            _name = name;
            _description = description;
            _price = price;
            _quantity = quantity;
            _category = category;
            _keywords = keywords;
            _reviews = ReviewRepo.GetInstance().GetProductReviews(_id);
            _sellMethod = new RegularSell();
        }

        public Product(int id, int shopId, string name,ISell sellMethod, string description, double price, Category category, int quantity, SynchronizedCollection<string> keywords, SynchronizedCollection<Review> reviews)
        {
            _id = id;
            _shopId = shopId;
            _name = name;
            _description = description;
            _price = price;
            _quantity = quantity;
            _category = category;
            _keywords = keywords;
            _reviews = reviews;
            _sellMethod = sellMethod;
        }

        public Product(ProductDTO pdto)
        {

            _id = pdto.Id;
            _description = pdto.Description;
            _name = pdto.Name;
            _price = pdto.Price;
            _quantity = pdto.Quantity;
            Enum.TryParse<Category>(pdto.Category, out var _category);
            _keywords = new SynchronizedCollection<string>();
            foreach(string key in pdto.Keywords.Split(" ,"))
            {
                _keywords.Add(key);
            }
            _reviews = new SynchronizedCollection<Review>();
            foreach (ReviewDTO reviewDTO in pdto.Reviews) _reviews.Add(new Review(reviewDTO));
            MarketContext context = MarketContext.GetInstance();
            List<ShopDTO> shops = context.Shops.AsNoTracking().Where(
                (s) => s.Products.Where((p) => p.Id == _id).Count() > 0).ToList();
            if (pdto.Id == -1)
                _shopId = -1;
            else if (shops.Count() > 0)
            {
                _shopId = shops.First().Id;
            }
            else throw new Exception("Could not find shop that has this product");
            if (pdto.SellMethod == "BidSell")
            {
                _sellMethod = new BidSell();
                foreach (BidDTO bidDto in pdto.Bids)
                {
                    ((BidSell)_sellMethod).Bids.TryAdd(bidDto.BiddingMemberId, new Bid(bidDto));
                }
            }
            else _sellMethod = new RegularSell();
        }

        public bool ContainKeyword(string keyWord)
        {
            return _keywords.ToList().Find((key) => key.ToLower().Equals(keyWord.ToLower())) != null;
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---------------------------");
            sb.AppendLine(string.Format("Product ID: %d", _id));
            sb.AppendLine(string.Format("Shop ID: %d", _shopId));
            sb.AppendLine(string.Format("Product Description: %s", _description));
            sb.AppendLine(string.Format("Quantity in stock: %d", _quantity));
            sb.AppendLine(string.Format("Catagroy: %s", _category.ToString()));
            sb.AppendLine("---------------------------");
            return sb.ToString();
        }
        /// <summary>
        /// checks if user added a review before, if no creating a new review and adding to prosuct reviews.
        /// </summary>
        /// <param name="userId"></param> userId that want to add review
        /// <param name="username"></param>
        /// <param name="comment"></param>
        /// <param name="rate"></param>
        /// <exception cref="Exception"></exception> if user added a review on this product before.
        public void AddReview(int userId, string username, string comment, double rate)
        {
            int unicReviewId = int.Parse($"{_id}{userId}");
            Review review = GetReview(unicReviewId);
            if (review == null && ValidCommentReview(comment) && ValidRateReview(rate))
            {
                Review reviewToAdd = new Review(unicReviewId, _id, username, comment, rate);
                _reviews.Add(reviewToAdd);
                ProductRepo.GetInstance().Update(this);
            }
            else throw new Exception("User already added a review.");
        }

        private Review GetReview(int unicReviewId)
        {
            return _reviews.ToList().Find((p) => p.Id == unicReviewId);
        }

        public double GetRate()
        {
            int rateNumber = _reviews.Count();
            double rateSum = 0;

            if (rateNumber <= 0) return 0;

            foreach (Review review in _reviews)
            {
                rateSum += review.Rate;
            }
            return rateSum / rateNumber;
        }

        private bool ValidCommentReview(string comment)
        {
            if (comment.Length == 0) throw new Exception("Invalid Empty Comment");
            return true;
        }
        private bool ValidRateReview(double rate)
        {
            if (rate < 0 || rate > 5) throw new Exception("Invalid Rate: Should be 0-5");
            return true;
        }
        public bool HasCategory(Category category)
        {
            return (_category & category) == category;
        }

        internal Product Clone()
        {
            int id = _id;
            int shopId = _shopId;
            string name = _name;
            double price = _price;
            int quantity = _quantity;
            ISell sellMethod = _sellMethod;
            Category category = _category;
            SynchronizedCollection<string> keywords = new SynchronizedCollection<string>();
            SynchronizedCollection<Review> reviews = new SynchronizedCollection<Review>();
            string description = _description;
            foreach (string keyword in _keywords)
                keywords.Add(keyword);
            foreach (Review review in _reviews)
                reviews.Add(review.Clone());
            return new Product(id, shopId, name, sellMethod, description, price, category, quantity, keywords, reviews);
        }

        public override string ToString()
        {
            return _name;
        }
        public bool CanBid()
        {
            return _sellMethod.CanBid();
        }
        public bool CanAddToCart()
        {
            return _sellMethod.CanAddToCart();
        }
    }
}
