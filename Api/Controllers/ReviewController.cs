using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Data;
using Api.Dto;
using Api.Entities;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class ReviewController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageService _imageService;

        public ReviewController(StoreContext context, IMapper mapper,IWebHostEnvironment webHostEnvironment,ImageService imageService)
        {
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Review>>> GetReview()
        {
            var review = await _context.Review.Include(c=>c.Product).OrderByDescending(c=>c.Date).ToListAsync();
            return Ok(review);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Review.FindAsync(id);
 
            if (review == null) return NotFound();
 
            return Ok(review);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Review>> CrateReview([FromForm] CreateReviewDto reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto); //แมบแบบแปลงชนิดข้อมูล
            
            if (reviewDto.File != null)
            {
                var imageResult = await _imageService.AddImageAsync(reviewDto.File);
 
                if (imageResult.Error != null)
                    return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });
 
                review.ImageUrl = imageResult.SecureUrl.ToString();
                review.PublicId = imageResult.PublicId;
            }
            review.Date = DateTime.Now;
            review.BuyerId = User.Identity.Name;
            
            await _context.Review.AddAsync(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Review>> UpdateReview([FromForm] UpdateReviewDto reviewDto)
        {
            var review = await _context.Review.FindAsync(reviewDto.Id);

            if (review == null) return NotFound();
 
            _mapper.Map(reviewDto, review); //การแมบ แก้ไขใช้รูปแบบนี้ (source,destination)
 
            if (reviewDto.File != null) 
            {
                var imageResult = await _imageService.AddImageAsync(reviewDto.File);
 
                if (imageResult.Error != null) 
                    return BadRequest(new ProblemDetails{Title = imageResult.Error.Message});
 
                if (!string.IsNullOrEmpty(review.PublicId)) 
                    await _imageService.DeleteImageAsync(review.PublicId);
 
                review.ImageUrl = imageResult.SecureUrl.ToString();
                review.PublicId = imageResult.PublicId;
            }
            review.Date = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(review);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Review>> RemoveReview(int id)
        {
            var review = await _context.Review.FindAsync(id);

            if(review == null) return NotFound();

            if (!string.IsNullOrEmpty(review.PublicId)) 
                await _imageService.DeleteImageAsync(review.PublicId);

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }
    }
}