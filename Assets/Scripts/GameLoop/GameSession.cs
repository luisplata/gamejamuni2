using System.Collections.Generic;

/// <summary>
/// Static run state for the menu → map → game flow. Survives scene loads
/// (static fields are process-wide), resets each PlayMode session, and never
/// persists to disk. currentLevelIndex is 0-based into RecipeDatabase.levels:
/// 0 = México, 1 = Colombia. It NEVER exceeds 1 — a last-level win routes to
/// Credits without advancing.
///
/// Per-run economy: coins earned by winning rounds, purchasedCards granted by
/// the Market scene (appended to the deck on the next deal), and soldOut — the
/// set of cards already bought this run (one purchase per item per run). All
/// three are cleared by ResetRun so a fresh run starts with an empty wallet.
/// </summary>
public static class GameSession
{
    /// <summary>Current level index, 0-based into RecipeDatabase.levels (0 = México, 1 = Colombia).</summary>
    public static int currentLevelIndex = 0;

    /// <summary>Coins earned this run (win grants). Spent in the Market scene.</summary>
    public static int coins = 0;

    /// <summary>Card copies bought in the Market this run; appended to the deck before the next deal.</summary>
    public static List<CardData> purchasedCards = new();

    /// <summary>Cards already bought this run — one purchase per item per run (rebuy blocked).</summary>
    public static HashSet<CardData> soldOut = new();

    /// <summary>
    /// Tutorial override level: when non-null, HandController.Awake picks this
    /// instead of the RecipeDatabase level (design D1). Set by MenuController
    /// BEFORE loading the Tutorial scene; cleared by ResetRun so a real run
    /// never inherits the tutorial identity.
    /// </summary>
    public static LevelConfig tutorialLevel;

    /// <summary>Fresh run: back to México with an empty wallet and no purchases.</summary>
    public static void ResetRun()
    {
        currentLevelIndex = 0;
        coins = 0;
        purchasedCards = new List<CardData>();
        soldOut = new HashSet<CardData>();
        tutorialLevel = null;
    }
}