using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Api.Dto
{
    public class UpdateReviewDto
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        
        [Required]
        public int Star { get; set; }
        public IFormFile? File { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}