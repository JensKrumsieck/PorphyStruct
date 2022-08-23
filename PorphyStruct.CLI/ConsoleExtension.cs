using ChemSharp.Molecules.Properties;
using MathNet.Numerics;
using Spectre.Console;

namespace PorphyStruct.CLI;

public static class ConsoleExtension
{
    public static readonly Dictionary<string, Color> Colors = new()
    {
        ["Doming"] = Color.Red1,
        ["Doming1"] = Color.Red1,
        ["Doming2"] = Color.Red3,
        ["Saddling"] = Color.Blue1,
        ["Saddling1"] = Color.Blue1,
        ["Saddling2"] = Color.Blue3,
        ["Ruffling"] = Color.Green1,
        ["Ruffling1"] = Color.Green1,
        ["Ruffling2"] = Color.Green3,
        ["WavingX"] = Color.Orange1,
        ["WavingX1"] = Color.Orange1,
        ["WavingX2"] = Color.Orange3,
        ["WavingY"] = Color.Purple_1,
        ["WavingY1"] = Color.Purple_1,
        ["WavingY2"] = Color.Purple3,
        ["Propellering"] = Color.RosyBrown,
        ["Propellering1"] = Color.RosyBrown,
        ["Propellering2"] = Color.SandyBrown
    };

    public static void Margin(this IAnsiConsole console, int count = 1)
    {
        for (var i = 0; i < count; i++)
            console.WriteLine(); //add margin
    }

    internal static void DrawChart(this IAnsiConsole console, List<KeyValueProperty> items)
    {
        console.Write(new BreakdownChart().Compact().ShowPercentage().AddItems(items, (item) => new BreakdownChartItem(
            item.Key, item.Value.Round(2), Colors[item.Key])));
    }

    internal static void DrawTable(this IAnsiConsole console, List<KeyValueProperty> items)
    {
        var table = new Table();
        table.HideHeaders().Width(40).AddColumn("").AddColumn("");
        table.Columns[1].RightAligned();
        table.Border = TableBorder.Minimal;
        foreach (var mode in items)
        {
            table.AddRow($"[bold]{mode.Key}[/]", $"{mode.Value:N4} {mode.Unit}");
        }

        console.Write(table);
    }
}