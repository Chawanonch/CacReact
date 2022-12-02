using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class CreateCategoryDto
    {
        [Required]
        public string? Name { get; set; }
    }
}