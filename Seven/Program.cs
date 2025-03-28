using Seven.Core.Random;
using Seven.Core.Rules;
using Seven.GA;
using System.Text.Json;

Result? LoadResult(string filepath)
{
    if (!File.Exists(filepath)) return null;
    string content = File.ReadAllText(filepath);
    return JsonSerializer.Deserialize<Result>(content);
}

void SaveResult(Result result)
{
    string filename = $"seven{DateTime.Now:yyyyMMddHHmmss}.json";
    JsonSerializerOptions options = new() { WriteIndented = true, IndentSize = 2 };
    string content = JsonSerializer.Serialize(result, options);
    File.WriteAllText(filename, content);
}

Console.Write("Input previous result filepath: ");
string filepath = Console.ReadLine()!.Trim('"');
Result? prevResult = LoadResult(filepath);

Rule rule = Rule.Standard;
GeneticAlgorithm ga = new(rule, Seiran.Instance);
CancellationTokenSource cancellationTokenSource = new();

var task = Task.Run(() => ga.Run(prevResult, cancellationTokenSource.Token));

Console.WriteLine("Press 'q' key to exit.");

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(intercept: true).Key;
        if (key == ConsoleKey.Q)
        {
            Console.WriteLine("Stopping process...");
            cancellationTokenSource.Cancel();
            break;
        }

        await Task.Delay(100);
    }
}

try
{
    var result = await task;
    SaveResult(result);
}
catch
{
    throw;
}
