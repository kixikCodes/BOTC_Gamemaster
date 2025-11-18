namespace BOTC;

public static class C {
    public const string Reset = "\u001b[0m";
    public const string Bold = "\u001b[1m";

    public const string Blue = "\u001b[34m";
    public const string Cyan = "\u001b[36m";
    public const string Red = "\u001b[31m";
    public const string DarkRed = "\u001b[38;5;88m";
    public const string Magenta = "\u001b[35m";
    public const string Yellow = "\u001b[33m";
    public const string Green = "\u001b[32m";

    public static string Color(string text, string color)
        => $"{color}{text}{Reset}";

    public static string BoldText(string text)
        => $"{Bold}{text}{Reset}";

    public static string ColorRole(string role) {
        string baseRole = role.Contains('(') ? role[..role.IndexOf(' ')] : role;

        if (BotcRoles.Townsfolk.Contains(baseRole))
            return Color(role, Blue);
        if (BotcRoles.Outsiders.Contains(baseRole))
            return Color(role, Cyan);
        if (BotcRoles.Minions.Contains(baseRole))
            return Color(role, Red);
        if (BotcRoles.Demons.Contains(baseRole))
            return Color(role, DarkRed);
        return role;
    }

    public static string ColorTraveller(string t) => Color(t, Magenta);
    public static string ColorFabled(string f) => Color(f, Yellow);
    public static string ColorLoric(string l) => Color(l, Green);
}

public class Program {
    static readonly string scriptsDir = "./Scripts";

    private static void PrintGameInfo(Game prepedGame) {
        Console.WriteLine("\n" + C.BoldText("\t--- Role Assignments ---"));
        var assignments = prepedGame.Assignments;
        foreach (var role in assignments) {
            string colored = C.ColorRole(role.Value);
            Console.WriteLine($"{role.Key}: {colored}");
        }
        if (prepedGame.Travellers.Count != 0) {
            Console.WriteLine("\n" + C.Color("Available Travellers:", C.Magenta));
            foreach (string traveller in prepedGame.Travellers)
                Console.WriteLine(C.ColorTraveller(traveller));
        }
        if (prepedGame.Fabled != "")
            Console.WriteLine("\nFabled: " + C.ColorFabled(prepedGame.Fabled));
        if (prepedGame.Loric != "")
            Console.WriteLine("\nLoric: " + C.ColorLoric(prepedGame.Loric));
        Console.WriteLine("\nThe possible minion/demon bluffs are:");
        foreach (string bluff in prepedGame.Bluffs)
            Console.WriteLine(C.ColorRole(bluff));
    }

    public static void Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(C.BoldText("=== Blood on the Clocktower ==="));
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
        // Once roles are locked in, ask if gamemaster CLI should be laucnhed.
        // Further game utilities will be accessed through that.
    }
}
