namespace CodeMetrics.Calculators.Contracts
{
    public interface IComplexityCalculatorFactory
    {
        IComplexityCalculator Create();
    }
}