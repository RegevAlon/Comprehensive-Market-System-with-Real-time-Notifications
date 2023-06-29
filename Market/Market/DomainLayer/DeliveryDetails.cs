using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DomainLayer
{
    public class DeliveryDetails
    {
        string name;
        string address;
        string city;
        string country;
        string zip;

        public DeliveryDetails()
        {
        }

        public DeliveryDetails(string name, string address, string city, string country, string zip)
        {
            this.name = name;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }

        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => address = value; }
        public string City { get => city; set => city = value; }
        public string Country { get => country; set => country = value; }
        public string Zip { get => zip; set => zip = value; }
    }
}
