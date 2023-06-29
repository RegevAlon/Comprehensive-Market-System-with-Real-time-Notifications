using Market.DomainLayer;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;

namespace Market.ServiceLayer
{
    public class SBid
    {
        public string biddingMember { get; set; }
        public List<string> pendingRequests { get; set; }
        public List<string> approveRequests { get; set; }
        public List<string> dissapproveRequests { get; set; }
        public int quantity { get; set; }
        public bool bidderApproved { get; set; }
        public double suggestedPrice { get; set; }
        public bool isClosed { get; set; }

        public SBid(Bid bid)
        {
            biddingMember = bid.BiddingMember.UserName;
            pendingRequests = new List<string>();
            approveRequests = new List<string>();
            dissapproveRequests = new List<string>();
            foreach (Member m in bid.OwnersApproved.Keys)
            {
                if (bid.OwnersApproved[m] == BidAccept.Pending)
                    pendingRequests.Add(m.UserName);
                if (bid.OwnersApproved[m] == BidAccept.Approved)
                    approveRequests.Add(m.UserName);
                if (bid.OwnersApproved[m] == BidAccept.Dissapproved)
                    dissapproveRequests.Add(m.UserName);

            }
            quantity = bid.Quantity;
            bidderApproved = bid.BidderApproved;
            suggestedPrice = bid.SuggestedPrice;
            isClosed = bid.AllApproved();
        }









    }
}