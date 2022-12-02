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
    public class CompanyController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public CompanyController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Company>>> GetCompany()
        {
            var company = await _context.Companies.ToListAsync();
            
            return Ok(company);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetIdCompany(int id){
            
            var company = await _context.Companies.FindAsync(id);
 
            if (company == null) return NotFound();
 
            return Ok(company);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany([FromForm] CreateCompanyDto companyDto){

            var company = _mapper.Map<Company>(companyDto); //แมบแบบแปลงชนิดข้อมูล
            company.Date = DateTime.Now;

            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            return Ok(company);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Company>> UpdateCompany([FromForm] UpdateCompanyDto companyDto){

            var company = await _context.Companies.FindAsync(companyDto.Id);

            if(company == null) return NotFound();
            _mapper.Map(companyDto, company); //การแมบ แก้ไขใช้รูปแบบนี้(source,destination)

            company.Date = DateTime.Now;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            
            return Ok(company);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Company>> DeleteCompany(int id){

            var company = await _context.Companies.FindAsync(id);

            if(company == null) return NotFound();

            _context.Companies.Remove(company);

            await _context.SaveChangesAsync();
            
            return Ok(company);
        }
    }
}