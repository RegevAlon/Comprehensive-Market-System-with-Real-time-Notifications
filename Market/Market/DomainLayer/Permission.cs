using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public enum Permission
    {
        ManageSupply = 1,
        Appoint = 2,
        Policy = 4,
        UserApplications = 8,
        ShopPurchaseHistory = 16,
        ShopAppointmentsInfo = 32,
        OpenCloseShop = 64,
        BidsPermissions = 128,
        All = ManageSupply| Appoint| Policy| UserApplications| ShopPurchaseHistory| ShopAppointmentsInfo| OpenCloseShop| BidsPermissions
    }
}
