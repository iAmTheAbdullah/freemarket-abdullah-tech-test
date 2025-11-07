using FreemarketFxAbdullahTask.Models;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask.Data;

public class BasketDbContext : DbContext
{
    public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options)
    {
    }

    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.DiscountPercentage).HasPrecision(5, 2);
            entity.HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey(bi => bi.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.HasKey(bi => bi.Id);
            entity.Property(bi => bi.Price).HasPrecision(18, 2);
            entity.Property(bi => bi.DiscountPercentage).HasPrecision(5, 2);
            entity.Property(bi => bi.ProductName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(dc => dc.Id);
            entity.Property(dc => dc.Code).IsRequired().HasMaxLength(50);
            entity.Property(dc => dc.Percentage).HasPrecision(5, 2);
            entity.HasIndex(dc => dc.Code).IsUnique();
        });
    }
}

