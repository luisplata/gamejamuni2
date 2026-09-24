using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Boot scene controller: reveals the menu with a fade-in, and starts a fresh
/// run on JUGAR — resets the run state (GameSession.ResetRun → México) and
/// loads the Map. The scene starts opaque black (FadeController.Awake), so
/// Start plays the intro reveal like the game scene does.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Tooltip("Full-screen black overlay for the intro reveal.")]
    [SerializeField] FadeController fade;

    /// <summary>
    /// Level the TUTORIAL button launches (La Llorona). Set BEFORE loading the
    /// Tutorial scene so HandController.Awake picks it up (design D1).
    /// </summary>
    [SerializeField] LevelConfig tutorialConfig;

    /// <summary>
    /// Intro reveal: black → transparent. Runs in Start so
    /// FadeController.Awake has already forced the overlay opaque first.
    /// </summary>
    void Start()
    {
        if (fade != null) fade.FadeIn();
    }

    /// <summary>
    /// JUGAR: reset the run to México and move to the Map.
    /// Wired to the button's onClick in the Menu scene.
    /// </summary>
    public void StartRun()
    {
        GameSession.ResetRun();
        SceneManager.LoadScene(Scenes.Map);
    }

    /// <summary>
    /// TUTORIAL: fresh run state, then carry the tutorial level (La Llorona)
    /// into the Tutorial scene BEFORE it loads — HandController.Awake overrides
    /// its serialized level with GameSession.tutorialLevel, so ordering here is
    /// what makes the tutorial teach the right enemy. JUGAR (StartRun) is
    /// untouched: ResetRun clears tutorialLevel before any real run.
    /// </summary>
    public void StartTutorial()
    {
        GameSession.ResetRun();
        GameSession.tutorialLevel = tutorialConfig;
        SceneManager.LoadScene(Scenes.Tutorial);
    }
}