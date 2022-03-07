using ChemSharp.Molecules.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using PorphyStruct.Web.Shared;

namespace PorphyStruct.Web.Pages;

public partial class Index
{
    public MacrocycleViewModel? DataContext;
    public TabControl Tabs;

    [Inject]
    IJSRuntime JSRuntime { get; set; }

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

    async Task DownloadImage()
    {
        if (DataContext == null) return;
        //custom fonts do not work in exported images from wasm
        //see upcoming fix: https://github.com/oxyplot/oxyplot/pull/1857
        using var stream = new MemoryStream();
        var itemIndex = Tabs.Pages.IndexOf(Tabs.ActivePage);
        var item = DataContext.Items[itemIndex];
        item.ExportPlot(stream, "png");
        var file = stream.ToArray();
        await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", DataContext.Title + "_graph.png", "application/octet-stream", file);
    }
}
