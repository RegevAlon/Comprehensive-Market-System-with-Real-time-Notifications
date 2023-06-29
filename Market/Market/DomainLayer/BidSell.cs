using Market.DataLayer;
using Market.DataLayer.DTOs;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class BidSell : ISell
    {
        //<bidder_member_id,Bid>
        private ConcurrentDictionary<int, Bid> _bids;
        public BidSell()
        {
            _bids = new ConcurrentDictionary<int, Bid>();
        }

        public ConcurrentDictionary<int, Bid> Bids { get => _bids; set => _bids = value; }

        public bool CanAddToCart()
        {
            throw new Exception("Cannot add to cart bid product");
        }

        public bool CanBid()
        {
            return true;
        }
        public Bid RemoveBid(int userId)
        {
            if (_bids.ContainsKey(userId))
            {
                _bids.TryRemove(userId, out Bid bid);
                DeleteBidFromDataBase(bid);
                return bid;
            }
            else throw new Exception("Bid did not found.");
        }

        public void AddBid(Bid newBid)
        {
            if (_bids.ContainsKey(newBid.BiddingMember.Id))
                throw new Exception("You already have a bid on this product.");
            _bids.TryAdd(newBid.BiddingMember.Id, newBid);
        }
        private void DeleteBidFromDataBase(Bid bid)
        {
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Bids.Where(b=>b.ProductId == bid.ProductId && b.BiddingMemberId==bid.BiddingMember.Id).FirstOrDefault();
            if (bidDTO!=null)
            {
                foreach(BidAnswerDTO ans in bidDTO.Answers)
                {
                    context.BidAnswers.Remove(ans);
                }
                context.Bids.Remove(bidDTO);
                context.SaveChanges();
            }
        }

        public List<Bid> RemoveOwnerFromBid(string username)
        {
            List<Bid> newApprovedBids = new List<Bid>();
            foreach(Bid bid in _bids.Values)
            {
                bool oldApprovedStatus = bid.AllApproved();
                bid.RemoveOwnerFromBid(username);
                bool newApprovedStatus = bid.AllApproved();
                if (!oldApprovedStatus && newApprovedStatus)
                    newApprovedBids.Add(bid);
            }
            return newApprovedBids;
        }

        internal void AddOwnerToBid(Member owner)
        {
            foreach (Bid bid in _bids.Values)
            {
                bid.AddOwnerToBid(owner);
            }
        }

        public void ApproveBid(int userId, string username)
        {
            Bid userBid = GetBid(userId);
            userBid.ApproveBid(username);
        }

        public Bid GetBid(int userId)
        {
            if (_bids.ContainsKey(userId))
                return _bids[userId];
            else throw new Exception($"Bid was not found for userId {userId}");
        }

        public double OfferCounterBid(int userId, string userName, double counterPrice)
        {
            Bid bid = GetBid(userId);
            return bid.OfferCounterBid(userName,counterPrice);  //Change the biiding price and return the old bidding price
        }

        public void ApproveCounterBid(int memberId)
        {
            Bid bid = GetBid(memberId);
            bid.ApproveCounterBid();

        }
        public void DissapproveBid(int userId, string userName)
        {
            Bid bid = GetBid(userId);
            bid.DissapproveBid(userName);
        }
    }
}
