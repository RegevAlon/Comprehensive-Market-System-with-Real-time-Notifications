class FilterType {
  ProductRating: boolean;
  PriceRange: boolean;
  Category: boolean;

  constructor(ProductRating: boolean, PriceRange: boolean, Category: boolean) {
    this.ProductRating = ProductRating;
    this.PriceRange = PriceRange;
    this.Category = Category;
  }
}

export default FilterType;
