using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Api.Entities.OrderAggregate
{
    [Owned] //Primary key ใช้จากตารางอื่นที่เรียกใช้อีกที
    public class ShippingAddress : Address
    {
        
    }
}