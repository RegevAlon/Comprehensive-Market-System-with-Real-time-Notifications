export default class PendingAgreement {
  shopId: number;
  appointer: string;
  appointee: string;
  approved: string[];
  declined: string[];
  pendings: string[];
  constructor(
    shopId: number,
    appointer: string,
    appointee: string,
    approved: string[],
    declined: string[],
    pendings: string[]
  ) {
    this.shopId = shopId;
    this.appointer = appointer;
    this.appointee = appointee;
    this.approved = approved;
    this.declined = declined;
    this.pendings = pendings;
  }
}
