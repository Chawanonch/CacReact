using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Dto;
using Api.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AddProductController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
 
        public AddProductController(StoreContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AddProduct>>> GetProducts()
        {
            var addProduct = await _context.AddProducts
            .Include(x=>x.Product)
                .ThenInclude(x=>x.Source)
            .Include(x=>x.Product)
                .ThenInclude(x=>x.Category)
            .Include(x=>x.Company)
            .OrderByDescending(d=>d.Id)
            .ToListAsync();
            return Ok(addProduct);
        }
 
        [HttpGet("{id}")]
        public async Task<ActionResult<AddProduct>> GetProduct(int id)
        {
            var addProduct = await _context.AddProducts.FindAsync(id);
 
            if (addProduct == null) return NotFound();
 
            return Ok(addProduct);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AddProduct>> CreateAddProduct([FromForm] CreateAddProductDto addProductDto){

            var addProduct = _mapper.Map<AddProduct>(addProductDto); //แมบแบบแปลงชนิดข้อมูล

            var product = await _context.Products.FirstOrDefaultAsync(c=>c.Id.Equals(addProductDto.ProductId));
            
            product.QuantityInStock += addProductDto.Quantity;
            _context.Products.Update(product);

            addProduct.Date = DateTime.Now;

            await _context.AddProducts.AddAsync(addProduct);
            await _context.SaveChangesAsync();
            return Ok(addProduct);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<AddProduct>> DeleteAddProduct(int id){

            var addProduct = await _context.AddProducts.FindAsync(id);

            var product = await _context.Products.FirstOrDefaultAsync(c=>c.Id.Equals(addProduct.ProductId));

            if(addProduct == null) return NotFound();

            product.QuantityInStock -= addProduct.Quantity;
            _context.Products.Update(product);

            if(product.QuantityInStock <= 0) return NotFound("จำนวนที่ลบมากกว่าจำนวนสินค้า");

            _context.AddProducts.Remove(addProduct);
            await _context.SaveChangesAsync();
            return Ok(addProduct);
        }
    }
}