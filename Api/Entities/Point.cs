using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class Point
    {
        public int Id { get; set; }
        public string QuantityPoint { get; set; }

        public DateTime Date { get; set;}

        public int UserId { get; set; }
        public User User { get; set; }
    }
}