using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
   public class DataContext :DbContext
    {
        public DataContext() 
        {
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder) 
        {
            if (!optionBuilder.IsConfigured) 
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity => 
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
                entity.Property(x => x.Username).HasMaxLength(100).IsRequired(true);
                entity.HasIndex(x => x.Username).IsUnique(true);
                entity.Property(x => x.Email).HasMaxLength(320).IsRequired(true);
                entity.Property(x => x.Password).HasMaxLength(512).IsRequired(true);
                entity.Property(x => x.FirstName).HasMaxLength(100);
                entity.Property(x => x.LastName).HasMaxLength(100);
                entity.Property(x => x.DateOfBirth).HasConversion(typeof(string));
                entity.Property(x => x.Address).HasMaxLength(200);
                entity.Property(x => x.UserType).HasConversion(typeof(string));
                entity.Property(x => x.Image).HasMaxLength(500);
                entity.Property(x => x.Verification).HasDefaultValue(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.Name).HasMaxLength(100);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Ingredient).HasMaxLength(300);
                entity.Property(x => x.Price).HasPrecision(8, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.Address).HasMaxLength(200);
                entity.Property(x => x.Comment).HasMaxLength(500);
                entity.Property(x => x.Price).HasPrecision(8, 2);
                entity.Property(x => x.ShippingCost).HasPrecision(8, 2);
                entity.Property(x => x.DurationDelivery).HasConversion(typeof(long));
                entity.Property(x => x.Status).HasDefaultValue(OrderStatus.AwaitingDelivery).HasConversion(typeof(string));

                entity.HasOne(d => d.Custumer)
              .WithMany(p => p.Orders)
              .HasForeignKey(d => d.CustumerId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_User_Oreder");

                entity.HasOne(d => d.Deliverer)
              .WithMany(p => p.Delivery)
              .HasForeignKey(d => d.DelivererId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Deliverer_Order");
            });

            modelBuilder.Entity<OrderProduct>(entity => 
            {
                entity.Property(x => x.Quantity);
               
                entity.HasOne(d => d.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_Oreder");

                entity.HasOne(d => d.Order)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_product");
            });
        }
    }
}
