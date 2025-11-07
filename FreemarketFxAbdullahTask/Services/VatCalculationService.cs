namespace FreemarketFxAbdullahTask.Services;

public class VatCalculationService : IVatCalculationService
{
    private const decimal VatRate = 0.20m;

    public decimal CalculateVat(decimal amount)
    {
        return amount * VatRate;
    }

    public decimal AddVat(decimal amount)
    {
        return amount * (1 + VatRate);
    }
}

