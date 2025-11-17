using System.Text.Json;
namespace BOTC;

public class Script {
    public string Name { get; set; } = "";
    public string Author { get; set; } = "";

    public List<string> Townsfolk { get; set; } = [];
    public List<string> Outsiders { get; set; } = [];
    public List<string> Minions { get; set; } = [];
    public List<string> Demons { get; set; } = [];
}

public class ScriptManager {
    public static List<Script> ParseScripts(string scriptsPath) {
        List<Script> scripts = [];

        foreach (string file in Directory.GetFiles(scriptsPath, "*.json")) {
            string json = File.ReadAllText(file);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Script current = new();

            var meta = root[0];
            current.Name = meta.GetProperty("name").GetString() ?? "";
            current.Author = meta.GetProperty("author").GetString() ?? "";

            bool failed = false;
            for (int i = 1; i < root.GetArrayLength(); i++) {
                string? character = root[i].GetString();
                if (character == null) {
                    Console.WriteLine($"Error: Failed to parse script: {file}");
                    failed = true;
                    break;
                }
                if (!BotcRoles.Lookup.TryGetValue(character, out var type)) {
                    Console.WriteLine($"Error: No such character \'{character}\': {file}");
                    continue;
                }
                switch (type) {
                    case BotcRoles.CharType.TOWNSFOLK:
                        current.Townsfolk.Add(character);
                        break;
                    case BotcRoles.CharType.OUTSIDER:
                        current.Outsiders.Add(character);
                        break;
                    case BotcRoles.CharType.MINION:
                        current.Minions.Add(character);
                        break;
                    case BotcRoles.CharType.DEMON:
                        current.Demons.Add(character);
                        break;
                }
            }
            if (failed)
                continue;
            scripts.Add(current);
        }
        return scripts;
    }

    private static void PrintScriptInfo(Script script) {
        Console.WriteLine($"--- {script.Name} by {script.Author} ---");
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

    public static Script SetScript(List<Script> scripts) {
        Script selected;
        while (true) {
            Console.WriteLine("\nWhat script to play with?\n"
                + "(type \"scripts\" for a list of available scripts)");
            string? inputScript = Console.ReadLine();
            if (inputScript == "scripts") {
                foreach (var s in scripts)
                    Console.WriteLine(s.Name);
                Console.WriteLine("For more info type \"info <script name>\"");
                string? command = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(command))
                    continue;
                string[] parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && parts[0].Equals("info", StringComparison.OrdinalIgnoreCase)) {
                    string scriptName = parts[1];
                    Script? infoScript = scripts.Find(s => 
                        s.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase)
                    );
                    if (infoScript != null)
                        PrintScriptInfo(infoScript);
                    else
                        Console.WriteLine($"Error: No script named \"{scriptName}\".");
                }
                continue;
            } else if (inputScript != null && scripts.Exists(x => x.Name == inputScript)) {
                selected = scripts.Find(s => s.Name == inputScript)!;
                break;
            }
            Console.WriteLine("Error: Script not available.");
        }
        Console.WriteLine($"Selected to play: {selected.Name} by {selected.Author}.");
        return selected;
    }
}
