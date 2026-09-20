using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Credits scene controller: reveals the credits with a fade-in and returns to
/// the Menu on VOLVER AL MENÚ. The scene starts opaque black
/// (FadeController.Awake), so Start plays the intro reveal like the other
/// scenes.
/// </summary>
public class CreditsController : MonoBehaviour
{
    [Tooltip("Full-screen black overlay for the intro reveal.")]
    [SerializeField] FadeController fade;

    /// <summary>
    /// Intro reveal: black → transparent. Runs in Start so
    /// FadeController.Awake has already forced the overlay opaque first.
    /// </summary>
    void Start()
    {
        if (fade != null) fade.FadeIn();
    }

    /// <summary>VOLVER AL MENÚ: back to the boot scene (which starts a fresh run on JUGAR).</summary>
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}