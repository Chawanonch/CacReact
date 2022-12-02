using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class ReviewImage
    {
        public string Id { get; set; }
        public int ReviewId { get; set; }
        public string Image { get; set; }
    }
}