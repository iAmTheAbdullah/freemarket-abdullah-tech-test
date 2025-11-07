using FreemarketFxAbdullahTask.Commands;
using FreemarketFxAbdullahTask.Commands.Handlers;
using FreemarketFxAbdullahTask.Queries;
using FreemarketFxAbdullahTask.Queries.Handlers;
using FreemarketFxAbdullahTask.Services;

namespace FreemarketFxAbdullahTask.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IVatCalculationService, VatCalculationService>();
        services.AddScoped<IBasketCalculationService, BasketCalculationService>();
        services.AddScoped<IBasketMapper, BasketMapper>();
        
        services.AddScoped<ICommandHandler<CreateBasketCommand, Guid>, CreateBasketCommandHandler>();
        services.AddScoped<ICommandHandler<AddItemCommand, Guid>, AddItemCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveItemCommand, bool>, RemoveItemCommandHandler>();
        services.AddScoped<ICommandHandler<ApplyDiscountCodeCommand, bool>, ApplyDiscountCodeCommandHandler>();

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetBasketQuery, Models.Basket?>, GetBasketQueryHandler>();
        services.AddScoped<IQueryHandler<GetBasketTotalQuery, decimal>, GetBasketTotalQueryHandler>();

        return services;
    }
}


