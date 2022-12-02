using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class UpdateCouponDto
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        [Range(1,1000)]
        public int QuantityCode { get; set; }
        [Required]
        [Range(1,99)]
        public int Percentage { get; set; }
    }
}