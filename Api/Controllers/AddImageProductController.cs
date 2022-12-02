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
    public class AddImageProductController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
 
        public AddImageProductController(StoreContext context,IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AddImageProduct>>> GetAddImages()
        {
            var addImage = await _context.AddImageProducts.Include(c=>c.Product).OrderByDescending(c=>c.Date).ToListAsync();
            return Ok(addImage);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddImageProduct>> GetAddImage(int id)
        {
            var addImage = await _context.AddImageProducts.FindAsync(id);
 
            if (addImage == null) return NotFound();
 
            return Ok(addImage);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AddImageProduct>> CrateAddImage([FromForm] CreateAddImageDto addImageDto)
        {
            var addImage = _mapper.Map<AddImageProduct>(addImageDto); //แมบแบบแปลงชนิดข้อมูล
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (addImageDto.File != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(addImageDto.File.FileName);
                var uploadsapp = Path.Combine(wwwRootPath, @"..\..\my-app\public\images\addproducts");

                if (!Directory.Exists(uploadsapp)) Directory.CreateDirectory(uploadsapp);

                //บันทึกรุปภาพใหม่
                using (var fileStreams = new FileStream(Path.Combine(uploadsapp, fileName + extension), FileMode.Create))
                {
                    addImageDto.File.CopyTo(fileStreams);
                }
                addImage.ImageUrl = @"\images\addproducts\" + fileName + extension;
            }

            addImage.Date = DateTime.Now;
            
            await _context.AddImageProducts.AddAsync(addImage);
            await _context.SaveChangesAsync();

            return Ok(addImage);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<AddImageProduct>> UpdateAddImage([FromForm] UpdateAddImageDto addImageDto)
        {
            var addImage = _mapper.Map<AddImageProduct>(addImageDto); //แมบแบบแปลงชนิดข้อมูล
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if(addImage == null) return NotFound();

            if (addImageDto.File != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(addImageDto.File.FileName);
                var uploadsapp = Path.Combine(wwwRootPath, @"..\..\my-app\public\images\addproducts");

                if (!Directory.Exists(uploadsapp)) Directory.CreateDirectory(uploadsapp);

                if (addImage.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, addImage.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploadsapp, fileName + extension), FileMode.Create))
                {
                    addImageDto.File.CopyTo(fileStreams);
                }
                addImage.ImageUrl = @"\images\addproducts\" + fileName + extension;
            }

            addImage.Date = DateTime.Now;

            _context.AddImageProducts.Update(addImage);
            await _context.SaveChangesAsync();

            return Ok(addImage);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<AddImageProduct>> RemoveAddImage(int id)
        {
            var addImage = await _context.AddImageProducts.FindAsync(id);
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if(addImage == null) return NotFound();

            if (addImage.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, addImage.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _context.AddImageProducts.Remove(addImage);
            await _context.SaveChangesAsync();

            return Ok(addImage);
        }
    }
}