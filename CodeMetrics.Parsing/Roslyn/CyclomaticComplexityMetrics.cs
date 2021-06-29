using CodeMetrics.Parsing.Contracts.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeMetrics.Parsing.Roslyn
{
    public class CyclomaticComplexityMetrics : IComplexityMetrics
    {
        //private readonly ICSharpConditionsVisitorFactory conditionsVisitorFactory;
        //private readonly Dictionary<string, ExpressionSyntax> declarationsDictionary = new Dictionary<string, ExpressionSyntax>();
        //private ICSharpConditionsVisitor conditionsVisitor;

        public CyclomaticComplexityMetrics()
        {
            DefaultValue = 1;
            //if (conditionsVisitorFactory == null) throw new ArgumentNullException(nameof(conditionsVisitorFactory));

            //this.conditionsVisitorFactory = conditionsVisitorFactory;
        }

        public int DefaultValue { get; }

        public int VisitExpression(ExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.ConditionalAccessExpression:
                    return 1;

                case SyntaxKind.ConditionalExpression:
                    return 2;

                default:
                    return 0;
            }
        }

        //public int VisitBinaryExpression(BinaryExpressionSyntax node)
        //{
        //    switch (node.Kind())
        //    {
        //        case SyntaxKind.LogicalOrExpression:
        //        case SyntaxKind.LogicalAndExpression:
        //        case SyntaxKind.CoalesceExpression:
        //            return 1;
        //        default:
        //            return 0;
        //    }
        //    //return node.OperatorToken.Kind() != SyntaxKind.QuestionQuestionToken ? 0 : 1;
        //}

        //public int VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        //{
        //    if (node.OperatorToken.Kind() != SyntaxKind.QuestionToken)
        //        return 0;
        //    return 1;
        //}

        //public int VisitConditionalExpression(ConditionalExpressionSyntax node)
        //{
        //    if (node.QuestionToken.Kind() != SyntaxKind.QuestionToken || node.ColonToken.Kind() != SyntaxKind.ColonToken)
        //        return 0;
        //    return 2;
        //}

        public int VisitDoStatement(DoStatementSyntax node)
        {
            int increment = 1;
            //var conditionComplexity = GetConditionComplexity(node.Condition);
            //increment += conditionComplexity;
            return increment;
        }

        public int VisitElseClause(ElseClauseSyntax node)
        {
            return 1;
        }

        public int VisitForEachStatement(ForEachStatementSyntax node)
        {
            return 1;
        }

        public int VisitForStatement(ForStatementSyntax node)
        {
            int increment = 1;
            //var conditionComplexity = GetConditionComplexity(node.Condition);
            //increment += conditionComplexity;
            return increment;
        }

        public int VisitIfStatement(IfStatementSyntax node)
        {
            int increment = 1;
            //var conditionComplexity = GetConditionComplexity(node.Condition);
            //increment += conditionComplexity;
            return increment;
        }

        public int VisitReturnStatement(ReturnStatementSyntax node)
        {
            var binaryExpressionSyntax = node.Expression as BinaryExpressionSyntax;
            if (binaryExpressionSyntax == null) return 0;

            int increment = 1;
            //var conditionComplexity = GetConditionComplexity(binaryExpressionSyntax);
            //increment += conditionComplexity;
            return increment;
        }


        public int VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return 0;
        }


        public int VisitSwitchSection(SwitchSectionSyntax node)
        {
            return !IsDefaultCase(node) ? 1 : 0;
        }

        public int VisitTryStatement(TryStatementSyntax node)
        {
            return node.Catches.Count;
        }

        //public void VisitVariableDeclaration(VariableDeclarationSyntax node)
        //{
        //    InitDeclarationsDictionary(node);
        //}

        public int VisitWhileStatement(WhileStatementSyntax node)
        {
            int increment = 1;
            //var conditionComplexity = GetConditionComplexity(node.Condition);
            //increment += conditionComplexity;
            return increment;
        }

        public int VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            return 0;
            //var binaryExpressions = node?.DescendantNodes()?.OfType<BinaryExpressionSyntax>();
            //if (binaryExpressions != null && binaryExpressions.Any())
            //{
            //    Count++;
            //    foreach (var binaryExpression in binaryExpressions)
            //    {
            //        var conditionComplexity = GetConditionComplexity(binaryExpression);
            //        Count += conditionComplexity;
            //    }
            //}
        }

        //public int VisitBinaryExpression(BinaryExpressionSyntax node)
        //{
        //switch (node.Kind())
        //{
        //    case SyntaxKind.LogicalOrExpression:
        //    case SyntaxKind.LogicalAndExpression:
        //    case SyntaxKind.CoalesceExpression:
        //        return 1;
        //}
        //}

        //public void VisitIdentifierName(IdentifierNameSyntax node)
        //{
        //    var identifierName = node.Identifier.Text;
        //    if (declarationsDictionary.ContainsKey(identifierName))
        //    {
        //        var expressionSyntaxNode = declarationsDictionary[identifierName];
        //        conditionsVisitor.Visit(expressionSyntaxNode);
        //    }
        //}

        private static bool IsDefaultCase(SwitchSectionSyntax switchSection)
        {
            var firstCaseLabel = switchSection.Labels.OfType<DefaultSwitchLabelSyntax>().FirstOrDefault();
            return firstCaseLabel != null;
        }

        public int VisitGotoStatement(GotoStatementSyntax node)
        {
            return 0;
        }

        public int VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            return 0;
        }

        //public void VisitIfStatementPre(IfStatementSyntax node)
        //{
        //    throw new System.NotImplementedException();
        //}
        //private void CreateConditionsVisitor()
        //{
        //    if (conditionsVisitor == null)
        //    {
        //        conditionsVisitor = conditionsVisitorFactory.Create(this);
        //    }
        //}

        //private int GetConditionComplexity(Microsoft.CodeAnalysis.SyntaxNode node)
        //{
        //    CreateConditionsVisitor();
        //    conditionsVisitor.Visit(node);
        //    return conditionsVisitor.Count;
        //}

        //private void InitDeclarationsDictionary(VariableDeclarationSyntax variableDeclaration)
        //{
        //    if (variableDeclaration?.Variables == null) return;

        //    foreach (var variable in variableDeclaration.Variables)
        //    {
        //        var binaryExpressionSyntax = variable.Initializer?.DescendantNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();
        //        if (binaryExpressionSyntax != null)
        //        {
        //            declarationsDictionary[variable.Identifier.Text] = binaryExpressionSyntax;
        //        }
        //    }
        //}
    }
}