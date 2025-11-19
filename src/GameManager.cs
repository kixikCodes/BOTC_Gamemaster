namespace BOTC;

public class Game {
    public Dictionary<string, Character> Assignments { get; set; } = [];
    public List<Character> Travellers { get; set; } = [];
    public Character? Fabled;
    public Character? Loric;
    public List<Jinx> ActiveJinxes { get; set; } = [];
    public List<Character> Bluffs { get; set; } = [];
    public Character? DrunkFake;
    public Character? LunaticFake;
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

    private record Roles(
        List<Character> PlayerRoles,
        List<Character> Bluffs,
        Character? DrunkFake,
        Character? LunaticFake
    );

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

    private static Roles PickRoles(Script script, List<string> players) {
        var (townsfolkCount, outsiderCount, minionCount, demonCount) 
            = RoleCountTable[players.Count];
        static Character PickRandom(List<Character> list) => list[rng.Next(list.Count)];

        Character demon = PickRandom(script.Demons);
        
        List<Character> selectedMinions = [.. script.Minions
            .OrderBy(_ => rng.Next())
            .Take(minionCount)
        ];
        if (selectedMinions.Exists(c => c.Id == "baron")) {
            outsiderCount++;
            townsfolkCount--;
        }

        List<Character> selectedOutsiders = [.. script.Outsiders
            .OrderBy(_ => rng.Next())
            .Take(outsiderCount)
        ];

        List<Character> selectedTownsfolk = [.. script.Townsfolk
            .OrderBy(_ => rng.Next())
            .Take(townsfolkCount)
        ];

        Character? drunkFake = null;
        if (selectedOutsiders.Exists(c => c.Id == "drunk")) {
            List<Character> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            drunkFake = PickRandom(remainingTownsfolk);
        }

        Character? lunaticFake = null;

        List<Character> roles = [demon];
        roles.AddRange(selectedMinions);
        roles.AddRange(selectedOutsiders);
        roles.AddRange(selectedTownsfolk);

        List<Character> allGoodSelected = [.. selectedTownsfolk, .. selectedOutsiders];
        var allPossible = script.Townsfolk.Concat(script.Outsiders).ToHashSet();
        foreach (Character role in allGoodSelected)
            allPossible.Remove(role);
        allPossible.RemoveWhere(c => c.Id == "drunk");
        if (drunkFake != null)
            allPossible.Remove(drunkFake);

        return new Roles(
            PlayerRoles: roles,
            Bluffs: [.. allPossible],
            DrunkFake: drunkFake,
            LunaticFake: lunaticFake
        );
    }

    private static Dictionary<string, Character> AssignRoles(List<string> players, List<Character> roles)
    {
        Dictionary<string, Character> assignments = [];
        List<string> shuffledPlayers = [.. players.OrderBy(_ => rng.Next())];
        List<Character> shuffledRoles = [.. roles.OrderBy(_ => rng.Next())];
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
        game.Assignments = AssignRoles(players, selected.PlayerRoles);
        game.DrunkFake = selected.DrunkFake;
        game.ActiveJinxes = [];
        if (script.Fabled != null && string.Equals(script.Fabled.Id, "djinn")) {
            var assigned = new HashSet<Character>(selected.PlayerRoles);
            foreach (var jinx in BotcRoles.Jinxes)
                if (assigned.Contains(jinx.Pair.Item1) && assigned.Contains(jinx.Pair.Item2))
                    game.ActiveJinxes.Add(jinx);
        }
        game.Bluffs = selected.Bluffs;

        return game;
    }
}
