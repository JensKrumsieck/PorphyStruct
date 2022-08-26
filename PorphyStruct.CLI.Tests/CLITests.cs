﻿using CommandDotNet.TestTools;
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
                "analyze testfiles/oriluy.cif -t Porphyrin");
        result.ExitCode.Should().Be(0);
    }
}