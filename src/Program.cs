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
            else if (role.Value.Id == "amnesiac")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.AmnesiacRole})");
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
        if (prepedGame.ActiveJinxes.Count != 0) {
            Console.WriteLine("\nJinxes:");
            foreach (var jinx in prepedGame.ActiveJinxes)
                Console.WriteLine($"\t{jinx.Pair.Item1} & {jinx.Pair.Item2}");
        }
        Console.WriteLine("\nThe possible minion/demon bluffs are:");
        foreach (Character bluff in prepedGame.Bluffs)
            Console.WriteLine($"\t{bluff}");
    }

    private static void PrintSetupGame(Game setGame, Script script) {
        Console.WriteLine(Format.BoldText($"\n=== Game Setup: {script.Name} ==="));
        Console.WriteLine("\n" + Format.BoldText("\t--- Characters & Order ---\n"));
        var assignments = setGame.Assignments;
        foreach (var role in assignments) {
            if (role.Value.Id == "drunk")
                Console.Write($"{role.Key}: {role.Value} ({setGame.DrunkFake})");
            else if (role.Value.Id == "grandmother")
                Console.Write($"{role.Key}: {role.Value} ({setGame.GrandmotherTarget})");
            else if (role.Value.Id == "amnesiac")
                Console.Write($"{role.Key}: {role.Value} ({setGame.AmnesiacRole})");
            else if (role.Value.Id == "lunatic")
                Console.Write($"{role.Key}: {role.Value} ({setGame.LunaticFake})");
            else if (role.Value.Id == "marionette")
                Console.Write($"{role.Key}: {role.Value} ({setGame.MarionetteFake})");
            else if (role.Value.Id == "eviltwin")
                Console.Write($"{role.Key}: {role.Value} ({setGame.EvilTwinTarget})");
            else
                Console.Write($"{role.Key}: {role.Value}");
            Console.WriteLine($" - {role.Value.Description}");
        }
        if (setGame.Travellers.Count != 0) {
            Console.WriteLine("\nTravellers:");
            foreach (var traveller in setGame.Travellers)
                Console.WriteLine($"{traveller} - {traveller.Description}");
            Console.WriteLine("Note - Traveller order and players must be assigned manually.");
        }
        Console.WriteLine("\n" + Format.BoldText("\t--- Storyteller & Special Rules ---"));
        if (setGame.Fabled == null && setGame.Loric == null)
            Console.WriteLine("None.");
        if (setGame.Fabled != null) {
            Console.WriteLine($"\nFabled: {setGame.Fabled} - {setGame.Fabled.Description}");
            if (setGame.Fabled.Id == "djinn" && setGame.ActiveJinxes.Count != 0) {
                Console.WriteLine("Active Jinxes:");
                foreach (var jinx in setGame.ActiveJinxes) {
                    Console.Write($"{jinx.Pair.Item1} & {jinx.Pair.Item2}");
                    Console.WriteLine($" - {jinx.Description}");
                }
            }
        }
        if (setGame.Loric != null) {
            Console.WriteLine($"\nLoric: {setGame.Loric} - {setGame.Loric.Description}");
            if (setGame.Loric.Id == "bootlegger")
                Console.WriteLine($"Bootlegger Rule: {setGame.BootleggerRule}");
        }
        Console.WriteLine("\n" + Format.BoldText("\t--- Possible Evil Team Bluffs ---\n"));
        foreach (Character bluff in setGame.Bluffs)
            Console.WriteLine($"{bluff} - {bluff.Description}");
        Console.WriteLine("\nGame created using BotC_Gamemaster.");
    }

    public static void Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(Format.BoldText("====== Blood on the Clocktower ======"));
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
            prepedGame.Travellers = GameManager.SetTravellers(prepedGame, script);
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
