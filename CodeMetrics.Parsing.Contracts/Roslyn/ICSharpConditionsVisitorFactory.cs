namespace CodeMetrics.Parsing.Contracts.Roslyn
{
    public interface ICSharpConditionsVisitorFactory
    {
        ICSharpConditionsVisitor Create();

        ICSharpConditionsVisitor Create(IComplexityMetrics complexityMetrics);
    }
}