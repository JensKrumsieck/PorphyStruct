using System.Collections.ObjectModel;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.ViewModel;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PorphyStruct.CLI;

public static class AnalysisExtension
{
    internal static void RenderAnalysis(this IAnsiConsole console, IList<AnalysisViewModel> items)
    {
        console.Margin();
        console.MarkupLine($"{items.Count} [b]Macrocycles[/] of given type could be found!");
        console.Margin();
        foreach (var item in items)
            RenderAnalysisItem(console, item.Analysis);
    }

    private static void RenderAnalysisItem(this IAnsiConsole console, MacrocycleAnalysis analysis)
    {
        var props = analysis.Properties!;
        console.MarkupLine(
            $"[invert][{analysis.AnalysisColor}]Analysis-Id: \t[bold]{analysis.AnalysisColor}[/][/][/]");
        console.Margin();
        
        console.RenderDoop(props);
        console.DrawTable(props.Simulation.SimulationResult);
        console.Margin();
        console.DrawChart(props.Simulation.SimulationResultPercentage);
        console.Margin(2);
    }
    private static void RenderDoop(this IAnsiConsole console, MacrocycleProperties properties)
    {
        var Dexp = new Panel(
            $"[b]{properties.OutOfPlaneParameter.Key}:[/] {properties.OutOfPlaneParameter.Value:N4}{properties.OutOfPlaneParameter.Unit}");
        var Dsim = new Panel(
            $"[b]{properties.Simulation.OutOfPlaneParameter.Key}:[/] {properties.Simulation.OutOfPlaneParameter.Value:N4}{properties.Simulation.OutOfPlaneParameter.Unit}");
        var DErr = new Panel($"Delta: {properties.Delta:N4} - Error {properties.Error:P1}");
        console.Write(new Columns(Dexp, Dsim, DErr));
    }
}