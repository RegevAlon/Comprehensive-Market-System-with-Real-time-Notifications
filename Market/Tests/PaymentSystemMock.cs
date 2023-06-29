using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Tests
{
    public interface IPaymentSystem
    {
        bool ProcessPayment(string paymentMethod, decimal amount);
    }

    public class PaymentSystemMock
    {
        private Mock<IPaymentSystem> _mock;

        public PaymentSystemMock()
        {
            _mock = new Mock<IPaymentSystem>();
        }

        public void SetupProcessPayment(string paymentMethod, decimal amount, bool result)
        {
            _mock.Setup(x => x.ProcessPayment(paymentMethod, amount)).Returns(result);
        }

        public IPaymentSystem Object
        {
            get { return _mock.Object; }
        }
    }
}
