namespace DependencyInjectionApi.Services;

public interface ICalculationService
{
    int CalculateDiscount(int originalPrice, int discountPercentage);
}

public class CalculationService : ICalculationService
{
    public int CalculateDiscount(int originalPrice, int discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        }

        return originalPrice - (originalPrice * discountPercentage / 100);
    }
}