using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Api.Dto
{
    public class CreateProductDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Detail { get; set; }

        [Required]
        [Range(100, Double.PositiveInfinity)]
        public long Price { get; set; }

        [Required]
        [Range(1, 100)]
        public int QuantityInStock { get; set; }
        
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SourceId { get; set; }

    }
}