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

    private static void PrintGameInfo(Game prepedGame) {
        Console.WriteLine("\n" + Format.BoldText("\t--- Role Assignments ---"));
        var assignments = prepedGame.Assignments;
        foreach (var role in assignments) {
            if (role.Value.Id == "drunk")
                Console.WriteLine($"{role.Key}: {role.Value} ({prepedGame.DrunkFake})");
            else
                Console.WriteLine($"{role.Key}: {role.Value}");
        }
        if (prepedGame.Travellers.Count != 0) {
            Console.WriteLine("\nAvailable Travellers:");
            foreach (Character traveller in prepedGame.Travellers)
                Console.WriteLine(traveller);
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
        while (true) {
            Console.WriteLine("\nAssigning character roles...");
            Game prepedGame = GameManager.CreateGame(playerNames, script);

            PrintGameInfo(prepedGame);

            Console.Write("\nWould you like to reassign? (y/n) ");
            string? input = Console.ReadLine();
            if (input == "n")
                break;
            else if (input == "y")
                continue;
        }
        /* TODO:
            - Ask if Travellers are desired and register them.
            - If there is a Bootlegger present and there is no preestablished rule, ask use for rule.
            - Display a final set-up game with full role/jinx/rule descriptions, players and order.
        */
    }
}
