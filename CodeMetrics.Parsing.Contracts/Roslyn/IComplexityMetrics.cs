using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMetrics.Parsing.Contracts.Roslyn
{
    public interface IComplexityMetrics
    {
        int DefaultValue { get; }
        //int VisitBinaryExpression(BinaryExpressionSyntax node);
        int VisitExpression(ExpressionSyntax node);
        //int VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node);

        //int VisitConditionalExpression(ConditionalExpressionSyntax node);

        int VisitDoStatement(DoStatementSyntax node);

        int VisitElseClause(ElseClauseSyntax node);

        int VisitForEachStatement(ForEachStatementSyntax node);

        int VisitForStatement(ForStatementSyntax node);

        //void VisitIfStatementPre(IfStatementSyntax node);
        int VisitIfStatement(IfStatementSyntax node);

        int VisitInitializerExpression(InitializerExpressionSyntax node);

        int VisitReturnStatement(ReturnStatementSyntax node);

        int VisitSwitchStatement(SwitchStatementSyntax node);
        int VisitSwitchSection(SwitchSectionSyntax node);

        int VisitTryStatement(TryStatementSyntax node);

        //void VisitVariableDeclaration(VariableDeclarationSyntax node);

        int VisitWhileStatement(WhileStatementSyntax node);
        int VisitGotoStatement(GotoStatementSyntax node);
        int VisitInvocationExpression(InvocationExpressionSyntax node);
        //int VisitBinaryExpression(BinaryExpressionSyntax node);
        //void VisitIdentifierName(IdentifierNameSyntax node);
    }
}