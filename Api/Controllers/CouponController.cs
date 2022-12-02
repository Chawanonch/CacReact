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
    public class CouponController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public CouponController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<Coupon>>> GetCoupon()
        {
            var coupon = await _context.Coupons.OrderBy(c=>c.Id).ToListAsync();
            return Ok(coupon);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetIdCoupon(int id){
            
            var coupon = await _context.Coupons.FindAsync(id);
 
            if (coupon == null) return NotFound();
 
            return Ok(coupon);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Coupon>> CreateCoupon([FromForm] CreateCouponDto couponDto){

            var coupon = _mapper.Map<Coupon>(couponDto); //แมบแบบแปลงชนิดข้อมูล
            coupon.Date = DateTime.Now;

            _context.Coupons.Add(coupon);

            await _context.SaveChangesAsync();

            return Ok(coupon);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Coupon>> UpdateCoupon([FromForm] UpdateCouponDto couponDto){

            var coupon = await _context.Coupons.FindAsync(couponDto.Id);

            if(coupon == null) return NotFound();
            _mapper.Map(couponDto, coupon); //การแมบ แก้ไขใช้รูปแบบนี้(source,destination)

            coupon.Date = DateTime.Now;

            _context.Coupons.Update(coupon);

            await _context.SaveChangesAsync();
            
            return Ok(coupon);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Coupon>> DeleteCoupon(int id){

            var coupon = await _context.Coupons.FindAsync(id);

            if(coupon == null) return NotFound();

            _context.Coupons.Remove(coupon);

            await _context.SaveChangesAsync();
            
            return Ok(coupon);
        }
    }
}