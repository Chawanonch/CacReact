using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.RequestHelpers
{
    public class ProductParams : PagianationParams
    {
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Source { get; set; }
    }
}