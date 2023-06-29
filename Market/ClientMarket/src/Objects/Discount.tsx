import Shop from "./Shop";
import Expression from "./Expression";

class Discount extends Expression {
  constructor(tag: string) {
    super(tag);
  }
  public toString = (shop: Shop): string => {
    return `discount`;
  };
}

export default Discount;
