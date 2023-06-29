using Market.RepoLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class ShopRatingFilter : FilterSearchType
    {
        private int _lowRate;
        private int _highRate;
        private List<Shop> _relevantShops;

        public ShopRatingFilter(int lowRate, int highRate)
        {
            _lowRate = lowRate;
            _highRate = highRate;
            _relevantShops = FindRelevantShops();
        }

        protected override bool Predicate(Product product)
        {
            return _relevantShops.ToList().Find((shop) => shop.Id == product.ShopId) != null;
        }

        private List<Shop> FindRelevantShops()
        {
            List<Shop> shops = ShopRepo.GetInstance().GetAll();
            return shops.FindAll((shop) => shop.Rating >= _lowRate && shop.Rating <= _highRate);
        }

    }
}
