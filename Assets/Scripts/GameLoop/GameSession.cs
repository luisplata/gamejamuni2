/// <summary>
/// Static run state for the menu → map → game flow. Survives scene loads
/// (static fields are process-wide), resets each PlayMode session, and never
/// persists to disk. currentLevelIndex is 0-based into RecipeDatabase.levels:
/// 0 = México, 1 = Colombia. It NEVER exceeds 1 — a last-level win routes to
/// Credits without advancing.
/// </summary>
public static class GameSession
{
    /// <summary>Current level index, 0-based into RecipeDatabase.levels (0 = México, 1 = Colombia).</summary>
    public static int currentLevelIndex = 0;

    /// <summary>Fresh run: back to México. Called by the Menu when a run starts.</summary>
    public static void ResetRun() => currentLevelIndex = 0;
}