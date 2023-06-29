using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public enum Category
    {
        Furnitures = 1,
        Cars = 2,
        Food = 4,
        Pockemon = 8,
        Electiricity = 16,
        None = 32,
        All = Furnitures | Cars | Food | Pockemon | Electiricity | None,
    }
}
