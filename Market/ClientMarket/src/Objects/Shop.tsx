import Product from "./Product";
import Appointment from "./Appointment";
import RuleInfo from "./Rules/RuleInfo";
import PurchasePolicyInfo from "./Policies/PurchasePolicyInfo";
import DiscountPolicyInfo from "./Policies/DiscountPolicyInfo";
import Purchase from "./Purchase";
import PendingAgreement from "./PendingAgreement";

export default class Shop {
  id: number;
  name: string;
  products: Product[];
  isOpen: boolean;
  purchasePolicies: PurchasePolicyInfo[];
  discountPolicies: DiscountPolicyInfo[];
  appointments: Appointment[];
  rules: RuleInfo[];
  purchases: Purchase[];
  pendingAgreements: PendingAgreement[];
  rating: number;

  constructor(
    Id: number,
    Name: string,
    Products: Product[],
    isOpen: boolean,
    purchasePolicies: PurchasePolicyInfo[],
    discountPolicies: DiscountPolicyInfo[],
    appointments: Appointment[],
    rules: RuleInfo[],
    purchases: Purchase[],
    pendingAgreements: PendingAgreement[],
    rate: number
  ) {
    this.id = Id;
    this.name = Name;
    this.products = Products;
    this.isOpen = isOpen;
    this.purchasePolicies = purchasePolicies;
    this.discountPolicies = discountPolicies;
    this.appointments = appointments;
    this.rules = rules;
    this.purchases = purchases;
    this.pendingAgreements = pendingAgreements;
    this.rating = rate;
  }
}
