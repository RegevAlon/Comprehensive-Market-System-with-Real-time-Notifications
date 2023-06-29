class Purchase {
  id: number;
  price: number;
  purchaseStatus: string;

  constructor(id: number, price: number, purchaseStatus: string) {
    this.id = id;
    this.price = price;
    this.purchaseStatus = purchaseStatus;
  }
}

export default Purchase;
