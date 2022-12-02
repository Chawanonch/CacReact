using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int QuantityCode { get; set; }
        public int Percentage { get; set; }
        public DateTime Date { get; set;}
    }
}