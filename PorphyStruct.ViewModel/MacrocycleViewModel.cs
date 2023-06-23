using System.ComponentModel;
using ChemSharp.Molecules;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis.Properties;
using TinyMVVM;

namespace PorphyStruct.ViewModel;

public class MacrocycleViewModel : ListingViewModel<AnalysisViewModel>
{
    /// <summary>
    /// The Path to open from
    /// </summary>
    public string Filename { get; }

    private Atom? _selectedAtom;
    /// <summary>
    /// Gets or Sets the selected Atom
    /// </summary>
    public Atom? SelectedAtom
    {
        get => _selectedAtom;
        set => Set(ref _selectedAtom, value, () => HandleAtomSelect(_selectedAtom));
    }
    
    /// <summary>
    /// The opened Macrocycle
    /// </summary>
    public Macrocycle Macrocycle { get; }

    public MacrocycleViewModel(string path)
    {
        Filename = path;
        Macrocycle = new Macrocycle(Filename);
    }

    public MacrocycleViewModel(Macrocycle cycle)
    {
        Macrocycle = cycle;
        Filename = cycle.Title;
    }
    public override string Title => Path.GetFileNameWithoutExtension(Filename);

    /// <summary>
    /// Runs Detection Algorithm
    /// </summary>
    /// <returns></returns>
    public async Task Analyze()
    {
        Items.Clear();
        await Task.Run(Macrocycle.Detect);
        Validate();
        foreach (var part in Macrocycle.DetectedParts)
        {
            var analysis = new AnalysisViewModel(this, part);
            part.Properties ??= new MacrocycleProperties(part);
            Items.Add(analysis);
            SelectedIndex = Items.IndexOf(analysis);
            analysis.PropertyChanged += Child_Changed;
        }

        SelectedIndexChanged += (sender, args) => SelectedAtom = null;
    }

    private void Child_Changed(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectedItem.Analysis)) return;
        var tmp = SelectedIndex;
        SelectedIndex = -1;
        OnSelectedIndexChanged();
        SelectedIndex = tmp;
        OnSelectedIndexChanged();
    }
    protected virtual void Validate() { }

    public virtual void HandleAtomSelect(Atom? selectedAtom)
    {
        foreach (var viewModel in Items)
        {
            if (!viewModel.Analysis.Atoms.Contains(selectedAtom))
            {
                viewModel.ExperimentalSeries.ClearSelection();
                viewModel.Model.InvalidatePlot(true);
                continue;
            }
            SelectedIndex = Items.IndexOf(viewModel);
            var index = viewModel.Analysis.GetMappingIndex(selectedAtom);
            viewModel.ExperimentalSeries.SelectItem(index);
            viewModel.Model.InvalidatePlot(true);
            break;
        }
    }
}
