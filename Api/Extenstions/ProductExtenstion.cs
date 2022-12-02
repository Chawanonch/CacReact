using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;

namespace Api.Extenstions
{
    public static class ProductExtenstion
    {
        public static IQueryable<Product> Sort(this IQueryable<Product> query, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return query.OrderBy(c => c.Name);

            query = orderBy switch
            {
                "price" => query.OrderBy(c => c.Price),
                "priceDesc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Id),
            };

            return query;
        }

        public static IQueryable<Product> Search(this IQueryable<Product> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Product> Filter(this IQueryable<Product> query, string categorys,string sources)
        {
            var cateList = new List<string>();
            var sourList = new List<string>();

            //Split(",") ตัดเครื่องหมาย
            if (!string.IsNullOrEmpty(categorys))
                cateList.AddRange(categorys.ToLower().Split(",").ToList());
            if (!string.IsNullOrEmpty(sources))
                sourList.AddRange(sources.ToLower().Split(",").ToList());

            //กระบวนการวนลูปอยู่ภายใน
            query = query.Where(p => cateList.Count == 0 || cateList.Contains(p.Category.Name.ToLower()));
            query = query.Where(p => sourList.Count == 0 || sourList.Contains(p.Source.Name.ToLower()));

            return query;
        }
    }
}