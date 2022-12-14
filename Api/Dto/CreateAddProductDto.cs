using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class CreateAddProductDto
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}