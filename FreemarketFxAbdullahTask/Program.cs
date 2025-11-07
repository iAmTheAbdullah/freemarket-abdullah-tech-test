using FreemarketFxAbdullahTask.Data;
using FreemarketFxAbdullahTask.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FreemarketFxAbdullahTask;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<BasketDbContext>(options =>
            options.UseInMemoryDatabase("BasketDb"));

        builder.Services.AddCommandHandlers();
        builder.Services.AddQueryHandlers();

        builder.Services.AddControllers();

        var app = builder.Build();

        if (!app.Environment.IsEnvironment("Test"))
        {
            await SeedDataAsync(app.Services);
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }

    private static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

        if (!await context.DiscountCodes.AnyAsync())
        {
            context.DiscountCodes.AddRange(
                new Models.DiscountCode { Id = Guid.NewGuid(), Code = "SAVE10", Percentage = 10, IsActive = true },
                new Models.DiscountCode { Id = Guid.NewGuid(), Code = "SAVE20", Percentage = 20, IsActive = true },
                new Models.DiscountCode { Id = Guid.NewGuid(), Code = "SAVE30", Percentage = 30, IsActive = true }
            );

            await context.SaveChangesAsync();
        }
    }
}