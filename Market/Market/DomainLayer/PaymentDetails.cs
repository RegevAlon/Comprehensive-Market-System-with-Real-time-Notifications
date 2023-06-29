using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class PaymentDetails
    {
        string cardNumber;
        string month;
        string year;
        string holder;
        string ccv;
        string id;

        public PaymentDetails()
        {
        }

        public PaymentDetails(string cardNumber, string month, string year, string holder, string ccv, string id)
        {
            this.cardNumber = cardNumber;
            this.month = month;
            this.year = year;
            this.holder = holder;
            this.ccv = ccv;
            this.id = id;
        }

        public string CardNumber { get => cardNumber; set => cardNumber = value; }
        public string Month { get => month; set => month = value; }
        public string Year { get => year; set => year = value; }
        public string Holder { get => holder; set => holder = value; }
        public string Ccv { get => ccv; set => ccv = value; }
        public string Id { get => id; set => id = value; }
    }
}
