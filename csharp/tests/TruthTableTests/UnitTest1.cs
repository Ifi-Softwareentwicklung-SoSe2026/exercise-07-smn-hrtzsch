namespace TruthTableTests;

using LogicExpressions;
using TruthTable;

public class UnitTest1
{
    [Fact]
    public void Evaluate_ReturnsAssignedVariableValue()
    {
        var variable = new Variable("a");

        bool result = TruthExpressionEvaluator.Evaluate(variable, new Dictionary<string, bool>
        {
            ["a"] = true,
        });

        Assert.True(result);
    }
}
