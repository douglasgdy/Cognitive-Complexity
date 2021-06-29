using System;
using System.Collections.Generic;
using CodeMetrics.Calculators.Contracts;
using CodeMetrics.Parsing.Contracts;
using CodeMetrics.Parsing.Contracts.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace CodeMetrics.Calculators.NRefactory
{
    public class AstCyclomaticComplexityCalculator : ComplexityCalculatorBase
    {
        public AstCyclomaticComplexityCalculator(IAstBranchesVisitorFactory branchesVisitorFactory, IComplexityFactory complexityFactory, IExceptionHandler exceptionHandler)
            : base(branchesVisitorFactory, complexityFactory, exceptionHandler)
        {
        }

        private static void AcceptVisitors(IEnumerable<Statement> blockStatements, IAstVisitor branchesVisitor)
        {
            foreach (var statement in blockStatements)
            {
                statement.AcceptVisitor(branchesVisitor);
            }
        }

        private static IEnumerable<Statement> ParseStatements(ISyntaxNodeDeclaration syntaxNode)
        {
            var blockStatement = (BlockStatement)syntaxNode.UnderlyingSyntaxNode.RawObject;
            return blockStatement.Statements;
        }

        protected override void AcceptVisitor(ISyntaxNodeDeclaration syntaxNode, IBranchesVisitor branchesVisitor)
        {
            var blockStatements = ParseStatements(syntaxNode);
            AcceptVisitors(blockStatements, (IAstBranchesVisitor)branchesVisitor);
        }
    }
}