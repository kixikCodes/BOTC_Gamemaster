using System.Text.Json;
namespace BOTC;

public class Script {
    public string Name { get; set; } = "";
    public string Author { get; set; } = "";

    public List<Character> Townsfolk { get; set; } = [];
    public List<Character> Outsiders { get; set; } = [];
    public List<Character> Minions { get; set; } = [];
    public List<Character> Demons { get; set; } = [];

    public List<Character> Travellers { get; set; } = [];
    public Character? Fabled;
    public Character? Loric;
    public List<Jinx> Jinxes { get; set; } = [];
}

public class ScriptManager {

    public static List<Jinx> ComputeJinxes(Script script)
    {
        if (script.Fabled != null && !string.Equals(script.Fabled.Id, "djinn"))
            return [];
        var allChars = new List<Character>();
        allChars.AddRange(script.Townsfolk);
        allChars.AddRange(script.Outsiders);
        allChars.AddRange(script.Minions);
        allChars.AddRange(script.Demons);
        allChars.AddRange(script.Travellers);

        var foundJinxes = new List<Jinx>();
        var charSet = new HashSet<Character>(allChars);
        foreach (var jinx in BotcRoles.Jinxes)
            if (charSet.Contains(jinx.Pair.Item1) && charSet.Contains(jinx.Pair.Item2))
                foundJinxes.Add(jinx);
        return foundJinxes;
    }

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
                string? id = root[i].GetString();
                if (id == null) {
                    Console.WriteLine($"Error: Failed to parse script: {file}");
                    failed = true;
                    break;
                }
                if (!BotcRoles.Characters.TryGetValue(id, out var character)) {
                    Console.WriteLine($"Error: No such character \'{id}\': {file}");
                    continue;
                }
                switch (character.Type) {
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
                    case CharType.TRAVELLER:
                        current.Travellers.Add(character);
                        break;
                    case CharType.FABLED:
                        current.Fabled = character;
                        break;
                    case CharType.LORIC:
                        current.Loric = character;
                        break;
                }
            }
            if (failed)
                continue;
            current.Jinxes = ComputeJinxes(current);
            scripts.Add(current);
        }
        return scripts;
    }

    private static void PrintScriptInfo(Script script) {
        Console.WriteLine(Format.BoldText($"--- {script.Name} by {script.Author} ---"));
        Console.WriteLine("Townsfolk:");
        foreach (Character character in script.Townsfolk)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Outsiders:");
        foreach (Character character in script.Outsiders)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Minions:");
        foreach (Character character in script.Minions)
            Console.WriteLine($"\t{character}");
        Console.WriteLine("Demons:");
        foreach (Character character in script.Demons)
            Console.WriteLine($"\t{character}");
        if (script.Travellers.Count != 0) {
            Console.WriteLine("Possible Travellers:");
            foreach (Character character in script.Travellers)
                Console.WriteLine($"\t{character}");
        }
        if (script.Fabled != null) {
            Console.WriteLine($"Fabled:\n\t{script.Fabled}");
            if (script.Jinxes.Count != 0) {
                Console.WriteLine("\tJinxes:");
                foreach (var jinx in script.Jinxes)
                    Console.WriteLine($"\t{jinx.Pair.Item1} & {jinx.Pair.Item2}");
            }
        }
        if (script.Loric != null)
            Console.WriteLine($"Loric:\n\t{script.Loric}");
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
