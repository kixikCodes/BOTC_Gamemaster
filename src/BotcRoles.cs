namespace BOTC;

public class BotcRoles {
    public enum CharType
    {
        TOWNSFOLK,
        OUTSIDER,
        MINION,
        DEMON,
        TRAVELLER
    }

    public static readonly List<string> Townsfolk = [
        "acrobat", "alchemist", "alsaahir", "amnesiac", "artist", "atheist", "balloonist", "banshee",
        "bountyhunter", "cannibal", "chambermaid", "chef", "choirboy", "clockmaker", "courtier",
        "cultleader", "dreamer", "empath", "engineer", "exorcist", "farmer", "fisherman", "flowergirl",
        "fool", "fortuneteller", "gambler", "general", "gossip", "grandmother", "highpriestess",
        "huntsman", "innkeeper", "investigator", "juggler", "king", "knight", "librarian", "lycanthrope",
        "magician", "mathematician", "mayor", "minstrel", "monk", "nightwatchman", "noble", "oracle",
        "pacifist", "philosopher", "pixie", "poppygrower", "preacher", "princess", "professor",
        "ravenkeeper", "sage", "sailor", "savant", "seamstress", "shugenja", "slayer", "snakecharmer",
        "soldier", "steward", "tealady", "towncrier", "undertaker", "villageidiot", "virgin",
        "washerwoman"
    ];

    public static readonly List<string> Outsiders = [
        "barber", "butler", "damsel", "drunk", "golem", "goon", "hatter", "heretic", "hermit", "klutz",
        "lunatic", "moonchild", "mutant", "ogre", "plaguedoctor", "politician", "puzzlemaster",
        "recluse", "saint", "snitch", "sweetheart", "tinker", "zealot"
    ];

    public static readonly List<string> Minions = [
        "assassin", "baron", "boffin", "boomdandy", "cerenovus", "devilsadvocate", "eviltwin",
        "fearmonger", "goblin", "godfather", "harpy", "marionette", "mastermind", "mezepheles",
        "organgrinder", "pithag", "poisoner", "psychopath", "scarletwoman", "spy", "summoner",
        "vizier", "widow", "witch", "wizard", "wraith", "xaan"
    ];

    public static readonly List<string> Demons = [
        "alhadikhia", "fanggu", "imp", "kazali", "legion", "leviathan", "lilmonsta", "lleech",
        "lordoftyphon", "nodashii", "ojo", "po", "pukka", "riot", "shabaloth", "vigormortis",
        "vortox", "yaggababble", "zombuul"
    ];

    public static readonly List<string> Travellers = [
        "scapegoat", "gunslinger", "beggar", "bureaucrat", "thief", "butcher", "bonecollector",
        "harlot", "barista", "deviant", "apprentice", "matron", "voudon", "judge", "bishop",
        "cacklejack", "gangster", "gnome"
    ];

    public static readonly List<string> Fabled = [
        "angle", "buddhist", "doomslayer", "hellslibrarian", "fiddler", "revolutionary", "toymaker",
        "djinn", "duchess", "fibbin", "sentinel", "spiritofivory", "deusexfiasco", "ferryman"
    ];

    public static readonly List<string> Loric = [
        "bootlegger", "bigwig", "gardener", "stormcatcher", "tor"
    ];

    public static readonly Dictionary<string, CharType> Lookup;

    //TODO: Create Djinn's jinx combination dictionary.

    static BotcRoles()
    {
        Lookup = [];
        _ = new BotcRoles();

        foreach (var name in Townsfolk)
            Lookup[name] = CharType.TOWNSFOLK;
        foreach (var name in Outsiders)
            Lookup[name] = CharType.OUTSIDER;
        foreach (var name in Minions)
            Lookup[name] = CharType.MINION;
        foreach (var name in Demons)
            Lookup[name] = CharType.DEMON;
        foreach (var name in Travellers)
            Lookup[name] = CharType.TRAVELLER;
    }
}
