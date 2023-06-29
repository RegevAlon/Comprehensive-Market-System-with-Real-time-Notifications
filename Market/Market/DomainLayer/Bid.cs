using Market.DataLayer;
using Market.RepoLayer;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;

namespace Market.DomainLayer
{
    public class Bid
    {
        private int _productId;
        private Member _biddingMember;
        private ConcurrentDictionary<Member, BidAccept> _ownersApproved;
        private int _quantity;
        private bool _bidderApproved;
        private double _suggestedPrice;

        public Bid(int productId,Member biddingMember, List<Member> ownersApproved, int quantity, double suggestedPrice)
        {
            _productId = productId;
            _biddingMember = biddingMember;
            _ownersApproved = new ConcurrentDictionary<Member, BidAccept>(
                ownersApproved.ToDictionary(owner => owner, _ => BidAccept.Pending));
            _quantity = quantity;
            _bidderApproved = true;
            _suggestedPrice = suggestedPrice;
        }

        public Bid(BidDTO bidDto)
        {
            _productId = bidDto.ProductId;
            _biddingMember = MemberRepo.GetInstance().GetById(bidDto.BiddingMemberId);
            _ownersApproved = new ConcurrentDictionary<Member, BidAccept>();
            _suggestedPrice = bidDto.SuggestedPrice;
            _quantity = bidDto.Quantity;
            _bidderApproved = bidDto.BidderApproved;
            foreach(BidAnswerDTO ans in bidDto.Answers)
            {
                _ownersApproved.TryAdd(MemberRepo.GetInstance().GetById(ans.OwnerId),CastBidAccept(ans.Answer));
            }
        }

        private BidAccept CastBidAccept(string status)
        {
            BidAccept result;

            if (Enum.TryParse(status, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Invalid status value.");
            }
        }



        public Member BiddingMember { get => _biddingMember; set => _biddingMember = value; }
        public ConcurrentDictionary<Member, BidAccept> OwnersApproved { get => _ownersApproved; set => _ownersApproved = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public bool BidderApproved { get => _bidderApproved; set => _bidderApproved = value; }
        public double SuggestedPrice { get => _suggestedPrice; set => _suggestedPrice = value; }
        public int ProductId { get => _productId; set => _productId = value; }

        public void RemoveOwnerFromBid(string username)
        {
            Member owner = GetMember(username);
            _ownersApproved.TryRemove(owner, out _);

            //======================Update DB========================
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b)=>b.ProductId == _productId && b.BiddingMemberId==_biddingMember.Id);
            if (bidDTO != null)
            {
                BidAnswerDTO ans = bidDTO.Answers.Find(a => a.OwnerId == owner.Id);
                if (ans != null)
                {
                    bidDTO.Answers.Remove(ans);
                    context.SaveChanges();
                }
            }
            //========================================================
        }

        public void AddOwnerToBid(Member owner)
        {
            _ownersApproved.TryAdd(owner, BidAccept.Pending);

            //======================Update DB========================
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b) => b.ProductId == _productId && b.BiddingMemberId == _biddingMember.Id);
            if (bidDTO != null)
            {
                bidDTO.Answers.Add(new BidAnswerDTO(owner.Id, BidAccept.Pending.ToString()));
                context.SaveChanges();
            }
            //========================================================
        }

        public bool AllApproved()
        {
            return _ownersApproved.Values.All((v) => v == BidAccept.Approved) && _bidderApproved;
        }

        public void ApproveBid(string username)
        {
            Member owner = GetMember(username);
            _ownersApproved[owner] = BidAccept.Approved;

            //======================Update DB========================
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b) => b.ProductId == _productId && b.BiddingMemberId == _biddingMember.Id);
            if (bidDTO != null)
            {
                BidAnswerDTO ans = bidDTO.Answers.Find(a => a.OwnerId == owner.Id);
                ans.Answer = _ownersApproved[owner].ToString();
                context.SaveChanges();
            }
            //========================================================
        }
        public double OfferCounterBid(string userName, double counterPrice)
        {
            double oldPrice;
            Member member = GetMember(userName);
            foreach (Member key in _ownersApproved.Keys.ToList())
            {
                _ownersApproved[key] = BidAccept.Pending;
            }
            if (_ownersApproved.ContainsKey(member))
            {
                _ownersApproved[member] = BidAccept.Approved;
            }
            oldPrice = _suggestedPrice;
            _suggestedPrice = counterPrice;
            _bidderApproved = false;
            UpdateToDatabase();
            return oldPrice;
        }

        private void UpdateToDatabase()
        {
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b) => b.ProductId == _productId && b.BiddingMemberId == _biddingMember.Id);
            if (bidDTO != null)
            {
                foreach (Member key in _ownersApproved.Keys.ToList())
                {
                    BidAnswerDTO ans = bidDTO.Answers.Find(a => a.OwnerId == key.Id);
                    if (ans != null) ans.Answer = _ownersApproved[key].ToString();
                }
                bidDTO.SuggestedPrice = _suggestedPrice;
                bidDTO.BidderApproved = _bidderApproved;
                context.SaveChanges();
            }
        }

        private Member GetMember(string username)
        {
            foreach (Member m in _ownersApproved.Keys)
            {
                if (m.UserName.Equals(username))
                    return m;
            }
            throw new Exception("owner username did not found in bid");
        }

        public void ApproveCounterBid()
        {
            _bidderApproved = true;

            //======================Update DB========================
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b) => b.ProductId == _productId && b.BiddingMemberId == _biddingMember.Id);
            if (bidDTO != null)
            {
                bidDTO.BidderApproved = true;
                context.SaveChanges();
            }
            //========================================================
        }

        public void DissapproveBid(string userName)
        {
            Member owner = GetMember(userName);
            _ownersApproved[owner] = BidAccept.Dissapproved;

            //======================Update DB========================
            MarketContext context = MarketContext.GetInstance();
            BidDTO bidDTO = context.Products.Find(_productId).Bids.Find((b) => b.ProductId == _productId && b.BiddingMemberId == _biddingMember.Id);
            if (bidDTO != null)
            {
                BidAnswerDTO ans = bidDTO.Answers.Find(a => a.OwnerId == owner.Id);
                ans.Answer = _ownersApproved[owner].ToString();
                context.SaveChanges();
            }
            //========================================================
        }
    }
}