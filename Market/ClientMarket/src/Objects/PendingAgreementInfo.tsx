export default class PendingAgreementInfo {
  id: number;
  description: string;
  appointeeUsername: string;
  approved: boolean;
  declined: boolean;
  pending: boolean;
  constructor(
    id: number,
    description: string,
    appointeeUsername: string,
    approved: boolean,
    declined: boolean,
    pending: boolean
  ) {
    this.id = id;
    this.description = description;
    this.appointeeUsername = appointeeUsername;
    this.approved = approved;
    this.declined = declined;
    this.pending = pending;
  }
}
