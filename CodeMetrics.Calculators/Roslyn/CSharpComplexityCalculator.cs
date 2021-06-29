using System;
using System.Collections.Generic;
using CodeMetrics.Calculators.Contracts;
using CodeMetrics.Parsing.Contracts;
using CodeMetrics.Parsing.Contracts.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeMetrics.Calculators.Roslyn
{
    public class CSharpComplexityCalculator : ComplexityCalculatorBase
    {
        public CSharpComplexityCalculator(ICSharpBranchesVisitorFactory branchesVisitorFactory, IComplexityFactory complexityFactory, IExceptionHandler exceptionHandler)
            : base(branchesVisitorFactory, complexityFactory, exceptionHandler)
        {
        }

        private static void AcceptVisitor(IEnumerable<SyntaxNode> blockStatements, ICSharpBranchesVisitor branchesVisitor)
        {
            foreach (var statement in blockStatements)
            {
                branchesVisitor.Visit(statement);
            }
        }

        private static IEnumerable<SyntaxNode> ParseStatements(ISyntaxNodeDeclaration syntaxNode)
        {
            var syntaxNodes = (CSharpSyntaxNode)syntaxNode.UnderlyingSyntaxNode.RawObject;
            var result = new List<CSharpSyntaxNode> { syntaxNodes };
            return result;
        }

        protected override void AcceptVisitor(ISyntaxNodeDeclaration syntaxNode, IBranchesVisitor branchesVisitor)
        {
            var blockStatements = ParseStatements(syntaxNode);
            AcceptVisitor(blockStatements, (ICSharpBranchesVisitor)branchesVisitor);
        }
    }
}