namespace TruthTableTests;

using System.Diagnostics;
using Input;
using LogicExpressions;
using TruthTable;

public sealed class TruthTableModuleTests
{
    private readonly TruthTermInputModule inputModule = new();

    [Fact]
    public void And_ReturnsTrueOnlyWhenBothOperandsAreTrue()
    {
        IExpressionNode expression = inputModule.Parse("A AND B").RootClause;

        Assert.False(Evaluate(expression, false, false));
        Assert.False(Evaluate(expression, false, true));
        Assert.False(Evaluate(expression, true, false));
        Assert.True(Evaluate(expression, true, true));
    }

    [Fact]
    public void Or_ReturnsFalseOnlyWhenBothOperandsAreFalse()
    {
        IExpressionNode expression = inputModule.Parse("A OR B").RootClause;

        Assert.False(Evaluate(expression, false, false));
        Assert.True(Evaluate(expression, false, true));
        Assert.True(Evaluate(expression, true, false));
        Assert.True(Evaluate(expression, true, true));
    }

    [Fact]
    public void Not_InvertsOperand()
    {
        IExpressionNode expression = inputModule.Parse("NOT A").RootClause;

        Assert.True(Evaluate(expression, false));
        Assert.False(Evaluate(expression, true));
    }

    [Fact]
    public void Parentheses_ChangeEvaluationOrder()
    {
        IExpressionNode grouped = inputModule.Parse("A AND (B OR C)").RootClause;
        IExpressionNode ungrouped = inputModule.Parse("A AND B OR C").RootClause;

        var assignments = new Dictionary<string, bool>
        {
            ["A"] = false,
            ["B"] = false,
            ["C"] = true,
        };

        Assert.False(TruthExpressionEvaluator.Evaluate(grouped, assignments));
        Assert.True(TruthExpressionEvaluator.Evaluate(ungrouped, assignments));
    }

    [Fact]
    public void EmptyInput_ThrowsClearParserError()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => inputModule.Parse(""));

        Assert.Contains("Unexpected end", exception.Message);
    }

    [Fact]
    public void MissingClosingParenthesis_ThrowsClearParserError()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => inputModule.Parse("A AND (B OR C"));

        Assert.Contains("Missing closing parenthesis", exception.Message);
    }

    [Fact]
    public void InvalidCharacter_ThrowsClearParserError()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => inputModule.Parse("A & B"));

        Assert.Contains("Invalid character '&'", exception.Message);
    }

    [Theory]
    [InlineData("A XOR B")]
    [InlineData("A NAND B")]
    [InlineData("A NOR B")]
    public void UnsupportedOperators_ThrowNotImplementedException(string expression)
    {
        Assert.Throws<NotImplementedException>(() => inputModule.Parse(expression));
    }

    [Fact]
    public void MatrixGenerator_CreatesAllAssignmentsExactlyOnce()
    {
        LogicExpressions.TruthTerm term = inputModule.Parse("A AND (B OR NOT C)");

        TruthTableMatrix matrix = TruthTableMatrixGenerator.Generate(term);

        string[] expectedAssignments =
        [
            "A=0,B=0,C=0",
            "A=0,B=0,C=1",
            "A=0,B=1,C=0",
            "A=0,B=1,C=1",
            "A=1,B=0,C=0",
            "A=1,B=0,C=1",
            "A=1,B=1,C=0",
            "A=1,B=1,C=1",
        ];

        Assert.Equal(["A", "B", "C"], matrix.Variables);
        Assert.Equal(8, matrix.Rows.Count);
        Assert.Equal(expectedAssignments, matrix.Rows.Select(FormatAssignments).ToArray());
    }

    [Fact]
    public void CommandlineTableGeneration_PrintsExpectedTruthTable()
    {
        string projectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/LogischeAusdrücke/LogischeAusdrücke.csproj"));
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo("dotnet", $"run --project \"{projectPath}\" -- tabelle")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        process.Start();
        process.StandardInput.WriteLine("A AND (B OR NOT C)");
        process.StandardInput.Close();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(20_000), "Commandline integration test timed out.");

        Assert.Equal(0, process.ExitCode);
        Assert.Contains("Truth table:", output);
        Assert.Contains("A | B | C | Result", output);
        Assert.Contains("1 | 0 | 0 | 1", output);
        Assert.True(string.IsNullOrWhiteSpace(error), error);
    }

    private static bool Evaluate(IExpressionNode expression, bool a, bool b = false)
    {
        return TruthExpressionEvaluator.Evaluate(expression, new Dictionary<string, bool>
        {
            ["A"] = a,
            ["B"] = b,
        });
    }

    private static string FormatAssignments(TruthTableRow row)
    {
        return string.Join(",", row.Assignments.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}={(pair.Value ? 1 : 0)}"));
    }
}
