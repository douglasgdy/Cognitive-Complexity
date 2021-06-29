using System;
using System.Linq;
using CodeMetrics.Calculators.Contracts;
using CodeMetrics.Common;
using CodeMetrics.Parsing.Contracts;
using NUnit.Framework;

namespace CodeMetrics.Calculators.Tests
{
    [TestFixture]
    public class CognitiveComplexityCalculatorTests
    {
        private IComplexityCalculator calculator;
        private IMethodsExtractor extractor;

        [SetUp]
        public void Setup()
        {
            var exceptionHandler = new TestExceptionHandler();
            var windsorContainer = ContainerFactory.CreateContainer(exceptionHandler);
            _ContainerType = ContainerSettings.ContainerType;
            var calculatorFactory = windsorContainer.Resolve<IComplexityCalculatorFactory>();
            calculator = calculatorFactory.Create();
            var extractorFactory = windsorContainer.Resolve<IMethodsExtractorFactory>();
            extractor = extractorFactory.Create();
        }

        private string _ContainerType;
        protected string ContainerType => _ContainerType;

        protected void AssertContainerType(string relevantContainerType)
        {
            if (ContainerType == relevantContainerType)
            {
                return;
            }

            Assert.Inconclusive();
        }

        [Test]
        public void Calculate_MethodWithSinglePath_Return0()
        {
            const string method =
                @"int x = 0;";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_MethodWithTryCatch_Return1()
        {
            const string method =
                @"try { } catch (Exception ex) { }";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_MethodWithTryAndMultipleCatch_Return2()
        {
            const string method =
                @"try { } catch (InvalidOperationException ioe) { } catch (Exception ex) { }";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_NestedForAndIfUsedInTryAndMultipleCatch_Return10()
        {
            const string method =
@"try
{
    if(true)
    {
        int x = 1;
    }
}
catch(InvalidOperationException ex)
{
    for (int i = 0; i < 10; i++)
    {
        if (i % 2 != 0)
        {
            continue;
        }

        Console.WriteLine(i);
    }
}
catch (Exception ex)
{
    if(a)
    {
    }
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(10));
        }

        [Test]
        public void Calculate_MethodWithTryAndMultipleCatchUsedInIfStatement_Return7()
        {
            const string method =
                @"
if(true)
{
    try 
    {
        if(true)
        {
            int x = 1;
        }
    } 
    catch (InvalidOperationException ioe) { } 
    catch (Exception ex) { }
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(7));
        }

        [Test]
        public void Calculate_MethodWithSingleIfWithoutElse_Return1()
        {
            const string method =
                @"if(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_MethodWithSingleIfWithElse_Return2()
        {
            const string method =
                @"if(b)
{
    int x = 1;
}
else
{
    int y = 2;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithIfWithElseIf_Return2()
        {
            const string method =
                @"if(b)
{
    int x = 1;
}
else if(c)
{
    int y = 2;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithIfWithElseIfWithElse_Return3()
        {
            const string method =
                @"if(b)
{
    int x = 1;
}
else if(c)
{
    int y = 2;
}
else
{
    int z = 3;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_MethodWithIfWithElseWithSingleLineIf_Return2()
        {
            const string method =
                @"if(b)
{
    int x = 1;
}
else
    if(c)
    int y = 2;
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithIfWithElseFollowedByIfWithElseIfWithElse_Return6()
        {
            const string method =
                @"if(a)
{
    int x = 1;
}
else 
{
    if(b)
    {
        int y = 2;
    }
    else if(c)
    {
    }
    else
    {
        int z = 3;
    }
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(6));
        }

        [Test]
        public void Calculate_MethodWithIfWithElseFollowedByDoubleIf_Return8()
        {
            const string method =
                @"if(a)
{
    if(b)
    {
        int y = 2;
    }
}
else 
{
    if(c)
    {
        int y = 2;
    }

    if(d)
    {
        int y = 2;
    }
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(8));
        }

        [Test]
        public void Calculate_MethodWithSingleWithAndOperator_Return2()
        {
            const string method =
                @"if(b1 && b2)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithSingleWithOrOperator_Return2()
        {
            const string method =
                @"if(b1 || b2)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithTwoAndOperators_Return2()
        {
            const string method =
                @"if(b1 && b2 && b3)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithAndAndOrOperators_Return3()
        {
            const string method =
                @"if(b1 && b2 || b3)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_MethodWithAndAndOrOperatorsWithBrackets_Return3()
        {
            const string method =
                @"if(b1 && (b2 || b3))
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_MethodWithForLoop_Return1()
        {
            const string method =
                @"for(int i = 0; i < 10; i++)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_MethodWithForLoopIncludingAndOperator_Return2()
        {
            const string method =
                @"for(int i = 0; i < 10 && i > 1; i++)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithForLoopIncludingAndOperatorWithOrOperator_Return3()
        {
            const string method =
                @"for(int i = 0; i < 10 || i > 21 && i < 100; i++)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperator_Return1()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"bool b = b1 && b2;";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorWithNegationOperator_Return2()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"bool b = b1 && !(b2 && b3);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorUsedInBrackets_Return1()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"bool b = b1 && (b2 && b3);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorWithNegationOperator_Return1()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"bool b = b1 && !b2;";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfTripleOperatorsWithNegationOperator_Return3()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"var x = a || b || c; // +1
        var x1 = a && !b && c && d; // +1
        var x2 = !(a && b && c); // +1";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfMixOperatorsUsedInIfStatement_Return4()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method =
                @"if (a // +1 for if
                        && b && c // +1
                        || d || e // +1
                        && f) // +1
                    {
                    }";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(4));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorUsedInIfStatement_Return2()
        {
            const string method =
                @"bool b = b1 && b2;
if(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfOrOperatorUsedInIfStatement_Return2()
        {
            const string method =
                @"bool b = b1 || b2;
if(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorUsedInIfStatementWithNegationOperator_Return2()
        {
            const string method =
                @"bool b = b1 && b2;
if(!b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_ForEachLoop_Return1()
        {
            const string method =
                @"foreach(var item in items)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_WhileLoop_Return1()
        {
            const string method =
                @"while(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfOrOperatorUsedInWhileStatement_Return2()
        {
            const string method =
                @"bool b = b1 || b2;
while(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorUsedInWhileStatement_Return2()
        {
            const string method =
                @"bool b = b1 && b2;
while(b)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_WhileLoopWithAndOperator_Return2()
        {
            const string method =
                @"while(b1 && b2)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_WhileLoopWithAndOperatorWithOrOperator_Return3()
        {
            const string method =
                @"while(b1 && b2 || b3)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_WhileLoopWithAndOperatorWithinParenthesisWithOrOperator_Return3()
        {
            const string method =
                @"while((b1 && b2) || b3)
{
    int x = 1;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_DoWhileLoop_Return1()
        {
            const string method =
                @"do
{
    int x = 1;
} while(b);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_DoWhileLoopWithAndOperator_Return2()
        {
            const string method =
                @"do
{
    int x = 1;
} while(b1 && b2);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_BooleanAssignmentOfAndOperatorUsedInDoWhileStatement_Return2()
        {
            const string method =
                @"bool b = b1 && b2;
do
{
    int x = 1;
}while(b)";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_DoWhileLoopWithAndOperatorWithOrOperator_Return3()
        {
            const string method =
                @"do
{
    int x = 1;
} while(b1 && b2 || b3);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_NestedIf_Return3()
        {
            const string method =
@"if(b1)
{
    if(b2) // +1
    {
        int x = 1;
    }
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_NestedIfWithElseIfWithElse_Return5()
        {
            const string method =
@"if (true) // +1
{
    if (b) // +2 (N=1)
        Console.WriteLine();
    else if (!b) // +1
        Console.WriteLine();
    else // +1
        Console.WriteLine();
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(5));
        }

        [Test]
        public void Calculate_SwitchWithTreeCasesWithoutDefault_Return1()
        {
            const string method =
@"switch (sf)
{
    case 0: break;
    case 1: break;
    case 2: break;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_SwitchWithTreeCasesWithDefault_Return1()
        {
            const string method =
@"switch (sf)
{
    case 0: break;
    case 1: break;
    case 2: break;
    default: break;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_NestedIfUsedInSwitch_Return3()
        {
            const string method =
@"switch (number) // +1
{
    case 1:
        if (true) // +2 (N=1)
            return 1;
        return 11;
    case 2:                   
        return 2;
    case 3:
        return 3;
    default:                  
        return 4;
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_Coalescing_Operator_Return0()
        {
            const string method = @"
object a = null;
object b = a ?? string.Empty";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_NestedIfUsedInLambda_Return2()
        {
            const string method =
@"var task = Task.Run(() =>
{
    if (b) // +2 (N=1)
        Console.WriteLine();
});";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_NestedIfUsedInLambdaUsedInIfStatement_Return4()
        {
            const string method =
@"if(true)
{
    var task = Task.Run(() => // +0 (N=1)
    {
        if (b) // +3 (N=2)
            Console.WriteLine();
    });
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(4));
        }

        [Test]
        public void Calculate_NestedIfUsedInAnonymousMethod_Return2()
        {
            const string method =
@"var task = Task.Run(delegate
{
    if (b) // +2 (N=1)
        Console.WriteLine();
});";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_NestedBinaryExpressionUsedInLinq_Return1()
        {
            const string method =
@"var list = Enumerable.Range(1, 20);
var count = list.Count(x => x % 2 == 0 && x % 3 == 0);
Console.WriteLine(count);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_NestedFlowBreakStatementsWithGotoLabel_Return21()
        {
            const string method =
@"MyLabel:
        for (var i = 0; i < 100; i++) // +1
        {
            foreach (var c in "") // +2 (N=1)
            {
                while (true) // +3 (N=2)
                {
                    do // +4 (N=3)
                    {
                        if (true) // +5 (N=4)
                            goto MyLabel; // +6 (N=5)
                    } while (false);
                }
            }
        }";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(21));
        }

        [Test]
        public void Calculate_NestedIfWithBreakUsedInForeach_Return5()
        {
            const string method =
@"foreach (var c in "") // +1
{
    if (true) // +2 (N=1)
        continue;

    if (false) // +2 (N=1)
        break;

    Console.WriteLine();
}";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(5));
        }

        [Test]
        public void Calculate_RecursiveMethodCall_Return1()
        {
            const string method = @"MyMethod();";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_MethodCallDifferentMethodWithSameName_Return0()
        {
            const string method = @"MyMethod(a);";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_RecursiveMethodCallWithIfWithBinaryExpression_Return3()
        {
            const string method =
@"if (a && !MyMethod()) // +3 (if, and, recursive)
{
    int x = 1;
}
        
int y = 2;
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_RecursiveMethodCallUsedInIfStatement_Return2()
        {
            const string method =
@"if (a) // +1
{
    MyMethod(); // +1
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_MethodWithTernaryOperator_Return1()
        {
            const string method = @"bool a = b ? c : d;";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_NestedTernaryOperatorUsedInIfStatement_Return3()
        {
            const string method = @"
if(true) // +1
{
    bool a = b ? c : d; // +2
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_NestedTernaryOperatorUsedInIfStatement_Return6()
        {
            const string method = @"
if(true) // +1
{
    bool a = b ? c :           // +2 (N=1)
                    d ? e : f; // +3 (N=2)
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(6));
        }

        [Test]
        public void Calculate_SingleObjectInitializer_ShouldReturn0()
        {
            const string method = @"
var p1 = new Person
{
    Name = string.Empty
};";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_MultipleObjectInitializer_ShouldReturn0()
        {
            const string method = @"
var p1 = new Person
{
    Name = string.Empty
};
var p2 = new Person
{
    Name = string.Empty
};";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_ConditionalAccessExpression_ShouldReturn0()
        {
            AssertContainerType(ContainerSettings.ROSLYN_INSTALLER_TYPE_NAME);

            const string method = @"
var numbers = new int[] { 10, 20 };
return numbers?.ToList();
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_ExpressionWithOrOperator_ShouldReturn1()
        {
            const string method = @"
int type = 1;
int? kind = null;
return kind == null || type != 1;
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_SingleObjectInitializerWithBinaryExpression_ShouldReturn0()
        {
            const string method = @"
string name = null;
var p1 = new Person
{
    Name = name ?? string.Empty
};";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_SingleInterfaceWithSingleMethod_ShouldReturn0()
        {
            const string fileCode = @"using System;

namespace SomeNamespace
{
    public interface ISomeInterface
    {
        void SomeMethod(object input);
    }
}";

            var syntaxNodes = extractor.Extract(fileCode);
            var syntaxNode = syntaxNodes.OfType<ISyntaxNode>().FirstOrDefault();

            var complexity = calculator.Calculate(syntaxNode);
            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_SingleAbstractClassWithSingleAbstractMethod_ShouldReturn0()
        {
            const string fileCode = @"using System;

namespace SomeNamespace
{
    public abstract class SomeAbstractClass
    {
        public abstract void SomeMethod(object input);
    }
}";

            var syntaxNodes = extractor.Extract(fileCode);
            var syntaxNode = syntaxNodes.OfType<ISyntaxNode>().FirstOrDefault();

            var complexity = calculator.Calculate(syntaxNode);
            Assert.That(complexity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_MixedFlowBreakStatements_Return35()
        {
            const string method = @"
var TransactionIndex ti = _persistit.getTransactionIndex();
while (true) { // +1
try {
lock (this) {
if (frst != null) { // +2 (nesting = 1)
if (frst.getVersion() > entry.getVersion()) { // +3 (nesting = 2)
throw new RollbackException();
}
if (txn.isActive()) { // +3 (nesting = 2)
for // +4 (nesting = 3)
(Entry e = frst; e != null; e = e.getPrevious()) {
long version = e.getVersion();
long depends = ti.wwDependency(version,
txn.getTransactionStatus(), 0);
if (depends == TIMED_OUT) { // +5 (nesting = 4)
throw new WWRetryException(version);
}
if (depends != 0 // +5 (nesting = 4)
&& depends != ABORTED) { // +1
throw new RollbackException();
}
}
}
}
entry.setPrevious(frst);
frst = entry;
break;
}
} catch (final WWRetryException re) { // +2 (nesting = 1)
try {
long depends = _persistit.getTransactionIndex()
.wwDependency(re.getVersionHandle(),txn.getTransactionStatus(),
SharedResource.DEFAULT_MAX_WAIT_TIME);
if (depends != 0 // +3 (nesting = 2)
&& depends != ABORTED) { // +1
throw new RollbackException();
}
} catch (InterruptedException ie) { // +3 (nesting = 2)
throw new PersistitInterruptedException(ie);
}
} catch (InterruptedException ie) { // +2 (nesting = 1)
throw new PersistitInterruptedException(ie);
}
}
";

            var complexity = CalculateMethodComplexity(method);

            Assert.That(complexity.Value, Is.EqualTo(35));
        }

        private IComplexity CalculateMethodComplexity(string method)
        {
            var fileCode = @"using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNamespace
{
    private class MyPerson
    {
        public string Name { get; set; }
    }

    public class MyClass
    {
        public void MyMethod()
        {
            " + method + @"
        }
    }
}";
            var syntaxNodes = extractor.Extract(fileCode);
            var syntaxNode = syntaxNodes.OfType<ISyntaxNode>().FirstOrDefault();

            return calculator.Calculate(syntaxNode);
        }
    }
}