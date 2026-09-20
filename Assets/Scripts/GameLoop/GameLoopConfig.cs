using UnityEngine;

/// <summary>
/// Static tuning for the game loop: player lives, enemy patience, the fade
/// duration, and the win/lose scene routing. Holds NO runtime state — every
/// mutable value lives on GameLoopController (the MonoBehaviour), never in a
/// ScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "GameLoopConfig", menuName = "Cards/Game Loop Config")]
public class GameLoopConfig : ScriptableObject
{
    [Tooltip("Player lives per round. 0 lives → GAME OVER (round lost).")]
    public int maxLives = 3;

    [Tooltip("Enemy patience per round. At 0 patience the enemy bites (−1 life) and patience refills.")]
    public int maxPatience = 3;

    [Tooltip("Seconds for a full fade (black→transparent reveal or transparent→black cover).")]
    public float fadeDuration = 1f;

    [Tooltip("Scene loaded on WIN (after the fade cover): the Map, where the run advances. Empty/null falls back to \"Map\".")]
    public string nextSceneName = "Map";

    [Tooltip("Scene loaded on GAME OVER / LOSE (after the fade cover): retries the SAME level, so the map position is kept. Empty/null falls back to \"Prototype\".")]
    public string nextSceneOnLose = "Prototype";
}