using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Api.Services
{
    public class PaymentService
    {
         private readonly IConfiguration _config;
        private readonly StoreContext _context;
        public PaymentService(IConfiguration config,StoreContext context)
        {
            _context = context;
            _config = config;
        }
 
        //สร้างรหัสการชำระเงิน
        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
 
            var service = new PaymentIntentService();
 
            var intent = new PaymentIntent();
                        
            var subtotal = basket.Items.Sum(item => item.Quantity * item.Product.Price);
            var deliveryFee = subtotal > 10000 ? 0 : 500;
            
	        //สร้างรายการใหม่
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = subtotal + deliveryFee,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
            }
            else //อัพเดทรายการเดิม
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = subtotal + deliveryFee
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
 
            return intent;
        }

    }
}