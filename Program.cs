using System.Text.Json;
namespace BotC;

public enum CharType
{
    TOWNSFOLK,
    OUTSIDER,
    MINION,
    DEMON
}

public class Characters {
    public readonly Dictionary<string, CharType> characterDict = new() {
        ["washerwoman"] = CharType.TOWNSFOLK,
        ["librarian"] = CharType.TOWNSFOLK,
        ["investigator"] = CharType.TOWNSFOLK,
        ["chef"] = CharType.TOWNSFOLK,
        ["empath"] = CharType.TOWNSFOLK,
        ["fortuneteller"] = CharType.TOWNSFOLK,
        ["undertaker"] = CharType.TOWNSFOLK,
        ["monk"] = CharType.TOWNSFOLK,
        ["ravenkeeper"] = CharType.TOWNSFOLK,
        ["virgin"] = CharType.TOWNSFOLK,
        ["slayer"] = CharType.TOWNSFOLK,
        ["soldier"] = CharType.TOWNSFOLK,
        ["mayor"] = CharType.TOWNSFOLK,
        ["butler"] = CharType.OUTSIDER,
        ["drunk"] = CharType.OUTSIDER,
        ["recluse"] = CharType.OUTSIDER,
        ["saint"] = CharType.OUTSIDER,
        ["poisoner"] = CharType.MINION,
        ["spy"] = CharType.MINION,
        ["scarletwoman"] = CharType.MINION,
        ["baron"] = CharType.MINION,
        ["imp"]  = CharType.DEMON
    };
}

public class Script {
    public string Name { get; set; } = "";
    public string Author { get; set; } = "";

    public List<string> Townsfolk { get; set; } = [];
    public List<string> Outsiders { get; set; } = [];
    public List<string> Minions { get; set; } = [];
    public List<string> Demons { get; set; } = [];
}

public class Program {
    static readonly Characters chars = new();

    static List<Script> ParseScripts(string scriptsPath) {
        List<Script> scripts = [];
        var charsDict = chars.characterDict;

        foreach (string file in Directory.GetFiles(scriptsPath, "*.json")) {
            string json = File.ReadAllText(file);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Script current = new();

            var meta = root[0];
            current.Name = meta.GetProperty("name").GetString() ?? "";
            current.Author = meta.GetProperty("author").GetString() ?? "";

            for (int i = 1; i < root.GetArrayLength(); i++) {
                string? character = root[i].GetString();
                if (character == null) {
                    Console.WriteLine($"Error: Failed to parse script: {file}");
                    goto skip;
                }
                if (!charsDict.TryGetValue(character, out var type)) {
                    Console.WriteLine($"Error: No such character \'{character}\': {file}");
                    continue;
                }
                switch (type) {
                    case CharType.TOWNSFOLK:
                        current.Townsfolk.Add(character);
                        break;
                    case CharType.OUTSIDER:
                        current.Outsiders.Add(character);
                        break;
                    case CharType.MINION:
                        current.Minions.Add(character);
                        break;
                    case CharType.DEMON:
                        current.Demons.Add(character);
                        break;
                }
            }
            scripts.Add(current);
            skip:;
        }
        return scripts;
    }

    static void PrintScriptRoles(Script script) {
        Console.WriteLine("Townsfolk:");
        foreach (string character in script.Townsfolk)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Outsiders:");
        foreach (string character in script.Outsiders)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Minions:");
        foreach (string character in script.Minions)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Demons:");
        foreach (string character in script.Demons)
            Console.WriteLine($"\t{character}");
    }

    static Dictionary<string, string> AssignRoles(List<string> players, Script script) {
        var rng = new Random();
        string PickRandom(List<string> list) => list[rng.Next(list.Count)];

        string demon = PickRandom(script.Demons);
        string minion = PickRandom(script.Minions);

        int outsiderCount = minion.Equals("baron") ? 3 : 1;
        List<string> selectedOutsiders = [.. script.Outsiders
            .OrderBy(_ => rng.Next())
            .Take(outsiderCount)
        ];

        int townsfolkCount = players.Count - (1 + 1 + outsiderCount);
        List<string> selectedTownsfolk = [.. script.Townsfolk
            .OrderBy(_ => rng.Next())
            .Take(townsfolkCount)
        ];

        if (selectedOutsiders.Contains("drunk")) {
            List<string> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            string fakeRole = PickRandom(remainingTownsfolk);
            selectedOutsiders.Remove("drunk");
            selectedOutsiders.Add($"drunk ({fakeRole})");
        }

        List<string> roles = [demon, minion];
        roles.AddRange(selectedOutsiders);
        roles.AddRange(selectedTownsfolk);

        List<string> shuffledPlayers = [.. players.OrderBy(_ => rng.Next())];
        List<string> shuffledRoles = [.. roles.OrderBy(_ => rng.Next())];
        Dictionary<string, string> assignments = [];
        for (int i = 0; i < players.Count; i++)
            assignments[shuffledPlayers[i]] = shuffledRoles[i];
        return assignments;
    }

    public static void Main(string[] args) {
        string scriptsDir = "./Scripts";
        Console.WriteLine("=== Blood on the Clocktower ===");
        Console.WriteLine("Easy game setup system by kixikCodes.");
        List<Script> scripts = ParseScripts(scriptsDir);

        int playerCount;
        while (true) {
            Console.WriteLine("\nHow many players? (7 - 12)");
            try {
                playerCount = Convert.ToInt32(Console.ReadLine());
                if (playerCount >= 7 && playerCount <= 12)
                    break;
                Console.WriteLine("Error: Invalid player count.");
            } catch (Exception) {
                Console.WriteLine("Error: Invalid input.");
            }
        }
        Console.WriteLine($"Players set to {playerCount}.");

        List<string> playerNames = [];
        for (int i = 0; i < playerCount; i++) {
            string? player;
            while (true) {
                Console.Write($"Set player {i + 1} name: ");
                player = Console.ReadLine();
                if (player != "" && player != null)
                    break;
                Console.WriteLine("Error: No name provided.");
            }
            playerNames.Add(player);
        }
        Console.WriteLine("Players registered.");

        Script script;
        while (true) {
            Console.WriteLine("\nWhat script to play with?\n"
                + "(type \"scripts\" for a list of available scripts)");
            string? inputScript = Console.ReadLine();
            if (inputScript == "scripts") {
                foreach (var s in scripts)
                    Console.WriteLine(s.Name);
                Console.WriteLine();
                continue;
            } else if (inputScript != null && scripts.Exists(x => x.Name == inputScript)) {
                script = scripts.Find(s => s.Name == inputScript)!;
                break;
            }
            Console.WriteLine("Error: Script not available.");
        }
        Console.WriteLine($"Selected to play: {script.Name} by {script.Author}.");
        //PrintScriptRoles(script);

        while (true) {
            Console.WriteLine("\nAssigning character roles...");
            var playerRoles = AssignRoles(playerNames, script);

            Console.WriteLine("\n--- Role Assignments ---");
            foreach (var assignment in playerRoles)
                Console.WriteLine($"{assignment.Key}: {assignment.Value}");
            reassign:
            Console.Write("\nWould you like to reassign? (y/n) ");
            string? input = Console.ReadLine();
            if (input == "n")
                break;
            else if (input == "y")
                continue;
            else
                goto reassign;
        }
    }
}
