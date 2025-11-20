using System.Text.Json;
namespace BOTC;

public enum CharType
{
    TOWNSFOLK,
    OUTSIDER,
    MINION,
    DEMON,
    TRAVELLER,
    FABLED,
    LORIC
}

public enum CharTeam
{
    GOOD,
    EVIL,
    NEUTRAL
}

public class Character {
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public CharType Type { get; set; }
    public CharTeam Team { get; set; }
    public string Description { get; set; } = "";

    public override string ToString() => ColorRole();

    public string ColorRole() {
        return Type switch {
            CharType.TOWNSFOLK => Format.Color(Name, Format.Blue),
            CharType.OUTSIDER => Format.Color(Name, Format.LightBlue),
            CharType.MINION => Format.Color(Name, Format.Red),
            CharType.DEMON => Format.Color(Name, Format.DarkRed),
            CharType.TRAVELLER => Format.Color(Name, Format.Purple),
            CharType.FABLED => Format.Color(Name, Format.Yellow),
            CharType.LORIC => Format.Color(Name, Format.Green),
            _ => Name
        };
    }
}

public class Jinx(Character a, Character b, string desc)
{
    public (Character, Character) Pair = (a, b);
    public string Description { get; set; } = desc;
}

public class BotcTokens {
    static readonly string characterData = "./Resources/characters.json";
    static readonly string jinxData = "./Resources/jinxes.json";

    public static readonly Dictionary<string, Character> Characters;

    private record JinxJson(string A, string B, string Desc);

    public static List<Jinx> LoadJinxes(string path)
    {
        var json = File.ReadAllText(path);
        var items = JsonSerializer.Deserialize<List<JinxJson>>(json);
        var result = new List<Jinx>();
        foreach (var item in items!) {
            result.Add(new Jinx(
                Characters[item.A],
                Characters[item.B]!,
                item.Desc)
            );
        }
        return result;
    }

    public static readonly List<Jinx> Jinxes;

    static BotcTokens()
    {
        Characters = [];
        Jinxes = [];
        var json = File.ReadAllText(characterData);
        using var doc = JsonDocument.Parse(json);
        foreach (var section in doc.RootElement.EnumerateObject()) {
            CharType type = section.Name switch {
                "townsfolk" => CharType.TOWNSFOLK,
                "outsiders" => CharType.OUTSIDER,
                "minions" => CharType.MINION,
                "demons" => CharType.DEMON,
                "travellers" => CharType.TRAVELLER,
                "fabled" => CharType.FABLED,
                "loric" => CharType.LORIC,
                _ => throw new Exception("Unknown type: " + section.Name)
            };
            CharTeam team = type switch {
                CharType.TOWNSFOLK or CharType.OUTSIDER => CharTeam.GOOD,
                CharType.MINION or CharType.DEMON => CharTeam.EVIL,
                _ => CharTeam.NEUTRAL
            };
            foreach (var entry in section.Value.EnumerateArray()) {
                var id = entry.GetProperty("id").GetString() ?? "";
                var name = entry.GetProperty("name").GetString() ?? "";
                var desc = entry.GetProperty("desc").GetString() ?? "";
                Characters[id] = new Character {
                    Id = id,
                    Name = name,
                    Type = type,
                    Team = team,
                    Description = desc
                };
            }
        }
        Jinxes = LoadJinxes(jinxData);
    }
}
