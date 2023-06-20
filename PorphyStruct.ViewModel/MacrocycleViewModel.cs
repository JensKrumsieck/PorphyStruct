using System.ComponentModel;
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
            analysis.PropertyChanged += Child_Changed;
            part.Properties ??= new MacrocycleProperties(part);
            Items.Add(analysis);
            SelectedIndex = Items.IndexOf(analysis);
        }
    }

    private void Child_Changed(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectedItem.Analysis)) return;
        var tmp = new List<AnalysisViewModel>(Items);
        var tmpIndex = SelectedIndex;
        Items.Clear();
        for(var i = 0; i < tmp.Count; i++) Items.Add(tmp[i]);
        SelectedIndex = -1;
        SelectedIndex = tmpIndex;
    }
    
    protected virtual void Validate() { }
}
