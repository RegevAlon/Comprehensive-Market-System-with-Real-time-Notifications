using Market.DomainLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Tests
{
    public interface IProductSupplier
    {
        bool CheckProductAvailability(string productName, int quantity);
        void OrderProducts(string productName, int quantity);
    }

    class ProductSupplierMock
    {
        private Mock<IProductSupplier> _mock;

        public ProductSupplierMock()
        {
            _mock = new Mock<IProductSupplier>();
        }

        public void SetupCheckProductAvailability(Product prod, string productName, int quantity, bool result)
        {
            _mock.Setup(x => x.CheckProductAvailability(productName, (prod.Quantity + quantity))).Returns(result);
            prod.Quantity += quantity;
        }

        public void VerifyOrderProducts(string productName, int quantity)
        {
            _mock.Verify(x => x.OrderProducts(productName, quantity), Times.Never);
        }

        public IProductSupplier Object
        {
            get { return _mock.Object; }
        }
    }
}
