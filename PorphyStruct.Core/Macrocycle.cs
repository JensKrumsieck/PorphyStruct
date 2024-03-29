﻿using ChemSharp.Molecules;
using ChemSharp.Molecules.Export;
using ChemSharp.Molecules.Extensions;
using Nodo.Search;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core;

public sealed class Macrocycle : Molecule
{
    private void Init(Molecule mol)
    {
        Atoms.AddRange(mol.Atoms);
        Bonds.AddRange(mol.Bonds);
        Title = mol.Title;
    }

    public Macrocycle(Stream stream, string extension) => Init(FromStream(stream, extension));
    public Macrocycle(string file) =>  Init(FromFile(file));

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
        const int minimumRingSize = 22;
        DetectedParts.Clear();
        //create a subset without dead ends and metals
        //clever pruning of the source graph
        var parts = this.Where(
                a => a.CanBeRingMember()
                     && !Constants.DeadEnds.Contains(a.Symbol)
                     && !ChemSharp.Constants.AminoAcids.ContainsKey(a.Residue)
            ).ConnectedFigures()
            .Where(a => a.Count >= minimumRingSize)
            .ToMolecules().ToList();
        var references = SetUpReferences().ToList();
        CacheNeighbors = false;
        foreach (var part in parts)
        {
            foreach (var reference in references)
            {
                if(part.Atoms.Count < reference.molecule.Atoms.Count) continue;
                foreach (var data in part.GetSubgraphs(reference.molecule))
                {
                    var mapping = new Dictionary<string, Atom>();
                    for (var i = 0; i < data.Atoms.Count; i++)
                        mapping.Add(reference.molecule.Atoms[i].Title, data.Atoms[i]);
                    var analysis = MacrocycleAnalysis.Create(data, mapping, reference.type);
                    var metal = Neighbors(analysis.N4Cavity[0]).FirstOrDefault(s => s.CanBeCenterAtom());
                    if (metal != null) analysis.Metal = metal;
                    DetectedParts.Add(analysis);
                }
            }
        }
        CacheNeighbors = true;
    }

    private IEnumerable<(MacrocycleType type, Molecule molecule)> SetUpReferences()
    {
        var types = Enum.GetValues<MacrocycleType>();
        //load matching macrocycle type
        foreach (var type in types)
        {
            var resourceName = $"PorphyStruct.Core.Reference.{type}.Doming.mol2";
            var reference = FromStream(
                                       ResourceUtil.LoadResource(resourceName)!,
                                       resourceName.Split('.').Last())
                .Where(a => a.CanBeRingMember()
                            && !Constants.DeadEnds.Contains(a.Symbol));
            yield return (type,reference);
        }
    }
}