namespace FreemarketFxAbdullahTask.Services;

public interface IVatCalculationService
{
    decimal CalculateVat(decimal amount);
    decimal AddVat(decimal amount);
}

