using CodeMetrics.Parsing.Contracts.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CodeMetrics.Parsing.Roslyn
{
    public class CognitiveComplexityMetrics : IComplexityMetrics
    {
        private static readonly SyntaxKind[] BinaryLogicalExpression = new SyntaxKind[] { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };
        private readonly NestingCalculator nestingCalculator;

        public CognitiveComplexityMetrics()
        {
            nestingCalculator = new NestingCalculator();
        }

        public int DefaultValue { get; }

        public int VisitExpression(ExpressionSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LogicalOrExpression
                    when HasParent(node, SyntaxKind.LogicalAndExpression) || HasParent(node, SyntaxKind.LogicalNotExpression) || !HasParent(node, BinaryLogicalExpression):
                case SyntaxKind.LogicalAndExpression
                    when HasParent(node, SyntaxKind.LogicalOrExpression) || HasParent(node, SyntaxKind.LogicalNotExpression) || !HasParent(node, BinaryLogicalExpression):
                    return 1;

                case SyntaxKind.ConditionalExpression:
                    return 1 + nestingCalculator.GetNesting(node);

                default:
                    return 0;
            }
        }

        public int VisitDoStatement(DoStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitIfStatement(IfStatementSyntax node)
        {
            // blow is for the 'else if' clause
            if (node.Parent.Kind() == SyntaxKind.ElseClause)
                return 0;

            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitElseClause(ElseClauseSyntax node)
        {
            return 1;
        }

        public int VisitForEachStatement(ForEachStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitForStatement(ForStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitReturnStatement(ReturnStatementSyntax node)
        {
            return 0;
        }

        public int VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitSwitchSection(SwitchSectionSyntax node)
        {
            return 0;
        }

        public int VisitTryStatement(TryStatementSyntax node)
        {
            return node.Catches.Count * (1 + nestingCalculator.GetNesting(node));
        }

        public int VisitWhileStatement(WhileStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            return 0;
        }

        public int VisitGotoStatement(GotoStatementSyntax node)
        {
            return 1 + nestingCalculator.GetNesting(node);
        }

        public int VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            return HasRecursion(node) ? 1 : 0;
        }

        private bool HasParent(ExpressionSyntax node, params SyntaxKind[] expectedKinds)
        {
            var kind = node.Parent.Kind();
            switch (kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return HasParent((ParenthesizedExpressionSyntax)node.Parent, expectedKinds);

                case SyntaxKind.LogicalNotExpression
                    when !expectedKinds.Contains(kind):
                    return HasParent((PrefixUnaryExpressionSyntax)node.Parent, expectedKinds);

                default:
                    return expectedKinds.Contains(kind);
            }
        }

        private bool HasRecursion(InvocationExpressionSyntax node)
        {
            var invokeIdentifierNode = node.Expression as IdentifierNameSyntax;
            if (invokeIdentifierNode == null)
                return false;

            var invokeName = invokeIdentifierNode.Identifier.Text;
            var invokeParameterCount = node.ArgumentList.Arguments.Count;
            var methodNode = FindMethodNode(node);
            var methodName = methodNode.Identifier.Text;
            var methodParameterCount = methodNode.ParameterList.Parameters.Count;

            return methodName == invokeName && methodParameterCount == invokeParameterCount;
        }

        private MethodDeclarationSyntax FindMethodNode(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return (MethodDeclarationSyntax)node;

                default:
                    return FindMethodNode((CSharpSyntaxNode)node.Parent);
            }
        }

        private class NestingCalculator
        {
            public int GetNesting(CSharpSyntaxNode node)
            {
                CalculateNestingAndEnqueue(node);

                try
                {
                    return GetNestingAndDequeue(node);
                }
                finally
                {
                    CleanUp();
                }
            }

            private readonly Stack<NestedNode> nestedNodes = new Stack<NestedNode>();

            private void CalculateNestingAndEnqueue(CSharpSyntaxNode node)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.IfStatement:
                    case SyntaxKind.ForStatement:
                    case SyntaxKind.ForEachStatement:
                    case SyntaxKind.WhileStatement:
                    case SyntaxKind.DoStatement:
                    case SyntaxKind.SwitchStatement:
                    case SyntaxKind.ConditionalExpression:
                    case SyntaxKind.GotoStatement:
                    case SyntaxKind.CatchClause:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                        if (!IsNestedNodeCalculated(node))
                        {
                            CalculateNestingAndEnqueue((CSharpSyntaxNode)node.Parent);
                            CalculateNestedNode(node);
                        }
                        break;

                    case SyntaxKind.MethodDeclaration:
                        break;

                    default:
                        // including try, else, block nodes
                        CalculateNestingAndEnqueue((CSharpSyntaxNode)node.Parent);
                        break;
                }
            }

            private bool IsNestedNodeCalculated(CSharpSyntaxNode node)
            {
                if (nestedNodes.Count > 0)
                {
                    var top = nestedNodes.Peek();
                    if (top.Node == node)
                    {
                        return true;
                    }
                }

                return false;
            }

            private void CalculateNestedNode(CSharpSyntaxNode node)
            {
                if (nestedNodes.Count > 0)
                {
                    var top = nestedNodes.Peek();
                    var nesting = top.Nesting + 1;
                    nestedNodes.Push(new NestedNode(node, nesting));
                }
                else
                {
                    nestedNodes.Push(new NestedNode(node, 0));
                }
            }

            private int GetNestingAndDequeue(CSharpSyntaxNode node)
            {
                if (nestedNodes.Count == 0)
                {
                    return 0;
                }

                var top = nestedNodes.Peek();
                if (node != top.Node)
                {
                    return node.Kind() == SyntaxKind.TryStatement ? top.Nesting + 1 : 0;
                }

                top = nestedNodes.Pop();
                return top.Nesting;
            }

            private void CleanUp()
            {
                if (nestedNodes.Count == 0)
                    return;

                var top = nestedNodes.Peek();
                switch (top.Node.Kind())
                {
                    case SyntaxKind.CatchClause:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                        nestedNodes.Pop();
                        CleanUp();
                        break;
                }
            }

            [System.Diagnostics.DebuggerDisplay("node: {Node.Kind()} nesting: {Nesting}")]
            private struct NestedNode
            {
                public NestedNode(CSharpSyntaxNode node, int nesting)
                {
                    Node = node;
                    Nesting = nesting;
                }

                public CSharpSyntaxNode Node { get; }
                public int Nesting { get; }
            }
        }
    }
}