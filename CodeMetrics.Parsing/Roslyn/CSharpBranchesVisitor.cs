using System;
using CodeMetrics.Parsing.Contracts.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMetrics.Parsing.Roslyn
{
    public class CSharpBranchesVisitor : CSharpSyntaxWalker, ICSharpBranchesVisitor
    {
        private readonly IComplexityMetrics complexityMetrics;

        public CSharpBranchesVisitor(IComplexityMetrics complexityMetrics)
        {
            if (complexityMetrics == null) throw new ArgumentNullException(nameof(complexityMetrics));

            this.complexityMetrics = complexityMetrics;
            Count = complexityMetrics.DefaultValue;
        }

        public int Count { get; private set; }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            base.VisitBinaryExpression(node);

            //var increment = complexityMetrics.VisitBinaryExpression(node);
            var increment = complexityMetrics.VisitExpression(node);
            Count += increment;
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            base.VisitConditionalAccessExpression(node);

            //var increment = complexityMetrics.VisitConditionalAccessExpression(node);
            var increment = complexityMetrics.VisitExpression(node);
            Count += increment;
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            base.VisitConditionalExpression(node);

            //var increment = complexityMetrics.VisitConditionalExpression(node);
            var increment = complexityMetrics.VisitExpression(node);
            Count += increment;
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            base.VisitDoStatement(node);

            var increment = complexityMetrics.VisitDoStatement(node);
            Count += increment;
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            //complexityMetrics.VisitIfStatementPre(node);

            base.VisitIfStatement(node);

            var increment = complexityMetrics.VisitIfStatement(node);
            Count += increment;
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            base.VisitElseClause(node);

            var increment = complexityMetrics.VisitElseClause(node);
            Count += increment;
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            base.VisitForEachStatement(node);

            var increment = complexityMetrics.VisitForEachStatement(node);
            Count += increment;
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            base.VisitForStatement(node);

            var increment = complexityMetrics.VisitForStatement(node);
            Count += increment;
        }

        //public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        //{
        //    base.VisitInitializerExpression(node);

        //    var increment = complexityMetrics.VisitInitializerExpression(node);
        //    Count += increment;
        //}

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            base.VisitReturnStatement(node);

            var increment = complexityMetrics.VisitReturnStatement(node);
            Count += increment;
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            base.VisitSwitchStatement(node);

            var increment = complexityMetrics.VisitSwitchStatement(node);
            Count += increment;
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            base.VisitSwitchSection(node);

            var increment = complexityMetrics.VisitSwitchSection(node);
            Count += increment;
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            base.VisitTryStatement(node);

            var increment = complexityMetrics.VisitTryStatement(node);
            Count += increment;
        }

        //public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        //{
        //    base.VisitVariableDeclaration(node);

        //    //complexityMetrics.VisitVariableDeclaration(node);
        //}

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            base.VisitWhileStatement(node);

            var increment = complexityMetrics.VisitWhileStatement(node);
            Count += increment;
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            base.VisitGotoStatement(node);

            var increment = complexityMetrics.VisitGotoStatement(node);
            Count += increment;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);

            var increment = complexityMetrics.VisitInvocationExpression(node);
            Count += increment;
        }
    }
}