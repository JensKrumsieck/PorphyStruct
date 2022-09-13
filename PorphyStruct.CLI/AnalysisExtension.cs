using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.ViewModel;
using Spectre.Console;

namespace PorphyStruct.CLI;

public static class AnalysisExtension
{
    internal static void RenderAnalysis(this IAnsiConsole console, IList<AnalysisViewModel> items)
    {
        console.Margin();
        var groups = items.GroupBy(a => a.Analysis.Type);
        foreach (var g in groups)
        {
            console.MarkupLine($"{g.Count()} [b]Macrocycles[/] of type [b]{g.Key}[/] could be found!");
        }
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
        console.DrawTable(props.Simulation!.SimulationResult);
        console.Margin();
        console.DrawChart(props.Simulation.SimulationResultPercentage);
        console.Margin(2);
    }

    private static void RenderDoop(this IAnsiConsole console, MacrocycleProperties properties)
    {
        var dexp = new Panel(
            $"[b]{properties.OutOfPlaneParameter.Key}:[/] {properties.OutOfPlaneParameter.Value:N4}{properties.OutOfPlaneParameter.Unit}");
        var dsim = new Panel(
            $"[b]{properties.Simulation!.OutOfPlaneParameter.Key}:[/] {properties.Simulation.OutOfPlaneParameter.Value:N4}{properties.Simulation.OutOfPlaneParameter.Unit}");
        var derr = new Panel($"Delta: {properties.Delta:N4} - Error {properties.Error:P1}");
        var type = new Panel(properties.AnalysisType);
        console.Write(new Columns(type, dexp, dsim, derr));
    }
}