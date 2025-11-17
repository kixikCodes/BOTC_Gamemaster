namespace BOTC;

public class GameManager {
    static readonly Random rng = new();

    public static int SetPlayerCount() {
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
        return playerCount;
    }

    public static List<string> SetPlayers(int playerCount) {
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
        return playerNames;
    }

    public static Dictionary<string, string> AssignRoles(List<string> players, Script script) {
        static string PickRandom(List<string> list) => list[rng.Next(list.Count)];

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

    // Add more gamemaster utilities

}
