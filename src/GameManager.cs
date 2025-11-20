namespace BOTC;

public class Game {
    public Dictionary<string, Character> Assignments { get; set; } = [];
    public List<Character> Travellers { get; set; } = [];
    public Character? Fabled;
    public Character? Loric;
    public List<Jinx> ActiveJinxes { get; set; } = [];
    public List<Character> Bluffs { get; set; } = [];
    public Character? AmnesiacRole;
    public Character? DrunkFake;
    public Character? LunaticFake;
    public Character? MarionetteFake;
    public string GrandmotherTarget = "";
    public string EvilTwinTarget = "";
    public string BootleggerRule = "";
}

public class GameManager {
    static readonly Random rng = new();

    static T PickRandom<T>(List<T> list) => list[rng.Next(list.Count)];

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
        Character? LunaticFake,
        Character? MarionetteFake,
        Character? AmnesiacRole
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

    private static Roles LegionGame(List<string> players, Script script, Character legion) {
        int playerCount = players.Count;
        int legionCount = Math.Max(1, (int)Math.Round(playerCount * 0.66));
        int townsfolkCount = playerCount - legionCount;
        var selectedTownsfolk = script.Townsfolk
            .OrderBy(_ => rng.Next())
            .Take(townsfolkCount)
            .ToList();
        List<Character> roles = [];
        for (int i = 0; i < legionCount; i++)
            roles.Add(legion);
        roles.AddRange(selectedTownsfolk);
        var bluffs = script.Townsfolk
            .Where(t => !selectedTownsfolk.Contains(t))
            .ToList();
        return new Roles(
            PlayerRoles: roles,
            Bluffs: bluffs,
            DrunkFake: null,
            LunaticFake: null,
            MarionetteFake: null,
            AmnesiacRole: null
        );
    }

    private static Roles PickRoles(Script script, List<string> players) {
        var (townsfolkCount, outsiderCount, minionCount, demonCount) 
            = RoleCountTable[players.Count];
        // Select demon, handle Legion if picked.
        Character demon = PickRandom(script.Demons);
        if (demon.Id == "legion")
            return LegionGame(players, script, demon);
        // Select minions, handle Baron manipulation.
        List<Character> selectedMinions = [.. script.Minions
            .OrderBy(_ => rng.Next())
            .Take(minionCount)
        ];
        if (selectedMinions.Exists(c => c.Id == "baron")) {
            outsiderCount++;
            townsfolkCount--;
        }
        // Select outsiders.
        List<Character> selectedOutsiders = [.. script.Outsiders
            .OrderBy(_ => rng.Next())
            .Take(outsiderCount)
        ];
        // Select townsfolk.
        List<Character> selectedTownsfolk = [.. script.Townsfolk
            .OrderBy(_ => rng.Next())
            .Take(townsfolkCount)
        ];
        // Select a true role for the Amnesiac if there is one.
        Character? amnesiacRole = null;
        if (selectedTownsfolk.Exists(c => c.Id == "amnesiac")) {
            List<Character> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            amnesiacRole = PickRandom(remainingTownsfolk);
        }
        // Select a fake role for the Drunk if there is one.
        Character? drunkFake = null;
        if (selectedOutsiders.Exists(c => c.Id == "drunk")) {
            List<Character> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            drunkFake = PickRandom(remainingTownsfolk);
        }
        // Select a fake role for the Marionette if there is one.
        Character? marionetteFake = null;
        if (selectedMinions.Exists(c => c.Id == "marionette")) {
            List<Character> remainingTownsfolk = [.. script.Townsfolk
                .Except(selectedTownsfolk)
            ];
            marionetteFake = PickRandom(remainingTownsfolk);
        }
        // Select a fake demon for the Lunatic if there is one.
        Character? lunaticFake = null;
        if (selectedOutsiders.Exists(c => c.Id == "lunatic"))
            lunaticFake = PickRandom([.. script.Demons.Where(c => c.Id != demon.Id)]);
        // Combine all selected roles.
        List<Character> roles = [demon];
        roles.AddRange(selectedMinions);
        roles.AddRange(selectedOutsiders);
        roles.AddRange(selectedTownsfolk);
        // Compute bluffs for Evil team.
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
            LunaticFake: lunaticFake,
            MarionetteFake: marionetteFake,
            AmnesiacRole: amnesiacRole
        );
    }

    private static Dictionary<string, Character> AssignRoles(List<string> players, List<Character> roles) {
        Dictionary<string, Character> assignments = [];
        List<string> shuffledPlayers = [.. players.OrderBy(_ => rng.Next())];
        List<Character> shuffledRoles = [.. roles.OrderBy(_ => rng.Next())];
        for (int i = 0; i < players.Count; i++)
            assignments[shuffledPlayers[i]] = shuffledRoles[i];

        return assignments;
    }

    private static string SetRoleTarget(Roles selected, Dictionary<string, Character> assignments, string id) {
        string target = "";
        if (selected.PlayerRoles.Exists(c => c.Id == id)) {
            string relevantPlayer = assignments
                .First(kv => kv.Value.Id == id)
                .Key;
            var validTargets = assignments
                .Where(kv =>
                    kv.Key != relevantPlayer &&
                    (kv.Value.Type == CharType.TOWNSFOLK || kv.Value.Type == CharType.OUTSIDER))
                .Select(kv => kv.Key)
                .ToList();
            target = PickRandom(validTargets);
        }
        return target;
    }

    public static List<Character> SetTravellers(Game prepedGame, Script script) {
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

    public static Game CreateGame(List<string> players, Script script) {
        Game game = new() {
            Fabled = script.Fabled,
            Loric = script.Loric
        };
        
        var selected = PickRoles(script, players);
        game.Assignments = AssignRoles(players, selected.PlayerRoles);
        game.AmnesiacRole = selected.AmnesiacRole;
        game.DrunkFake = selected.DrunkFake;
        game.LunaticFake = selected.LunaticFake;
        game.MarionetteFake = selected.MarionetteFake;
        game.GrandmotherTarget = SetRoleTarget(selected, game.Assignments, "grandmother");
        game.EvilTwinTarget = SetRoleTarget(selected, game.Assignments, "eviltwin");
        game.ActiveJinxes = [];
        if (script.Fabled != null && string.Equals(script.Fabled.Id, "djinn")) {
            var assigned = new HashSet<Character>(selected.PlayerRoles);
            foreach (var jinx in BotcTokens.Jinxes)
                if (assigned.Contains(jinx.Pair.Item1) && assigned.Contains(jinx.Pair.Item2))
                    game.ActiveJinxes.Add(jinx);
        }
        game.Bluffs = selected.Bluffs;

        return game;
    }
}
