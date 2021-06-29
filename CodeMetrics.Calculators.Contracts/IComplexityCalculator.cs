using CodeMetrics.Parsing.Contracts;

namespace CodeMetrics.Calculators.Contracts
{
    public interface IComplexityCalculator
    {
        IComplexity Calculate(ISyntaxNode syntaxNode);
    }
}