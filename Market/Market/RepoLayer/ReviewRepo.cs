using Market.DomainLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.RepoLayer
{
    public class ReviewRepo: IRepo<Review>
    {
        //<reviewId, Review>
        private static ConcurrentDictionary<int, Review> _reviews;
        private static ReviewRepo _reviewRepo = null;

        private ReviewRepo()
        {
            _reviews = new ConcurrentDictionary<int, Review>();
        }
        public static ReviewRepo GetInstance()
        {
            if (_reviewRepo == null)
                _reviewRepo = new ReviewRepo();
            return _reviewRepo;
        }

        public void Add(Review item)
        {
            _reviews.TryAdd(item.Id, item);
        }

        public bool ContainsID(int id)
        {
            return _reviews.ContainsKey(id);
        }

        public bool ContainsValue(Review item)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Review> GetAll()
        {
            return _reviews.Values.ToList();
        }

        public Review GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Review item)
        {
            throw new NotImplementedException();
        }
        public SynchronizedCollection<Review> GetProductReviews(int productId)
        {
            SynchronizedCollection<Review> reviews = new SynchronizedCollection<Review>();
            foreach(Review item in _reviews.Values)
            {
                if (item.ProductId == productId)
                    reviews.Add(item);
            }
            return reviews;
        }

        public void Clear()
        {
            _reviews.Clear();
        }
        public void ResetDomainData()
        {
            _reviews.Clear();
        }
    }
}
