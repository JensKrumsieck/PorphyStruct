using CommandDotNet;
using PorphyStruct.Core.Analysis;

namespace PorphyStruct.CLI;

public class Program
{
    public static int Main(string[] args) => new AppRunner<Program>().Run(args);

    // ReSharper disable once UnusedMember.Global
    [DefaultCommand]
    public static void Analyze(
        [Operand(Description = "File to Analyze")] string file,
        [Named('t', "type", Description = "The Macrocycle's type")]
        string rawType
        )
    {
        var type = Enum.Parse(typeof(MacrocycleType), rawType);
        Console.WriteLine(file);
        Console.WriteLine(type.ToString());
    }
}