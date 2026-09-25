using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Legend scene controller: shows the current level's lore (legendTitle +
/// legendText read from RecipeDatabase.levels[GameSession.currentLevelIndex])
/// and forwards to the Market on Continuar. An EMPTY legend auto-forwards to
/// the Market immediately (retrocompatible no-op for old LevelConfigs with
/// null/empty fields). Narration audio is null-safe.
/// </summary>
public class LegendController : MonoBehaviour
{
    [Tooltip("Global level catalog; legend fields read from levels[currentLevelIndex].")]
    [SerializeField] RecipeDatabase database;

    [Tooltip("Legend title label (TMP).")]
    [SerializeField] TMP_Text titleText;

    [Tooltip("Legend body label (TMP).")]
    [SerializeField] TMP_Text bodyText;

    [Tooltip("Full-screen black overlay; the legend reveals behind the fade-in.")]
    [SerializeField] FadeController fade;

    /// <summary>
    /// Scene loaded by CONTINUAR (and by the empty-legend auto-forward). The
    /// real Legend routes to the Market; the tutorial copy (TutorialLegend)
    /// overrides this to TutorialMarket so the guided chain keeps moving.
    /// </summary>
    [SerializeField] string nextScene = Scenes.Market;

    /// <summary>
    /// Reads the current level's legend. Empty legendText → immediately loads
    /// nextScene (no legend UI shown, no null-ref). Otherwise fills the TMP
    /// labels, plays narration if present (null-safe), and fades in.
    /// </summary>
    void Start()
    {
        var level = CurrentLevel();
        if (level == null || string.IsNullOrEmpty(level.legendText))
        {
            SceneManager.LoadScene(nextScene);
            return;
        }

        if (titleText != null) titleText.text = level.legendTitle;
        if (bodyText != null) bodyText.text = level.legendText;
        if (level.audioNarration != null)
            AudioSource.PlayClipAtPoint(level.audioNarration, Vector3.zero);
        if (fade != null) fade.FadeIn();
    }

    /// <summary>CONTINUAR: proceed to nextScene (the Market for this level).</summary>
    public void Continue()
    {
        SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Current level's config, bounds-guarded (null when unwired/out of range).
    /// Tutorial override (design D1, same as HandController.Awake): a live
    /// GameSession.tutorialLevel beats the database pick, so the tutorial chain
    /// (Menu → TutorialLegend → TutorialMarket → Tutorial) shows La Llorona's
    /// legend instead of the RecipeDatabase level. ResetRun clears it before
    /// any real run, so the real flow is untouched.
    /// </summary>
    LevelConfig CurrentLevel()
    {
        if (GameSession.tutorialLevel != null) return GameSession.tutorialLevel;
        if (database == null || database.levels == null) return null;
        int index = GameSession.currentLevelIndex;
        if (index < 0 || index >= database.levels.Count) return null;
        return database.levels[index];
    }
}