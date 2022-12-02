using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.RequestHelpers
{
    public class PagianationParams
    {
        private const int MaxPageSize = 50; //ค่า Default จำนวนสูงสุดต่อหน้า
        private int _pageSize = 8; //ค่า Default จำนวนต่อหน้า
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}