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
    private ElementReference _inputFile;
    private bool _uploadBusy;

    [Inject] private IJSRuntime JsRuntime { get; set; }

    [Inject] private IFileReaderService FileReaderService { get; set; }

    private decimal _uploadValue;

    protected override void OnInitialized()
    {
        Settings.Instance.Font = "Montserrat";
        base.OnInitialized();
    }

    private async Task OnFileChange()
    {
        _uploadValue = 0;
        _uploadBusy = true;
        DataContext = null;

        foreach (var file in await FileReaderService.CreateReference(_inputFile).EnumerateFilesAsync())
        {
            var fileInfo = await file.ReadFileInfoAsync();
            fileInfo.PositionInfo.PositionChanged += (s, e) =>
            {
                if (e.PercentageDeltaSinceAcknowledge < 2) return;
                _uploadValue = e.Percentage;
                InvokeAsync(StateHasChanged);
                e.Acknowledge();
            };
            using var memoryStream = await file.CreateMemoryStreamAsync(4096);
            var cycle = new Macrocycle(memoryStream, Path.GetExtension(fileInfo.Name)[1..]) { Title = fileInfo.Name };
            DataContext = new MacrocycleViewModel(cycle);
            break; //stop on first
        }
        _uploadBusy = false;
        await FileReaderService.CreateReference(_inputFile).ClearValue();
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
        await JsRuntime.InvokeVoidAsync("BlazorDownloadFile", DataContext.Title + "_graph.png", "application/octet-stream", file);
    }
}
