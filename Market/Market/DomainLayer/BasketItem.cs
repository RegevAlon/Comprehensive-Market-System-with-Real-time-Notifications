using Market.DataLayer.DTOs;
using Market.RepoLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class BasketItem
    {
        private Product _product;
        private double _priceAfterDiscount;
        private int _quantity;
        public BasketItem(PurchasedItemDTO basketItemDTO)
        {
            _product = ProductRepo.GetInstance().GetById(basketItemDTO.ProductId);
            _priceAfterDiscount = basketItemDTO.PriceAfterDiscount;
            _quantity = basketItemDTO.Quantity;
        }

        public BasketItem(BasketItemDTO basketItemDTO)
        {
            _product = ProductRepo.GetInstance().GetById(basketItemDTO.Product.Id);
            _priceAfterDiscount = basketItemDTO.PriceAfterDiscount;
            _quantity = basketItemDTO.Quantity;
        }

        public BasketItem(Product product, int quantity)
        {
            _product = product;
            _priceAfterDiscount = product.Price;
            _quantity = quantity;
        }

        public BasketItem(Bid bid, Product product)
        {
            _product = product;
            _priceAfterDiscount = bid.SuggestedPrice;
            _quantity = bid.Quantity;
        }

        public BasketItem(Product product, int quantity, double priceAfterDiscount)
        {
            _product = product;
            _priceAfterDiscount = priceAfterDiscount;
            _quantity = quantity;
        }

        public Product Product { get => _product; set => _product = value; }
        public double PriceAfterDiscount { get => _priceAfterDiscount; set => _priceAfterDiscount = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }

        internal BasketItem Clone()
        {
            Product productCoppy = _product.Clone();
            double priceAfterDiscount = _priceAfterDiscount;
            int quantity = _quantity;
            return new BasketItem(productCoppy,quantity,priceAfterDiscount);
        }
    }
}
