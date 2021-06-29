using Microsoft.CodeAnalysis;

namespace CodeMetrics.Parsing.Contracts.Roslyn
{
    public interface ICSharpConditionsVisitor : IConditionsVisitor//IBranchesVisitor
    {
        void Visit(SyntaxNode node);
    }
}