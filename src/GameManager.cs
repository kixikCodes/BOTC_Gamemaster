namespace BOTC;

public class Game {
    public Dictionary<string, string> Assignments { get; set; } = [];
    public List<string> Travellers { get; set; } = [];
    public string Fabled { get; set; } = "";
    public string Loric { get; set; } = "";
    public List<string> Bluffs { get; set; } = [];
}

public class GameManager {
    static readonly Random rng = new();

    static readonly Dictionary<int, (int Townsfolk, int Outsiders, int Minions, int Demons)>
    RoleCountTable = new() {
        { 5, (4, 0, 0, 1) },
        { 6, (4, 0, 1, 1) },
        { 7, (4, 1, 1, 1) },
        { 8, (5, 1, 1, 1) },
        { 9, (5, 2, 1, 1) },
        {10, (5, 2, 2, 1) },
        {11, (6, 2, 2, 1) },
        {12, (6, 3, 2, 1) }
    };

    private record RoleResult(List<string> Roles, List<string> Bluffs);

    public static int SetPlayerCount() {
        int playerCount;
        while (true) {
            Console.WriteLine("\nHow many players? (5 - 12)");
            try {
                playerCount = Convert.ToInt32(Console.ReadLine());
                if (playerCount >= 5 && playerCount <= 12)
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

    private static RoleResult PickRoles(Script script, List<string> players) {
        var (townsfolkCount, outsiderCount, minionCount, demonCount) 
            = RoleCountTable[players.Count];
        static string PickRandom(List<string> list) => list[rng.Next(list.Count)];

        string demon = PickRandom(script.Demons);
        
        List<string> selectedMinions = [.. script.Minions
            .OrderBy(_ => rng.Next())
            .Take(minionCount)
        ];
        if (selectedMinions.Contains("baron")) {
            outsiderCount++;
            townsfolkCount--;
        }

        List<string> selectedOutsiders = [.. script.Outsiders
            .OrderBy(_ => rng.Next())
            .Take(outsiderCount)
        ];

        List<string> selectedTownsfolk = [.. script.Townsfolk
            .OrderBy(_ => rng.Next())
            .Take(townsfolkCount)
        ];

        string? drunkFake = null;
        if (selectedOutsiders.Contains("drunk")) {
            List<string> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            drunkFake = PickRandom(remainingTownsfolk);
            selectedOutsiders.Remove("drunk");
            selectedOutsiders.Add($"drunk ({drunkFake})");
        }

        List<string> roles = [demon];
        roles.AddRange(selectedMinions);
        roles.AddRange(selectedOutsiders);
        roles.AddRange(selectedTownsfolk);

        List<string> allGoodSelected = [.. selectedTownsfolk, .. selectedOutsiders];
        var allPossible = script.Townsfolk.Concat(script.Outsiders).ToHashSet();
        foreach (string role in allGoodSelected)
            allPossible.Remove(role);
        allPossible.Remove("drunk");
        if (drunkFake != null)
            allPossible.Remove(drunkFake);

        return new RoleResult(Roles: roles, Bluffs: [.. allPossible]);
    }

    private static Dictionary<string, string> AssignRoles(List<string> players, List<string> roles)
    {
        Dictionary<string, string> assignments = [];
        List<string> shuffledPlayers = [.. players.OrderBy(_ => rng.Next())];
        List<string> shuffledRoles = [.. roles.OrderBy(_ => rng.Next())];
        for (int i = 0; i < players.Count; i++)
            assignments[shuffledPlayers[i]] = shuffledRoles[i];

        return assignments;
    }

    public static Game CreateGame(List<string> players, Script script) {
        Game game = new() {
            Travellers = script.Travellers,
            Fabled = script.Fabled,
            Loric = script.Loric
        };
        
        var selected = PickRoles(script, players);
        game.Assignments = AssignRoles(players, selected.Roles);
        game.Bluffs = selected.Bluffs;

        //TODO: Compute Jinxes if Djinn is present.

        return game;
    }
}
