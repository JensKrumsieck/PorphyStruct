using ChemSharp.Molecules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using PorphyStruct.Web.Shared;
using Tewr.Blazor.FileReader;

namespace PorphyStruct.Web.Pages;

public partial class Index
{
    public MacrocycleViewModel? DataContext;
    public TabControl Tabs;
    private ElementReference inputFile;
    private bool uploadBusy;

    [Inject]
    IJSRuntime JSRuntime { get; set; }

    [Inject]
    IFileReaderService fileReaderService { get; set; }

    private decimal UploadValue;

    protected override void OnInitialized()
    {
        Settings.Instance.Font = "Montserrat";
        base.OnInitialized();
    }
    async Task OnFileChange()
    {
        UploadValue = 0;
        uploadBusy = true;
        Molecule molecule;
        DataContext = null;

        foreach (var file in await fileReaderService.CreateReference(inputFile).EnumerateFilesAsync())
        {
            var fileInfo = await file.ReadFileInfoAsync();
            fileInfo.PositionInfo.PositionChanged += (s, e) =>
            {
                if (e.PercentageDeltaSinceAcknowledge < 2) return;
                UploadValue = e.Percentage;
                InvokeAsync(StateHasChanged);
                e.Acknowledge();
            };
            using MemoryStream memoryStream = await file.CreateMemoryStreamAsync(4096);
            molecule = await MoleculeFactory.CreateFromStreamAsync(memoryStream, Path.GetExtension(fileInfo.Name)[1..]);
            var cycle = new Macrocycle(molecule.AtomDataProvider) { Title = fileInfo.Name };
            DataContext = new(cycle);
            break; //stop on first
        }
        uploadBusy = false;
        await fileReaderService.CreateReference(inputFile).ClearValue();
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
