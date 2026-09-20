using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Owns the round rules: player lives, enemy patience (the enemy identity is
/// the current LevelConfig), win/lose, and the fade + auto-reset flow. Serves
/// as the bridge between the UI (PREPARAR button) and HandController.
/// The game starts AUTOMATICALLY on scene load (Start → StartGame): the intro
/// fade reveal + deal happens without any button press.
///
/// Mapping (user-validated, single switch in ServeFood):
///   Green (Normal) / Gold (Star)  → WIN        → fade + full reset
///   Blue (Presented) / Purple (Cursed) / Gray (Filler) → patience −1 (benign retry)
///   Red (Fail "Comida Cruda")     → BITE       → life −1, patience refills
///   Patience 0 → BITE (life −1, patience refills); lives 0 → GAME OVER → fade + full reset.
///
/// Animation wiring (all null-guarded — unwired views no-op): PlayerView/
/// EnemyView receive per-outcome triggers; win/game-over reactions fire in
/// ServeFood/Bite BEFORE the round-end fade (DelayedFade) so they stay visible
/// for ReactionShowDelay seconds. ResetRound hard-returns both views to Idle.
///
/// All runtime state lives HERE on the MonoBehaviour — never in the config SO.
/// </summary>
public class GameLoopController : MonoBehaviour
{
    [Tooltip("Static tuning: maxLives / maxPatience / fadeDuration.")]
    [SerializeField] GameLoopConfig config;

    [Tooltip("Owns the cook/trash/deal mechanics; PrepareFood returns the resolution.")]
    [SerializeField] HandController handController;

    [Tooltip("Full-screen black overlay for the intro reveal and round-end cover.")]
    [SerializeField] FadeController fadeController;

    [Tooltip("HUD label showing lives (e.g. 'VIDAS: 3').")]
    [SerializeField] TMP_Text livesText;

    [Tooltip("HUD label showing enemy patience (e.g. 'PACIENCIA: 3').")]
    [SerializeField] TMP_Text patienceText;

    [Tooltip("World-space player view (idle + one-shot anims). Null = no-op.")]
    [SerializeField] PlayerView playerView;

    [Tooltip("World-space enemy view (idle + reactions + phrase bubble). Null = no-op.")]
    [SerializeField] EnemyView enemyView;

    /// <summary>
    /// How long win/game-over reactions stay visible before the round-end fade
    /// covers them (animations fire first, fade is delayed by this).
    /// </summary>
    const float ReactionShowDelay = 0.5f;

    int _lives;
    int _patience;
    bool _roundActive;

    /// <summary>The enemy identity: the current level's config (levelName + recipes = gustos).</summary>
    public LevelConfig Enemy => handController != null ? handController.CurrentLevel : null;

    /// <summary>
    /// Auto-start: the game begins by itself on scene load — the intro fade
    /// reveal + deal happens without any button. Runs in Start (not Awake) so
    /// FadeController.Awake has already forced the overlay opaque black first.
    /// </summary>
    void Start()
    {
        StartGame();
    }

    /// <summary>
    /// Seed the round state, refresh the HUD, then fade the black overlay out
    /// (reveal) and deal at the fade end.
    /// </summary>
    public void StartGame()
    {
        _roundActive = true;
        _lives = MaxLives();
        _patience = MaxPatience();
        UpdateHUD();
        FadeIn(() =>
        {
            if (handController != null) handController.StartDeal();
        });
    }

    /// <summary>
    /// PREPARAR: resolve the cook and apply the result mapping. No-ops (no
    /// punishment) when the round is inactive (fading / between rounds) or the
    /// cook queue is empty (PrepareFood returns null — nothing was cooked).
    /// </summary>
    public void ServeFood()
    {
        if (!_roundActive || handController == null) return;
        ResolutionResult? result = handController.PrepareFood();
        if (result == null) return;

        switch (result.Value.Kind)
        {
            // Green (Normal) / Gold (Star): the enemy is satisfied → round won.
            case RecipeKind.Normal:
            case RecipeKind.Star:
                // Dish + player + enemy reactions fire BEFORE the fade (D6):
                // the win is visible for ReactionShowDelay, then covered.
                handController?.LiveDish?.PlayReaction(result.Value.Kind);
                playerView?.OnWin();
                enemyView?.PlayReaction(EnemyReaction.Win, result.Value.Reaction);
                WinRound();
                break;

            // Blue (Presented) / Purple (Cursed) / Gray (Filler): benign retry —
            // patience drops; at 0 the enemy bites.
            case RecipeKind.Presented:
            case RecipeKind.Cursed:
            case RecipeKind.Filler:
                handController?.LiveDish?.PlayReaction(result.Value.Kind);
                playerView?.OnLosePatience();
                enemyView?.PlayReaction(EnemyReaction.Patience);
                _patience--;
                UpdateHUD();
                if (_patience <= 0) Bite();
                break;

            // Red (Fail, "Comida Cruda"): the only grave insult → bite.
            case RecipeKind.Fail:
                handController?.LiveDish?.PlayReaction(result.Value.Kind);
                playerView?.OnLoseLife();
                enemyView?.PlayReaction(EnemyReaction.Bite);
                Bite();
                break;
        }
    }

    /// <summary>Round won: stop input, cover to black, then full reset + reveal.</summary>
    void WinRound()
    {
        _roundActive = false;
        StartCoroutine(DelayedFade(ReactionShowDelay, ResetRound));
    }

    /// <summary>
    /// Enemy bite: −1 life, patience refills to max, HUD updated. At 0 lives →
    /// GAME OVER: the eat reaction fires BEFORE the fade, stop input, cover to
    /// black (delayed), then full reset + reveal.
    /// </summary>
    void Bite()
    {
        _lives--;
        _patience = MaxPatience();
        UpdateHUD();
        if (_lives <= 0)
        {
            _roundActive = false;
            playerView?.OnGameOver();
            enemyView?.PlayReaction(EnemyReaction.Eaten);
            StartCoroutine(DelayedFade(ReactionShowDelay, ResetRound));
        }
    }

    /// <summary>
    /// Full round reset (runs behind the black cover): restore lives/patience,
    /// hard-return both character views to Idle, deal a fresh round (StartDeal
    /// resets zones, refills, cursor, dish and deck), refresh the HUD, then
    /// reveal the fresh round. No START return.
    /// </summary>
    void ResetRound()
    {
        _roundActive = true;
        _lives = MaxLives();
        _patience = MaxPatience();
        playerView?.ResetToIdle();
        enemyView?.ResetToIdle();
        if (handController != null) handController.StartDeal();
        UpdateHUD();
        FadeIn();
    }

    /// <summary>
    /// Lets a round-end reaction play for <paramref name="delay"/> seconds
    /// BEFORE the fade cover starts (win/game-over visibility, D6).
    /// </summary>
    IEnumerator DelayedFade(float delay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        FadeOut(onComplete);
    }

    /// <summary>Refreshes the VIDAS / PACIENCIA HUD labels (no-op when unwired).</summary>
    void UpdateHUD()
    {
        if (livesText != null) livesText.text = $"VIDAS: {_lives}";
        if (patienceText != null) patienceText.text = $"PACIENCIA: {_patience}";
    }

    void FadeIn(Action onComplete = null)
    {
        if (fadeController != null) fadeController.FadeTo(0f, GetFadeDuration(), onComplete);
        else onComplete?.Invoke();
    }

    void FadeOut(Action onComplete = null)
    {
        if (fadeController != null) fadeController.FadeTo(1f, GetFadeDuration(), onComplete);
        else onComplete?.Invoke();
    }

    int MaxLives() => config != null ? config.maxLives : 3;
    int MaxPatience() => config != null ? config.maxPatience : 3;
    float GetFadeDuration() => config != null ? config.fadeDuration : 1f;
}