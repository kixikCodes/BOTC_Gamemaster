namespace BOTC;

public class Program {
    static readonly string scriptsDir = "./Scripts";

    public static void Main() {
        Console.WriteLine("=== Blood on the Clocktower ===");
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
            var playerRoles = GameManager.AssignRoles(playerNames, script);

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
        // Once roles are locked in, ask if gamemaster CLI should be laucnhed.
        // Further game utilities will be accessed through that.
    }
}
