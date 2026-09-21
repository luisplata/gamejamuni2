using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Map scene controller: shows the run's two level nodes (México top,
/// Colombia bottom — labels come from RecipeDatabase.levels), places the
/// player marker on the current level, and animates it down to Colombia when
/// the run arrives after winning México (GameSession.currentLevelIndex > 0
/// means "came from a win" — the Map is only reachable from the Menu after a
/// reset, or from a win). JUGAR starts the current level in Prototype.
/// </summary>
public class MapController : MonoBehaviour
{
    [Tooltip("Global level catalog; node labels + JUGAR text read from levels[currentLevelIndex].")]
    [SerializeField] RecipeDatabase database;

    [Tooltip("Player marker Image (RectTransform) placed on the current node.")]
    [SerializeField] RectTransform marker;

    [Tooltip("Top node (México).")]
    [SerializeField] RectTransform nodeMexico;

    [Tooltip("Bottom node (Colombia).")]
    [SerializeField] RectTransform nodeColombia;

    [Tooltip("Top node label (México).")]
    [SerializeField] TMP_Text labelMexico;

    [Tooltip("Bottom node label (Colombia).")]
    [SerializeField] TMP_Text labelColombia;

    [Tooltip("JUGAR button label — shows the current level's region, e.g. 'JUGAR MÉXICO'.")]
    [SerializeField] TMP_Text playLabel;

    [Tooltip("Full-screen black overlay; the map reveals behind the fade-in.")]
    [SerializeField] FadeController fade;

    /// <summary>Seconds for the win-travel marker animation (México → Colombia).</summary>
    const float TravelDuration = 1.5f;

    /// <summary>
    /// Node labels from the recipe database (regionName — levelName), matching
    /// the game scene's level identity. Bounds-guarded: unwired/missing levels
    /// leave the labels untouched.
    /// </summary>
    void Awake()
    {
        if (database == null || database.levels == null) return;
        if (database.levels.Count > 0)
        {
            if (labelMexico != null) labelMexico.text = NodeLabel(0);
        }
        if (database.levels.Count > 1)
        {
            if (labelColombia != null) labelColombia.text = NodeLabel(1);
        }
    }

    /// <summary>"REGION — LevelName" for node <paramref name="index"/> (fallback: region only).</summary>
    string NodeLabel(int index)
    {
        var level = database.levels[index];
        if (level == null) return "";
        return string.IsNullOrEmpty(level.levelName)
            ? level.regionName
            : $"{level.regionName} — {level.levelName}";
    }

    /// <summary>
    /// Reveal the map behind the fade-in, then place the marker. A run arriving
    /// at index &gt; 0 (won México) animates the marker down to Colombia; a fresh
    /// run (index 0) just places it on México.
    /// </summary>
    void Start()
    {
        if (fade != null) fade.FadeIn(OnRevealed);
        else OnRevealed();
    }

    void OnRevealed()
    {
        if (GameSession.currentLevelIndex > 0) StartCoroutine(Travel());
        else Place();
    }

    /// <summary>Win-travel: lerp the marker from México to Colombia, then place.</summary>
    IEnumerator Travel()
    {
        if (marker == null || nodeMexico == null || nodeColombia == null)
        {
            Place();
            yield break;
        }

        Vector2 from = nodeMexico.anchoredPosition;
        Vector2 to = nodeColombia.anchoredPosition;
        float t = 0f;
        while (t < TravelDuration)
        {
            t += Time.deltaTime;
            marker.anchoredPosition = Vector2.Lerp(from, to, Mathf.Clamp01(t / TravelDuration));
            yield return null;
        }
        Place();
    }

    /// <summary>Snap the marker to the current node and refresh the JUGAR label.</summary>
    void Place()
    {
        if (marker != null)
        {
            if (GameSession.currentLevelIndex == 0 && nodeMexico != null)
                marker.anchoredPosition = nodeMexico.anchoredPosition;
            else if (GameSession.currentLevelIndex == 1 && nodeColombia != null)
                marker.anchoredPosition = nodeColombia.anchoredPosition;
        }

        if (playLabel != null && database != null && database.levels != null
            && GameSession.currentLevelIndex >= 0
            && GameSession.currentLevelIndex < database.levels.Count
            && database.levels[GameSession.currentLevelIndex] != null)
        {
            playLabel.text = $"JUGAR {database.levels[GameSession.currentLevelIndex].regionName}";
        }
    }

    /// <summary>
    /// JUGAR: start the current level's pipeline — Legend (lore) → Market
    /// (catalog) → Prototype. The game scene picks the level from GameSession.
    /// This is the ONLY entry-side routing change of the pipeline.
    /// </summary>
    public void PlayLevel()
    {
        SceneManager.LoadScene(Scenes.Legend);
    }
}