using EF_Calls_Analyzer;

IEnumerable<string> fileContents = File.ReadLines("C:\\logs.txt");

List<Command> commands = [];

bool readingCommandText = false;
int timeRun = 0;
string commandText = "";

foreach (string line in fileContents)
{
    if (line.Contains("Executed DbCommand"))
    {
        readingCommandText = true;
        string[] timeStrings = line.Split('(');
        string[] timeStrings2 = timeStrings[1].Split("ms");
        string timeString = timeStrings2[0].Replace(",","");
        timeRun = int.Parse(timeString);
    }
    else {
        if (line.Contains("INF]"))
        {
            if (readingCommandText) {
                readingCommandText = false;
                commands.Add(new Command { CommandText = commandText, TimeRunningMs = timeRun });
                timeRun = 0;
                commandText = "";
            }
        }
        else if (readingCommandText) {
            commandText += line;
        }
    }
}

Console.WriteLine("Select option: 1-Top 20 Slowest 2-Command Frequency");
string option = Console.ReadLine()!;
Console.WriteLine("---------------------------------------------");
Console.WriteLine("---------------------------------------------");

switch (option) {
    case "1":
        List<Command> biggest = commands.OrderByDescending(c => c.TimeRunningMs).Take(20).ToList();

        foreach (Command command in biggest)
        {
            Console.WriteLine(command.CommandText);
            Console.WriteLine(command.TimeRunningMs);
            Console.WriteLine("---------------------------------------------");
        }

        break;
    case "2":
        var GroupByText = commands.GroupBy(c => c.CommandText).Select(g => new { key = g.Key, avgRunTime = g.Average(c => c.TimeRunningMs), count = g.Count() }).OrderByDescending(g => g.count);
        foreach (var group in GroupByText)
        {
            Console.WriteLine(group.key);
            Console.WriteLine($"Frequency: {group.count}");
            Console.WriteLine($"AvgRunTimeMs: {group.avgRunTime}");
            Console.WriteLine("---------------------------------------------");
        }
        break;
    default: break;
}

