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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        await SeedDataAsync(app.Services);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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