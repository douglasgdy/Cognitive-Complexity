using System;
using CodeMetrics.Calculators.Contracts;
using CodeMetrics.Parsing.Contracts;

namespace CodeMetrics.Calculators
{
    public abstract class ComplexityCalculatorBase : IComplexityCalculator
    {
        private readonly IBranchesVisitorFactory<IBranchesVisitor> branchesVisitorFactory;
        private readonly IComplexityFactory complexityFactory;
        private readonly IExceptionHandler exceptionHandler;

        protected ComplexityCalculatorBase(IBranchesVisitorFactory<IBranchesVisitor> branchesVisitorFactory, IComplexityFactory complexityFactory, IExceptionHandler exceptionHandler)
        {
            if (branchesVisitorFactory == null) throw new ArgumentNullException(nameof(branchesVisitorFactory));
            if (complexityFactory == null) throw new ArgumentNullException(nameof(complexityFactory));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            this.branchesVisitorFactory = branchesVisitorFactory;
            this.complexityFactory = complexityFactory;
            this.exceptionHandler = exceptionHandler;
        }

        public IComplexity Calculate(ISyntaxNode syntaxNode)
        {
            try
            {
                return TryCalculate(syntaxNode);
            }
            catch (Exception ex)
            {
                exceptionHandler.HandleException(ex);
                var branchesVisitor = branchesVisitorFactory.Create();
                return CreateComplexity(branchesVisitor);
            }
        }

        protected abstract void AcceptVisitor(ISyntaxNodeDeclaration syntaxNode, IBranchesVisitor branchesVisitor);

        private IComplexity CreateComplexity(IBranchesCounter branchesVisitor)
        {
            var complexity = branchesVisitor.Count;
            return complexityFactory.Create(complexity);
        }

        private IComplexity TryCalculate(ISyntaxNodeDeclaration syntaxNode)
        {
            var branchesVisitor = branchesVisitorFactory.Create();
            AcceptVisitor(syntaxNode, branchesVisitor);
            return CreateComplexity(branchesVisitor);
        }
    }
}