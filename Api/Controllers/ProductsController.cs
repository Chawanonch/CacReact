using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Dto;
using Api.Entities;
using Api.Extenstions;
using Api.RequestHelpers;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;

        public ProductsController(StoreContext context, IMapper mapper,ImageService imageService)
        {
            _imageService = imageService;
            _context = context;
            _mapper = mapper;
        }
 
        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var query = _context.Products
            .Sort(productParams.OrderBy)
            .Search(productParams.SearchTerm)
            .Filter(productParams.Category, productParams.Source)
            .Include(c=>c.Category)
            .Include(c=>c.Source)
            .AsQueryable();
            var products = await PagedList<Product>.ToPagedList(query,
            productParams.PageNumber, productParams.PageSize);
            Response.AddPaginationHeader(products.MetaData);
            return Ok(products);
        }
 
        [HttpGet("{id}" ,Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
            .Include(x=>x.Category)
            .Include(x=>x.Source)
            .FirstAsync(x=>x.Id.Equals(id));
            //SingleOrDefaultAsync(x => x.Id == id)
            if (product == null) return NotFound();
 
            return Ok(product);
        }
        
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            //อ่านค่าซ้ำกันแค่ค่าเดียว
            var category = await _context.Products.Select(p => p.Category.Name).Distinct().ToListAsync();
            var source = await _context.Products.Select(p => p.Source.Name).Distinct().ToListAsync();
            
            return Ok(new { category , source });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] CreateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto); //แมบแบบแปลงชนิดข้อมูล

            if (productDto.File != null)
            {
                var imageResult = await _imageService.AddImageAsync(productDto.File);
 
                if (imageResult.Error != null)
                    return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });
 
                product.ImageUrl = imageResult.SecureUrl.ToString();
                product.PublicId = imageResult.PublicId;
            }
            product.Date = DateTime.Now;

            _context.Products.Add(product);

            var result = await _context.SaveChangesAsync() > 0;
 
            if (result) return CreatedAtRoute("GetProduct", new { id = product.Id }, product.Id);
 
            return BadRequest(new ProblemDetails { Title = "Problem creating new product" });
        } 

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProduct([FromForm] UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(productDto.Id);
 
            if (product == null) return NotFound();
 
            _mapper.Map(productDto, product); //การแมบ แก้ไขใช้รูปแบบนี้ (source,destination)
 
            if (productDto.File != null) 
            {
                var imageResult = await _imageService.AddImageAsync(productDto.File);
 
                if (imageResult.Error != null) 
                    return BadRequest(new ProblemDetails{Title = imageResult.Error.Message});
 
                if (!string.IsNullOrEmpty(product.PublicId)) 
                    await _imageService.DeleteImageAsync(product.PublicId);
 
                product.ImageUrl = imageResult.SecureUrl.ToString();
                product.PublicId = imageResult.PublicId;
            }
            product.Date = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;
 
            if (result) return Ok(product);
 
            return BadRequest(new ProblemDetails { Title = "Problem updating product" });

        } 
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id){

            var product = await _context.Products.FindAsync(id);
 
            if (product == null) return NotFound();
 
            if (!string.IsNullOrEmpty(product.PublicId)) 
                await _imageService.DeleteImageAsync(product.PublicId);
 
            _context.Products.Remove(product);
 
            var result = await _context.SaveChangesAsync() > 0;
 
            if (result) return Ok();
 
            return BadRequest(new ProblemDetails { Title = "Problem deleting product" });
        }
    }
}