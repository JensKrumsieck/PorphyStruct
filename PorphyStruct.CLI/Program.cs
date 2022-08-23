using CommandDotNet;
using CommandDotNet.Diagnostics;
using ConsoleTables;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;

namespace PorphyStruct.CLI;

public class Program
{
    static int Main(string[] args)
    {
        Debugger.AttachIfDebugDirective(args);
        return new AppRunner<Program>().Run(args);
    }

    [Command("analyze")]
    public void Analyze(
        IConsole console,
        [Positional(Description = "Input file to analyze")]
        string file,
        [Option('t', "type", Description = "Macrocyclic Type")]
        string rawType)
    {
        var type = (MacrocycleType)Enum.Parse(typeof(MacrocycleType), rawType);
        var macrocycle = new Macrocycle(file)
        {
            MacrocycleType = type
        };
        var viewModel = new MacrocycleViewModel(macrocycle);
        Task.Run(async () =>
        {
            await viewModel.Analyze();
            console.WriteLine($"{viewModel.Items.Count} {type}s found!");
            foreach (var item in viewModel.Items)
            {
                var result = item.Analysis.Properties?.Simulation.SimulationResult;
                var headers = result?.Select(s => s.Key).ToArray();
                var values = result?.Select(s => s.Value).ToArray();
                var table = new ConsoleTable(headers);
                table.AddRow(values);
                table.Write();
            }
        });
    }
}