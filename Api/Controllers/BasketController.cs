using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Dto;
using Api.Entities;
using Api.Extenstions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetBasket")] //กำหนดชื่อเส้นทางซึ่งสามารถ redirect มาได้
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket(GetbuyerId());
 
            if (basket == null) return NotFound();
 
            return basket.MapBasketToDto();
        }
 
        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            var basket = await RetrieveBasket(GetbuyerId());
 
            if (basket == null) basket = CreateBasket();
 
            var product = await _context.Products.FindAsync(productId);
 
            if (product == null) return BadRequest(new ProblemDetails{Title="Not Product"});
 
            basket.AddItem(product, quantity);
 
            var result = await _context.SaveChangesAsync() > 0;
 
            //Redirect to Route
            if (result) return CreatedAtRoute("GetBasket", basket.MapBasketToDto());
 
            return BadRequest(new ProblemDetails { Title = "Error Add Basket" });
        }
 
        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            var basket = await RetrieveBasket(GetbuyerId());
 
            if (basket == null) return NotFound();
 
            basket.RemoveItem(productId, quantity);
 
            var result = await _context.SaveChangesAsync() > 0;
 
            if (result) return Ok();
 
            return BadRequest(new ProblemDetails { Title = "Error Remove Basket" });
        }
 
        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                return null;
            }
 
            var basket = await _context.Baskets
                   .Include(i => i.Items)
                   .ThenInclude(p => p.Product)
                   .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
            return basket;
        }
        private string GetbuyerId()
        {
            // ?? ถ้าเป็น Null
            return User.Identity.Name ?? Request.Cookies["buyerId"];
        }


        private Basket CreateBasket()
        {
            var buyerId = User.Identity?.Name;
            if(string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(7) };
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }
 
            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
            return basket;
        }
    }
}