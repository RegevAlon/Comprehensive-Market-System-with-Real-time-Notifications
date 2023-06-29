using Market.DomainLayer;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    public class SBidSell
    {
        //<bidder_member_id,Bid>
        private List<SBid> bids { get; set; }
        public SBidSell(BidSell bidSell)
        {
            bids = new List<SBid>();
            foreach(Bid bid in  bidSell.Bids.Values)
            {
                bids.Add(new SBid(bid));
            }
        }
    }
}