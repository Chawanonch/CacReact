using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class CreateAddImageDto
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}