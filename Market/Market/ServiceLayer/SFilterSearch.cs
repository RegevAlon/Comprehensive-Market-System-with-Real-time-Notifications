using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    [Serializable]
    public class SFilterSearch
    {
        private int _code; // 0 for pricerange, 1 for raterange, 2 for category.
        private int _low;
        private int _high;
        private string _category;

        public SFilterSearch(int code, int low, int high, string category)
        {
            _code = code;
            _low = low;
            _high = high;
            _category = category;
        }

        public int Code { get => _code; set => _code = value; }
        public int Low { get => _low; set => _low = value; }
        public int High { get => _high; set => _high = value; }
        public string Category { get => _category; set => _category = value; }
    }
}
