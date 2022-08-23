using CommandDotNet;
using CommandDotNet.Spectre;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using Spectre.Console;

namespace PorphyStruct.CLI;

public class Program
{
    public static AppRunner AppRunner =>
        new AppRunner<Program>()
            .UseSpectreAnsiConsole()
            .UseSpectreArgumentPrompter();

    private static int Main(string[] args) => AppRunner.Run(args);

    [Command("analyze")]
    public async Task Analyze(
        IAnsiConsole console,
        [Positional(Description = "Input file to analyze. Can be of type .cif, .mol, .mol2, .pdb or .xyz")]
        string file,
        [Option('t', "type",
            Description = "Choose Macrocyclic Type: Porphyrin, Corrole, Norcorrole, Corrphycene or Porphycene")]
        string rawType,
        [Named('x', "extended", Description = "Use Extended Basis")]
        bool extendedBasis,
        [Named("no-export", Description = "Do not write any files!")]
        bool doNotExport
    )
    {
        Settings.Instance.UseExtendedBasis = extendedBasis;
        var type = (MacrocycleType) Enum.Parse(typeof(MacrocycleType), rawType);
        var macrocycle = new Macrocycle(file)
        {
            MacrocycleType = type
        };
        var viewModel = new MacrocycleViewModel(macrocycle);

        console.Write(new Rule(file));
        console.Write(new Rule(
            $"Using [invert]{type}[/] with [invert]{(extendedBasis ? "extended Basis" : "minimal Basis")}[/]"));

        await console.Status().StartAsync("Analyzing", async ctx => { await viewModel.Analyze(); });
        console.RenderAnalysis(viewModel.Items);
        if (!doNotExport) Export(file, viewModel);
    }

    private static void Export(string file, MacrocycleViewModel viewModel)
    {
        foreach (var analysisViewModel in viewModel.Items)
        {
            var path = Path.ChangeExtension(file, null);
            if (viewModel.Items.Count > 1) path += $"_{analysisViewModel.Analysis.AnalysisColor}";
            analysisViewModel.ExportSimulation(path, "md");
            analysisViewModel.ExportSimulation(path, "json");
        }
    }
}