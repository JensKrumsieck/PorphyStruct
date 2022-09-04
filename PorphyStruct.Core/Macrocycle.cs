using ChemSharp.Molecules;
using ChemSharp.Molecules.Extensions;
using Nodo.Search;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core;

public sealed class Macrocycle : Molecule
{
    /// <summary>
    /// Correlates type to size of ring fragment
    /// </summary>
    private static readonly Dictionary<MacrocycleType, int> RingSize = new()
    {
        { MacrocycleType.Porphyrin, 24 },
        { MacrocycleType.Corrole, 23 },
        { MacrocycleType.Norcorrole, 22 },
        { MacrocycleType.Porphycene, 24 },
        { MacrocycleType.Corrphycene, 24 }
    };

    private void Init(Molecule mol)
    {
        Atoms.AddRange(mol.Atoms);
        Bonds.AddRange(mol.Bonds);
        Title = mol.Title;
    }
    
    public Macrocycle(Stream stream, string extension)
    {
        var mol = FromStream(stream, extension);
        Init(mol);
    }

    public Macrocycle(string file)
    {
        var mol = FromFile(file);
        Init(mol);
    }
    
    /// <summary>
    /// Gets or Sets the Macrocycle Type
    /// </summary>
    public MacrocycleType MacrocycleType { get; set; } = MacrocycleType.Porphyrin;

    /// <summary>
    /// Contains detected fragment data
    /// </summary>
    public readonly IList<MacrocycleAnalysis> DetectedParts = new List<MacrocycleAnalysis>();

    /// <summary>
    /// Runs a Graph-Theory based Detection algorithm to find Macrocycles in Molecule
    /// </summary>
    /// <returns></returns>
    public void Detect()
    {
        DetectedParts.Clear();
        //create a subset without dead ends and metals
        var parts = this.Where(
                a => a.IsNonCoordinative()
                     && !Constants.DeadEnds.Contains(a.Symbol)
                     && !ChemSharp.Constants.AminoAcids.ContainsKey(a.Residue)
            ).ConnectedFigures()
            .Where(a => a.Count >= RingSize[MacrocycleType])
            .ToMolecules().ToList();
        
        //load matching macrocycle type
        var resourceName = $"PorphyStruct.Core.Reference.{MacrocycleType}.Doming.mol2";
        var reference = FromStream(
                ResourceUtil.LoadResource(resourceName)!,
                resourceName.Split('.').Last())
            .Where(a => a.IsNonCoordinative()
                        && !Constants.DeadEnds.Contains(a.Symbol));
        foreach (var data in parts.SelectMany(p => p.GetSubgraphs(reference)))
        {
            for (var i = 0; i < data.Atoms.Count; i++)
                data.Atoms[i].Title = reference.Atoms[i].Title;
            RebuildCache();
            var analysis = MacrocycleAnalysis.Create(data, MacrocycleType);
            var metal = Neighbors(analysis.N4Cavity[0]).FirstOrDefault(s => !s.IsNonCoordinative());
            if (metal != null) analysis.Metal = metal;
            DetectedParts.Add(analysis);
        }
    }
}
