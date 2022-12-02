using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class CreateReviewDto
    {
        public string? Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Star { get; set; }
        
        [Required]
        public IFormFile? File { get; set; }
        
        [Required]
        public int ProductId { get; set; }
    }
}