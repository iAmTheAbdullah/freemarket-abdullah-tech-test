using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FreemarketFxAbdullahTask.Tests.Integration;

public class BasketApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<BasketDbContext>));

            services.AddDbContext<BasketDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });

        builder.UseEnvironment("Test");
    }

    public BasketDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<BasketDbContext>();
    }

    public void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
        
        if (!db.DiscountCodes.Any())
        {
            db.DiscountCodes.AddRange(
                new DiscountCode { Id = Guid.NewGuid(), Code = "SAVE10", Percentage = 10, IsActive = true },
                new DiscountCode { Id = Guid.NewGuid(), Code = "SAVE20", Percentage = 20, IsActive = true },
                new DiscountCode { Id = Guid.NewGuid(), Code = "SAVE30", Percentage = 30, IsActive = true }
            );
            db.SaveChanges();
        }
    }
}

