namespace BOTC;

public class BotcRoles {
    public enum CharType
    {
        TOWNSFOLK,
        OUTSIDER,
        MINION,
        DEMON
    }

    private readonly List<string> Townsfolk = [
        "acrobat", "alchemist", "alsaahir", "amnesiac", "artist", "atheist", "balloonist", "banshee",
        "bountyhunter", "cannibal", "chambermaid", "chef", "choirboy", "clockmaker", "courtier",
        "cultleader", "dreamer", "empath", "engineer", "exorcist", "farmer", "fisherman", "flowergirl",
        "fool", "fortuneteller", "gambler", "general", "gossip", "grandmother", "highpriestess",
        "huntsman", "innkeeper", "investigator", "juggler", "king", "knight", "librarian","lycanthrope",
        "magician", "mathematician", "mayor", "minstrel", "monk", "nightwatchman", "noble", "oracle",
        "pacifist", "philosopher", "pixie", "poppygrower", "preacher", "princess", "professor",
        "ravenkeeper", "sage", "sailor", "savant", "seamstress", "shugenja", "slayer", "snakecharmer",
        "soldier", "steward", "tealady", "towncrier", "undertaker", "villageidiot", "virgin",
        "washerwoman"
    ];

    private readonly List<string> Outsiders = [
        "barber", "butler", "damsel", "drunk", "golem", "goon", "hatter", "heretic", "hermit", "klutz",
        "lunatic", "moonchild", "mutant", "ogre", "plaguedoctor", "politician", "puzzlemaster",
        "recluse", "saint", "snitch", "sweetheart", "tinker", "zealot"
    ];

    private readonly List<string> Minions = [
        "assassin", "baron", "boffin", "boomdandy", "cerenovus", "devilsadvocate", "eviltwin",
        "fearmonger", "goblin", "godfather", "harpy", "marionette", "mastermind", "mezepheles",
        "organgrinder", "pithag", "poisoner", "psychopath", "scarletwoman", "spy", "summoner",
        "vizier", "widow", "witch", "wizard", "wraith", "xaan"
    ];

    private readonly List<string> Demons = [
        "alhadikhia", "fanggu", "imp", "kazali", "legion", "leviathan", "lilmonsta", "lleech",
        "lordoftyphon", "nodashii", "ojo", "po", "pukka", "riot", "shabaloth", "vigormortis",
        "vortox", "yaggababble", "zombuul"
    ];

    // Add Jinxes, Lorics and Travelers

    public static readonly Dictionary<string, CharType> Lookup;

    static BotcRoles()
    {
        Lookup = [];
        var c = new BotcRoles();

        foreach (var name in c.Townsfolk)
            Lookup[name] = CharType.TOWNSFOLK;
        foreach (var name in c.Outsiders)
            Lookup[name] = CharType.OUTSIDER;
        foreach (var name in c.Minions)
            Lookup[name] = CharType.MINION;
        foreach (var name in c.Demons)
            Lookup[name] = CharType.DEMON;
    }
}
