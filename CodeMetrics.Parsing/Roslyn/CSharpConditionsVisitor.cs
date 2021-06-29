using System;
using CodeMetrics.Parsing.Contracts.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMetrics.Parsing.Roslyn
{
    public class CSharpConditionsVisitor : CSharpSyntaxWalker, ICSharpConditionsVisitor
    {
        private readonly IComplexityMetrics complexityMetrics;

        public CSharpConditionsVisitor()
        {
        }

        public CSharpConditionsVisitor(IComplexityMetrics complexityMetrics)
        {
            if (complexityMetrics == null) throw new ArgumentNullException(nameof(complexityMetrics));

            this.complexityMetrics = complexityMetrics;
        }

        public int Count { get; private set; }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            base.VisitBinaryExpression(node);

            //var increment = complexityMetrics.VisitBinaryExpression(node);
            //Count += increment;

            //switch (node.Kind())
            //{
            //    case SyntaxKind.LogicalOrExpression:
            //    case SyntaxKind.LogicalAndExpression:
            //        ++Count;
            //        break;

            //    case SyntaxKind.CoalesceExpression:
            //        Count += 1;
            //        break;
            //}
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);

            //complexityMetrics.VisitIdentifierName(node);

            //var identifierName = node.Identifier.Text;
            //if (declarationsDictionary.ContainsKey(identifierName))
            //{
            //    var expressionSyntaxNode = declarationsDictionary[identifierName];
            //    Visit(expressionSyntaxNode);
            //}
        }
    }
}