using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Dto;
using Api.Entities;
using Api.Entities.OrderAggregate;
using Api.Extenstions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Api.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IConfiguration _config;
        public OrderController(StoreContext context,IConfiguration config)
        {
            _config = config;
            _context = context;
        }
 
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders()
        {
            return await _context.Orders
               .ProjectOrderToOrderDto()
               .Where(x => x.BuyerId == User.Identity.Name)
               .OrderByDescending(c=>c.Id)
               .ToListAsync();
        }
 
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(x => x.BuyerId == User.Identity.Name && x.Id == id)
                .FirstOrDefaultAsync();
        }
 
        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto)
        {
            var basket = await _context.Baskets
                .RetrieveBasketWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            if (basket == null) return BadRequest(new ProblemDetails { Title = "Could not locate basket" });
 
            var items = new List<OrderItem>(); //รายการสินค้าสั่งซื้อทั้งหมด
 
            foreach (var item in basket.Items)
            {
                var productItem = await _context.Products.FindAsync(item.ProductId);
                //รายชื่อสินค้าหนึ่งรายการ
                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    ImageUrl = productItem.ImageUrl
                };
 
                //สินค้าที่สั่งซื้อ จำนวนและราคา
                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem); //แอดรายการสินค้าทั้งหมดที่สั่งซื้อ
                productItem.QuantityInStock -= item.Quantity; //ลดจำนวนสินค้าในสต๊อก

                if(productItem.QuantityInStock < 0) return BadRequest(new ProblemDetails { Title = "out of stock" });
            }

            var subtotal = items.Sum(item => item.Price * item.Quantity);
            var deliveryFree = subtotal > 10000 ? 0 : 500;
            
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c=>c.Code.Equals(orderDto.UseCoupon));
            string showTextCoupon;

            if(coupon != null) {
                coupon.QuantityCode -= 1 ;

                if(coupon.QuantityCode < 0 ) {
                    coupon.QuantityCode += 1;
                    showTextCoupon = "Coupon"+ " " + "Out";
                }
                else {
                    //บวกลบจากราคาสินค้าทั้งหมด
                    subtotal -= subtotal * coupon.Percentage / 100;
                    showTextCoupon = "useCoupon" + " " + coupon.Code + " " + coupon.Percentage.ToString() + "%";
                }
            }
            else {
                showTextCoupon = "nouseCoupon";
            }

            //รวบรวม Order,OrderItems
            var order = new Order
            {
                OrderItems = items,
                BuyerId = User.Identity.Name,
                ShippingAddress = orderDto.ShippingAddress,
                Subtotal = subtotal,
                DeliveryFree = deliveryFree,
                //ShowTextCoupon
                UseCoupon = showTextCoupon,
                PaymentIntentId = basket.PaymentIntentId
            };

            _context.Orders.Add(order); //สร้าง Order และ OrderItem ในขั้นตอนเดียว

            _context.Baskets.Remove(basket);  //ลบ  Basket และ BasketItem ในขั้นตอนเดียว
 
            if (orderDto.SaveAddress)   //กรณีต้องการบันทึกที่อยุ่จัดส่งลง UserAddress
            {
                var user = await _context.Users
                    .Include(a => a.Address)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
 
                var address = new UserAddress
                {
                    FullName = orderDto.ShippingAddress.FullName,
                    Address1 = orderDto.ShippingAddress.Address1,
                    Address2 = orderDto.ShippingAddress.Address2,
                    City = orderDto.ShippingAddress.City,
                    State = orderDto.ShippingAddress.State,
                    Zip = orderDto.ShippingAddress.Zip,
                    Country = orderDto.ShippingAddress.Country
                };
                user.Address = address;
            }
 
            //บันทึกทุก transaction ครั้งเดียว
            var result = await _context.SaveChangesAsync() > 0;
 
            if (result) return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);
 
            return BadRequest(new ProblemDetails { Title = "Problem creating order" });
        }
    }
}