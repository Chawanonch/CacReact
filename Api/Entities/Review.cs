using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Star { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }

        public string BuyerId { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string PublicId { get; set; } = string.Empty;

    }
}