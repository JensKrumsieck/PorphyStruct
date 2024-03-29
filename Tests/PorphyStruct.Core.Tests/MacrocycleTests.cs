﻿using FluentAssertions;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Analysis.Properties;
using Xunit;

namespace PorphyStruct.Core.Tests;

public class MacrocycleTests
{
    private static readonly Dictionary<MacrocycleType, int> RingSize = new()
    {
        { MacrocycleType.Porphyrin, 24 },
        { MacrocycleType.Corrole, 23 },
        { MacrocycleType.Norcorrole, 22 },
        { MacrocycleType.Porphycene, 24 },
        { MacrocycleType.Corrphycene, 24 }
    };
    
    public static IEnumerable<object[]> GetMacrocycles()
    {
        yield return new object[] {"files/830942.cif", MacrocycleType.Corrole};
        yield return new object[] {"files/NidPTETMP.cif", MacrocycleType.Porphyrin};
        yield return new object[] {"files/Zn(py)TNPCP.cif", MacrocycleType.Porphyrin};
        yield return new object[] {"files/oriluy.cif", MacrocycleType.Porphyrin}; 
        yield return new object[] {"files/CuHETMP.cif", MacrocycleType.Porphyrin};
        yield return new object[] {"files/SADZAA.mol2", MacrocycleType.Corrphycene};
        yield return new object[] {"files/YUKJOD.mol2", MacrocycleType.Porphycene};
        yield return new object[] {"files/ATUVIV.mol2", MacrocycleType.Porphycene};
        yield return new object[] {"files/1544170.cif", MacrocycleType.Corrole};
        yield return new object[] {"files/XASBEC.mol2", MacrocycleType.Corrole};
        yield return new object[] {"files/EMOWOS.mol2", MacrocycleType.Norcorrole};
    }

    [Theory]
    [MemberData(nameof(GetMacrocycles))]
    public void Macrocycle_CanBeCreated(string path, MacrocycleType type)
    {
        var cycle = new Macrocycle(path);
        cycle.Should().NotBeNull();
        cycle.Atoms.Should().HaveCountGreaterThan(0);
        cycle.Bonds.Should().HaveCountGreaterThan(0);
    }

    [Theory]
    [MemberData(nameof(GetMacrocycles))]
    public void Macrocycle_CanDetect(string path, MacrocycleType type)
    {
        var cycle = new Macrocycle(path);
        cycle.Detect();
        cycle.DetectedParts.Should().HaveCountGreaterThan(0);
        foreach (var p in cycle.DetectedParts)
        {
            p.Properties ??= new MacrocycleProperties(p);
            p.Atoms.Should().HaveCount(RingSize[type]);
            p.Properties.Should().NotBeNull();
            p.Properties.Analysis.Type.Should().Be(type);
        }
    }

    [Fact]
    public void All_Corroles_WillBeDetected()
    {
        var cycle = new Macrocycle("files/830942.cif");
        cycle.Detect();
        cycle.DetectedParts.Should().HaveCount(4);
    }
}