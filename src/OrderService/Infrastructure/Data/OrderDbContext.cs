using Microsoft.EntityFrameworkCore;
using MassTransit;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MassTransit EF Outbox/Inbox schema
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // CustomerId value object - map as simple column
            entity.OwnsOne(e => e.CustomerId, ownedNav =>
            {
                ownedNav.Property(c => c.Value);
            });

            // Money value object for TotalAmount
            entity.OwnsOne(e => e.TotalAmount, ownedNav =>
            {
                ownedNav.Property(m => m.Amount).HasPrecision(18, 2);
                ownedNav.Property(m => m.Currency).HasMaxLength(3);
            });

            entity.Property(e => e.Status).HasConversion<string>();
            
            // Configure relationship - use backing field for encapsulation
            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            // Metadata for backing field
            var navigation = entity.Metadata.FindNavigation(nameof(Order.Items));
            if (navigation != null)
            {
                navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).HasMaxLength(200);

            // Money value objects for UnitPrice
            entity.OwnsOne(e => e.UnitPrice, ownedNav =>
            {
                ownedNav.Property(m => m.Amount).HasPrecision(18, 2);
                ownedNav.Property(m => m.Currency).HasMaxLength(3);
            });

            // Money value objects for TotalPrice
            entity.OwnsOne(e => e.TotalPrice, ownedNav =>
            {
                ownedNav.Property(m => m.Amount).HasPrecision(18, 2);
                ownedNav.Property(m => m.Currency).HasMaxLength(3);
            });
        });
    }
}
