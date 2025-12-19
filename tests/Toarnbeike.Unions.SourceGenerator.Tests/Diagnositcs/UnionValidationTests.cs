using Microsoft.CodeAnalysis.Testing;
using Toarnbeike.Unions.Utilities;

namespace Toarnbeike.Unions.Diagnositcs;

public class UnionValidationTests
{
    [Test]
    public async Task UnionWithOneCase_Produces_T001Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        public partial class Status { }

        public record A;
        """;

        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU001")
            .WithSpan("/0/Test1.cs", 4, 22, 4, 28)
            .WithMessage("Union 'Status' must declare at least two distinct cases"));
    }

    [Test]
    public async Task UnionWithDuplicateCaseType_Produces_T002Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B))]
        [UnionCase(typeof(A))]
        public partial class Status { }

        public record A;
        public record B;
        """;

        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU002")
            .WithSpan("/0/Test1.cs", 6, 22, 6, 28)
            .WithMessage("Union case 'A' is specified more than once"));
    }

    [Test]
    public async Task UnionWithInvalidCaseType_Produces_T003Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(object))]
        public partial class Status { }

        public record A;
        """;

        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU003")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Type 'Object' is not a valid union case"));
    }

    [Test]
    public async Task NonPartialUnion_Produces_TU004Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B))]
        public class Status { }

        public record A;
        public record B;
        """;

        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU004")
            .WithSpan("/0/Test1.cs", 5, 14, 5, 20)
            .WithMessage("Union declaration 'Status' must be declared partial"));
    }

    [Test]
    public async Task UnionCaseWithNonRecordType_Produces_T005Warning()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B))]
        public partial class Status { }

        public record A;
        public class B;
        """;

        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerWarning("TU005")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Union case 'B' should be a record"));
    }

    [Test]
    public async Task InterfaceUnionCase_Produces_T006Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B))]
        public partial class Status { }

        public record A;
        public interface B;
        """;
        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU006")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Union case 'B' must be a concrete type, not an interface or abstract type"));
    }

    [Test]
    public async Task AbstractUnionCase_Produces_T006Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B))]
        public partial class Status { }

        public record A;
        public abstract record B;
        """;
        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU006")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Union case 'B' must be a concrete type, not an interface or abstract type"));
    }

    [Test]
    public async Task GenericUnionCase_Produces_T007Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A<int>))]
        [UnionCase(typeof(B))]
        public partial class Status { }
        
        public record A<T>;
        public record B;
        """;
        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU007")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Union case 'A' must be a non generic type"));
    }

    [Test]
    public async Task NestedUnionCase_Produces_T008Error()
    {
        var source = """
        using Toarnbeike.Unions;

        [UnionCase(typeof(A))]
        [UnionCase(typeof(B.C))]
        public partial class Status { }
        
        public record A;
        public record B { public record C; }
        """;
        await AnalyzerTest.VerifyAsync(
            source,
            DiagnosticResult.CompilerError("TU008")
            .WithSpan("/0/Test1.cs", 5, 22, 5, 28)
            .WithMessage("Union case 'C' must be an unnested type"));
    }
}