namespace CodeMetrics.Calculators.Contracts
{
    public interface IComplexityFactory
    {
        IComplexity Create(int value);
    }
}