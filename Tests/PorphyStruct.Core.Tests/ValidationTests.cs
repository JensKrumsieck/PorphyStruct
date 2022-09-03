using FluentAssertions;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Analysis.Properties;
using Xunit;

namespace PorphyStruct.Core.Tests;

public class ValidationTests
{
    [Theory]
    [InlineData("files/CuHETMP.cif", 0.293,-.182,1.692,-.137,.028,-.024)]
    [InlineData("files/NidPTETMP.cif", 0, -1.192, 0,.154,.154,0)]
    [InlineData("files/Zn(py)TNPCP.cif", .669,.126,.041,.086,-.057,.008)]
    public async Task Test_ShelnuttData(string path, double doming, double saddling, double ruffling, double wavingX, double wavingY, double propellering )
    {
        var cycle = new Macrocycle(path) {MacrocycleType = MacrocycleType.Porphyrin};
        await cycle.Detect();
        var part = cycle.DetectedParts[0];
        part.Properties ??= await MacrocycleProperties.CreateAsync(part);
        var properties = part.Properties;
        properties.Should().NotBeNull();

        var result = properties.Simulation.SimulationResult.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).Values
            .Select(v => Math.Round(Math.Abs(v), 3)).ToList();
        var threshold = 0.4;
        result[0].Should().BeApproximately(Math.Abs(doming), Math.Abs(doming) * threshold + .002);
        result[1].Should().BeApproximately(Math.Abs(saddling), Math.Abs(doming) * threshold+ .002);
        result[2].Should().BeApproximately(Math.Abs(ruffling), Math.Abs(doming) * threshold+ .002);
        (result[3] + result[4]).Should().BeApproximately(Math.Abs(wavingX) + Math.Abs(wavingY), (Math.Abs(wavingX) + Math.Abs(wavingY)) * threshold+ .002);
        result[5].Should().BeApproximately(Math.Abs(propellering), Math.Abs(doming) * threshold+ .002);
    }
}