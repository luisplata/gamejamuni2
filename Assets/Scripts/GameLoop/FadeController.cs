using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full-screen black overlay controller for the intro reveal and the round-end
/// cover. FadeIn goes 1→0 (black to transparent, revealing the scenario),
/// FadeOut goes 0→1 (cover to black), FadeTo targets any alpha. Input is
/// blocked during any fade AND while the overlay is opaque by toggling the
/// overlay Image's raycastTarget (true when alpha > 0, false only at fully
/// transparent). The scene starts opaque black (Awake forces alpha=1).
/// </summary>
public class FadeController : MonoBehaviour
{
    [Tooltip("Full-screen black Image, LAST sibling of the Canvas (renders above everything).")]
    [SerializeField] Image fadeImage;

    [Tooltip("Seconds for FadeIn/FadeOut when no explicit duration is given.")]
    [SerializeField] float duration = 1f;

    /// <summary>Currently running fade coroutine (stopped before starting a new one).</summary>
    Coroutine _fade;

    /// <summary>Force opaque black + input blocking at scene start (intro cover).</summary>
    void Awake()
    {
        SetAlpha(1f);
    }

    /// <summary>Reveal: black → transparent over the default duration.</summary>
    public void FadeIn(Action onComplete = null)
    {
        FadeTo(0f, duration, onComplete);
    }

    /// <summary>Cover: transparent → black over the default duration.</summary>
    public void FadeOut(Action onComplete = null)
    {
        FadeTo(1f, duration, onComplete);
    }

    /// <summary>
    /// Lerp the overlay alpha to <paramref name="targetAlpha"/> over
    /// <paramref name="durationSeconds"/> seconds, then invoke
    /// <paramref name="onComplete"/>. Stops any running fade first. When the
    /// overlay is unwired the callback fires immediately so game flow never
    /// deadlocks on a missing reference.
    /// </summary>
    public void FadeTo(float targetAlpha, float durationSeconds, Action onComplete = null)
    {
        if (fadeImage == null)
        {
            onComplete?.Invoke();
            return;
        }
        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(FadeRoutine(targetAlpha, Mathf.Max(0f, durationSeconds), onComplete));
    }

    IEnumerator FadeRoutine(float targetAlpha, float durationSeconds, Action onComplete)
    {
        float start = fadeImage.color.a;
        float t = 0f;
        while (t < durationSeconds)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, targetAlpha, durationSeconds <= 0f ? 1f : t / durationSeconds));
            yield return null;
        }
        SetAlpha(targetAlpha);
        _fade = null;
        onComplete?.Invoke();
    }

    /// <summary>Applies alpha and mirrors input blocking: raycastTarget on while visible.</summary>
    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
        fadeImage.raycastTarget = alpha > 0f;
    }
}