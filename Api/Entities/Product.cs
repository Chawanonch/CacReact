using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public long Price { get; set; }
        public int QuantityInStock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime Date { get; set;}

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        public int SourceId { get; set; }
        public Source Source { get; set; }

        public string PublicId { get; set; } = string.Empty;
    }
}