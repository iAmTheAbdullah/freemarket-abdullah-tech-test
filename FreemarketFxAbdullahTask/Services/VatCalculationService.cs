namespace FreemarketFxAbdullahTask.Services;

public class VatCalculationService : IVatCalculationService
{
    private const decimal VatRate = 0.20m;

    public decimal CalculateVat(decimal amount)
    {
        return Math.Round(amount * VatRate, 2);
    }

    public decimal AddVat(decimal amount)
    {
        return Math.Round(amount * (1 + VatRate), 2);
    }
}

