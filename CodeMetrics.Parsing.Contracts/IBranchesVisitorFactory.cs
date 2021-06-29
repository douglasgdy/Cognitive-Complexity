namespace CodeMetrics.Parsing.Contracts
{
    public interface IBranchesVisitorFactory<out T> where T : IBranchesVisitor
    {
        T Create();
    }
}