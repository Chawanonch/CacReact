using Api.Data;
using Api.Dto;
using Api.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class CategoryController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public CategoryController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            var cate = await _context.Categories.ToListAsync();
            
            return Ok(cate);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetIdCategory(int id){
            
            var cate = await _context.Categories.FindAsync(id);
 
            if (cate == null) return NotFound();
 
            return Ok(cate);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromForm] CreateCategoryDto categoryDto){

            var cate = _mapper.Map<Category>(categoryDto); //แมบแบบแปลงชนิดข้อมูล
            cate.Date = DateTime.Now;

            _context.Categories.Add(cate);

            await _context.SaveChangesAsync();

            return Ok(cate);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Category>> UpdateCategory([FromForm] UpdateCategoryDto categoryDto){

            var cate = await _context.Categories.FindAsync(categoryDto.Id);

            if(cate == null) return NotFound();
            _mapper.Map(categoryDto, cate); //การแมบ แก้ไขใช้รูปแบบนี้(source,destination)

            cate.Date = DateTime.Now;

            _context.Categories.Update(cate);

            await _context.SaveChangesAsync();
            
            return Ok(cate);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id){

            var cate = await _context.Categories.FindAsync(id);

            if(cate == null) return NotFound();

            _context.Categories.Remove(cate);

            await _context.SaveChangesAsync();
            
            return Ok(cate);
        }
    }
}