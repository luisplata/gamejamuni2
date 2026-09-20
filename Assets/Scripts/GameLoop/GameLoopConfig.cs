using UnityEngine;

/// <summary>
/// Static tuning for the game loop: player lives, enemy patience, and the
/// fade duration. Holds NO runtime state — every mutable value lives on
/// GameLoopController (the MonoBehaviour), never in a ScriptableObject.
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

    [Tooltip("Scene loaded on WIN or GAME OVER (after the fade cover). Empty/null falls back to the currently active scene's name.")]
    public string nextSceneName = "Prototype";
}