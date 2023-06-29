class Bid {
  biddingMember: string;
  pendingRequests: string[];
  approveRequests: string[];
  dissapproveRequests: string[];
  quantity: number;
  bidderApproved: boolean;
  suggestedPrice: number;
  isClosed: boolean;

  constructor(
    biddingMember: string,
    pendingRequests: string[],
    approveRequests: string[],
    dissapproveRequests: string[],
    quantity: number,
    bidderApproved: boolean,
    suggestedPrice: number,
    isClosed: boolean
  ) {
    this.biddingMember = biddingMember;
    this.pendingRequests = pendingRequests;
    this.approveRequests = approveRequests;
    this.dissapproveRequests = dissapproveRequests;
    this.quantity = quantity;
    this.bidderApproved = bidderApproved;
    this.suggestedPrice = suggestedPrice;
    this.isClosed = isClosed;
  }
}

export default Bid;
