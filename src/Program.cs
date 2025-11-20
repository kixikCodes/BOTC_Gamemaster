namespace BOTC;

public static class Format {
    public const string Reset = "\e[0m";
    public const string Bold = "\e[1m";

    public const string Blue = "\e[38;5;39m";
    public const string LightBlue = "\e[38;5;117m";
    public const string Red = "\e[38;5;203m";
    public const string DarkRed = "\e[38;5;160m";
    public const string Purple = "\e[38;5;135m";
    public const string Yellow = "\e[38;5;220m";
    public const string Green = "\e[38;5;34m";

    public static string Color(string text, string color)
        => $"{color}{text}{Reset}";

    public static string BoldText(string text)
        => $"{Bold}{text}{Reset}";
}

public class Program {
    static readonly string scriptsDir = "./Scripts";

    private static void PrintPreview(Game prepedGame) {
        Console.WriteLine("\n" + Format.BoldText("\t--- Role Assignments ---"));
        var assignments = prepedGame.Assignments;
        foreach (var role in assignments) {
            if (role.Value.Id == "drunk")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.DrunkFake})");
            else if (role.Value.Id == "grandmother")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.GrandmotherTarget})");
            else if (role.Value.Id == "lunatic")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.LunaticFake})");
            else if (role.Value.Id == "marionette")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.MarionetteFake})");
            else if (role.Value.Id == "eviltwin")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.EvilTwinTarget})");
            else
                Console.WriteLine($"{role.Key}: {role.Value}");
        }
        if (prepedGame.Fabled != null) {
            Console.WriteLine($"\nFabled: {prepedGame.Fabled}");
            if (prepedGame.ActiveJinxes.Count != 0) {
                Console.WriteLine("\tActive Jinxes:");
                foreach (var jinx in prepedGame.ActiveJinxes)
                    Console.WriteLine($"\t{jinx.Pair.Item1} & {jinx.Pair.Item2}");
            }
        }
        if (prepedGame.Loric != null)
            Console.WriteLine($"\nLoric: {prepedGame.Loric}");
        Console.WriteLine("\nThe possible minion/demon bluffs are:");
        foreach (Character bluff in prepedGame.Bluffs)
            Console.WriteLine(bluff);
    }

    private static List<Character> SetTravellers(Game prepedGame, Script script) {
        if (script.Travellers.Count == 0) {
            Console.WriteLine("This script has no available Travellers.");
            return [];
        }
        List<Character> chosen = [];
        while (true) {
            Console.WriteLine("\nAvailable Travellers:");
            for (int i = 0; i < script.Travellers.Count; i++)
                Console.WriteLine($"\t{i + 1}: {script.Travellers[i]}");
            Console.Write("Enter number to add traveller (or press Enter to finish): ");
            string? sel = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sel))
                break;
            if (!int.TryParse(sel, out int idx) || idx < 1 || idx > script.Travellers.Count) {
                Console.WriteLine("Invalid selection.");
                continue;
            }
            chosen.Add(script.Travellers[idx - 1]);
            Console.WriteLine("Traveller added.");
            if (prepedGame.Travellers.Count == 0)
                break;
        }
        return chosen;
    }

    private static void PrintSetupGame(Game setGame, Script script) {
        Console.WriteLine(Format.BoldText($"=== Game Setup: {script.Name} ==="));
        Console.WriteLine("\n" + Format.BoldText("\t--- Characters & Order ---\n"));
        var assignments = setGame.Assignments;
        foreach (var role in assignments) {
            if (role.Value.Id == "drunk")
                Console.WriteLine($"{role.Key}: {role.Value} ({setGame.DrunkFake})");
            else if (role.Value.Id == "grandmother")
                Console.WriteLine($"{role.Key}: {role.Value} ({setGame.GrandmotherTarget})");
            else if (role.Value.Id == "lunatic")
                Console.WriteLine($"{role.Key}: {role.Value} ({setGame.LunaticFake})");
            else if (role.Value.Id == "marionette")
                Console.WriteLine($"{role.Key}: {role.Value} ({setGame.MarionetteFake})");
            else if (role.Value.Id == "eviltwin")
                Console.WriteLine($"{role.Key}: {role.Value} ({setGame.EvilTwinTarget})");
            else
                Console.WriteLine($"{role.Key}: {role.Value}");
            Console.WriteLine($"\t{role.Value.Description}");
        }
        if (setGame.Travellers.Count != 0) {
            Console.WriteLine("\nSet Traveller:");
            foreach (var traveller in setGame.Travellers) {
                Console.WriteLine($"\t{traveller}");
                Console.WriteLine($"\t{traveller.Description}");
            }
            Console.WriteLine("Note - Traveller order and players must be assigned manually.");
        }
        Console.WriteLine("\n" + Format.BoldText("\t--- Storyteller & Special Rules ---"));
        if (setGame.Fabled == null && setGame.Loric == null)
            Console.WriteLine("None.");
        if (setGame.Fabled != null) {
            Console.WriteLine($"\nFabled: {setGame.Fabled} - {setGame.Fabled.Description}");
            if (setGame.Fabled.Id == "djinn" && setGame.ActiveJinxes.Count != 0) {
                Console.WriteLine("\tActive Jinxes:");
                foreach (var jinx in setGame.ActiveJinxes) {
                    Console.WriteLine($"\t{jinx.Pair.Item1} & {jinx.Pair.Item2}");
                    Console.WriteLine($"\t{jinx.Description}");
                }
            }
        }
        if (setGame.Loric != null) {
            Console.WriteLine($"\nLoric: {setGame.Loric} - {setGame.Loric.Description}");
            if (setGame.Loric.Id == "bootlegger")
                Console.WriteLine($"\t{setGame.BootleggerRule}");
        }
        Console.WriteLine("\n" + Format.BoldText("\t--- Possible Evil Team Bluffs ---\n"));
        foreach (Character bluff in setGame.Bluffs)
            Console.WriteLine(bluff);
        Console.WriteLine("\nGame created using BotC_Gamemaster.");
    }

    public static void Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(Format.BoldText("=== Blood on the Clocktower ==="));
        Console.WriteLine("Easy game setup system by kixikCodes.");

        // Parse all scripts
        List<Script> scripts = ScriptManager.ParseScripts(scriptsDir);
        // Set player count and names
        int playerCount = GameManager.SetPlayerCount();
        List<string> playerNames = GameManager.SetPlayers(playerCount);
        // Select script from parsed scripts
        Script script = ScriptManager.SetScript(scripts);
        // Run role assignment loop
        Game prepedGame;
        while (true) {
            Console.WriteLine("\nAssigning character roles...");
            prepedGame = GameManager.CreateGame(playerNames, script);
            // Print a preview of the game's role assignments
            PrintPreview(prepedGame);
            Console.Write("\nWould you like to reassign? (y/n) ");
            string? input = Console.ReadLine();
            if (input == "n")
                break;
            else if (input == "y")
                continue;
        }
        Console.WriteLine("Roles locked in.");
        // Ask for Traveller players and set them
        Console.Write("\nWould you like to add travellers? (y/n)");
        string? travellersInput = Console.ReadLine();
        if (travellersInput == "y")
            prepedGame.Travellers = SetTravellers(prepedGame, script);
        // If there is a Bootlegger, ask for homebrew rule
        if (prepedGame.Loric != null && prepedGame.Loric.Id == "bootlegger") {
            Console.WriteLine("\nA Bootlegger is present.");
            Console.WriteLine("Enter a house rule description for Bootlegger (or press Enter to skip):");
            string? rule = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(rule))
                prepedGame.BootleggerRule = rule;
        }
        // Print the final set-up game in detail and exit
        PrintSetupGame(prepedGame, script);
    }
}
