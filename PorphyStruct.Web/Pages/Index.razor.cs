using ChemSharp.Molecules.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;

namespace PorphyStruct.Web.Pages;

public partial class Index
{
    public MacrocycleViewModel? DataContext;
    bool busy;

    protected override void OnInitialized()
    {
        Settings.Instance.Font = "Montserrat";
        base.OnInitialized();
    }
    async Task OnFileChange(InputFileChangeEventArgs args)
    {
        var file = args.File;
        var molecule = await BlazorMoleculeFactory.CreateAsync(file);
        var cycle = new Macrocycle(molecule.AtomDataProvider);
        DataContext = new(cycle);
    }

    async Task OnAnalyzeClick()
    {
        busy = true;
        await DataContext?.Analyze()!;
        busy = false;
    }
}
