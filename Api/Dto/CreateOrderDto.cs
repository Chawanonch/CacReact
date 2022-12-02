using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities.OrderAggregate;

namespace Api.Dto
{
    //เก็บเฉพาะที่อยู่จัดส่ง
    public class CreateOrderDto
    {
        public bool SaveAddress { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public string UseCoupon { get; set; }
        public int? UsePoint { get; set; }
    }
}