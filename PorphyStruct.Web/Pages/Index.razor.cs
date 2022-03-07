using ChemSharp.Molecules.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;

namespace PorphyStruct.Web.Pages;

public partial class Index
{
    public MacrocycleViewModel? DataContext;

    protected override void OnInitialized()
    {
        Settings.Instance.Font = "Montserrat";
        base.OnInitialized();
    }
    async Task OnFileChange(InputFileChangeEventArgs args)
    {
        var file = args.File;
        var molecule = await BlazorMoleculeFactory.CreateAsync(file, 81920000L);
        var cycle = new Macrocycle(molecule.AtomDataProvider) { Title = file.Name };
        DataContext = new(cycle);
    }

    async Task OnAnalyzeClick() => await DataContext?.Analyze()!;
}
