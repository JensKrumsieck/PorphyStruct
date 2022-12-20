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
            part.Properties ??= new MacrocycleProperties(part);
            Items.Add(analysis);
            SelectedIndex = Items.IndexOf(analysis);
        }
    }

    protected virtual void Validate() { }
}
