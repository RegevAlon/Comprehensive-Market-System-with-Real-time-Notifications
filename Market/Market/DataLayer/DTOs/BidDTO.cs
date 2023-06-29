using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.DomainLayer;
using Market.DataLayer.DTOs;
using System.Reflection;

namespace Market.DataLayer
{
    [Table("Bids")]
    public class BidDTO
    {
        [Key]
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        [Key]
        [ForeignKey("Members")]
        public int BiddingMemberId { get; set; }

        [Column("Quantity")] // Specify the column name
        public int Quantity { get; set; }

        [Column("BidderApproved")] // Specify the column name
        public bool BidderApproved { get; set; }

        [Column("SuggestedPrice")] // Specify the column name
        public double SuggestedPrice { get; set; }

        public List<BidAnswerDTO> Answers { get; set; }
        public BidDTO()
        {
        }

        public BidDTO(int productId, int biddingMemberId, int qunatity,bool bidderApproved,double suggestedPrice)
        {
            ProductId = productId;
            BiddingMemberId = biddingMemberId;
            BidderApproved = bidderApproved;
            Quantity = qunatity;
            SuggestedPrice = suggestedPrice;
            Answers = new List<BidAnswerDTO>();
        }


        public BidDTO(Bid bid)
        {
            ProductId = bid.ProductId;
            BiddingMemberId = bid.BiddingMember.Id;
            Quantity = bid.Quantity;
            BidderApproved = bid.BidderApproved;
            SuggestedPrice = bid.SuggestedPrice;
            Answers = new List<BidAnswerDTO>();
            foreach(Member owner in bid.OwnersApproved.Keys)
            {
                Answers.Add(new BidAnswerDTO(owner.Id, bid.OwnersApproved[owner].ToString()));
            }
        }
    }
}
