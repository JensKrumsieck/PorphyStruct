using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;

namespace PorphyStruct.CLI.Tests;

public class CLITests
{
    [Fact]
    public void Analyze_ShouldNotThrow()
    {
        var result = Program.AppRunner
            .RunInMem(
                "analyze testfiles/oriluy.cif");
        result.ExitCode.Should().Be(0);
    }
}