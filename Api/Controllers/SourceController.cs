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
    public class SourceController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public SourceController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Source>>> GetSource()
        {
            var sour = await _context.Sources.ToListAsync();
            
            return Ok(sour);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Source>> GetIdSource(int id){
            
            var sour = await _context.Sources.FindAsync(id);
 
            if (sour == null) return NotFound();
 
            return Ok(sour);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Source>> CreateSource([FromForm] CreateSourceDto sourceDto){

            var sour = _mapper.Map<Source>(sourceDto); //แมบแบบแปลงชนิดข้อมูล
            
            sour.Date = DateTime.Now;

            _context.Sources.Add(sour);

            await _context.SaveChangesAsync();

            return Ok(sour);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Source>> UpdateSource([FromForm] UpdateSourceDto sourceDto){

            var sour = await _context.Sources.FindAsync(sourceDto.Id);

            if(sour == null) return NotFound();
            _mapper.Map(sourceDto, sour); //การแมบ แก้ไขใช้รูปแบบนี้(source,destination)

            sour.Date = DateTime.Now;

            _context.Sources.Update(sour);

            await _context.SaveChangesAsync();
            
            return Ok(sour);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Source>> DeleteSource(int id){

            var sour = await _context.Sources.FindAsync(id);

            if(sour == null) return NotFound();

            _context.Sources.Remove(sour);

            await _context.SaveChangesAsync();
            
            return Ok(sour);
        }
    }
}