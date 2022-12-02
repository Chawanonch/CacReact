using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class StoreContext : IdentityDbContext<User, Role, int> //เปลี่ยนการสืบทอดเป็น Identity
    {
        public StoreContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<AddProduct> AddProducts { get; set; }
        public DbSet<AddImageProduct> AddImageProducts { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        

        //จะสร้าง OrderItem ให้เองเป็นผลมาจาก public List<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public object Reviews { get; internal set; }

        //สร้างข้อมูลเริ่มต้นให้กับ Role
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(a => a.Address)
                .WithOne()
                .HasForeignKey<UserAddress>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>()
               .HasData(
                   new Role { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                   new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
               );
        }

    }
}