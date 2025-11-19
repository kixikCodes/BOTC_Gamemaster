namespace BOTC;

public static class C {
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

    // FIXME: Once the Character class exists, it may be wiser to just put this there based on its Type.
    public static string ColorRole(string role) {
        string baseRole = role.Contains('(') ? role[..role.IndexOf(' ')] : role;

        if (BotcRoles.Townsfolk.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, Blue);
        if (BotcRoles.Outsiders.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, LightBlue);
        if (BotcRoles.Minions.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, Red);
        if (BotcRoles.Demons.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, DarkRed);
        if (BotcRoles.Travellers.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, Purple);
        if (BotcRoles.Fabled.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, Yellow);
        if (BotcRoles.Loric.Contains(baseRole, StringComparer.OrdinalIgnoreCase))
            return Color(role, Green);
        return role;
    }
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
            Console.WriteLine("\nAvailable Travellers:");
            foreach (string traveller in prepedGame.Travellers)
                Console.WriteLine(C.ColorRole(traveller));
        }
        if (prepedGame.Fabled != "") {
            Console.WriteLine("\nFabled: " + C.ColorRole(prepedGame.Fabled));
            if (prepedGame.ActiveJinxes.Count != 0) {
                Console.WriteLine("\tActive Jinxes:");
                foreach (var jinx in prepedGame.ActiveJinxes)
                    Console.WriteLine("\t" + C.ColorRole(jinx.Pair.Item1)
                        + " & " + C.ColorRole(jinx.Pair.Item2));
            }
        }
        if (prepedGame.Loric != "")
            Console.WriteLine("\nLoric: " + C.ColorRole(prepedGame.Loric));
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
        /* TODO:
            - Ask if Travellers are desired and register them.
            - If there is a Bootlegger present and there is no preestablished rule, ask use for rule.
            - Display a final set-up game with full role/jinx/rule descriptions, players and order.
        */
    }
}
