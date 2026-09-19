using UnityEngine;

/// <summary>
/// Keeps the authored 16:9 world framing fully visible at any screen aspect ratio.
/// The vertical axis is fixed (orthographic size stays 5 → 10 world units tall);
/// the horizontal mismatch is absorbed with letterbox/pillarbox bars via Camera.rect.
/// </summary>
public class CameraViewportFitter : MonoBehaviour
{
    /// <summary>Reference aspect: 1920×1080.</summary>
    const float ReferenceAspect = 16f / 9f;

    /// <summary>Dead band around scale == 1 to avoid jitter at exactly 16:9.</summary>
    const float AspectEpsilon = 0.001f;

    Camera _cam;
    float _lastAspect = -1f;

    void OnValidate() => Apply();
    void Awake() => Apply();
    void LateUpdate() => Apply();

    void Apply()
    {
        if (_cam == null)
            _cam = GetComponent<Camera>();
        if (_cam == null || Screen.height == 0)
            return;

        float aspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(aspect - _lastAspect) < 0.0001f)
            return; // aspect unchanged → rect already correct

        _lastAspect = aspect;
        float scale = aspect / ReferenceAspect;

        if (scale < 1f - AspectEpsilon)
        {
            // Narrower than 16:9 (e.g. 4:3) → letterbox bars top/bottom, centered
            _cam.rect = new Rect(0f, (1f - scale) * 0.5f, 1f, scale);
        }
        else if (scale > 1f + AspectEpsilon)
        {
            // Wider than 16:9 (e.g. 21:9) → pillarbox bars left/right
            _cam.rect = new Rect((1f - 1f / scale) * 0.5f, 0f, 1f / scale, 1f);
        }
        else
        {
            // Exactly 16:9 → full viewport
            _cam.rect = new Rect(0f, 0f, 1f, 1f);
        }
    }
}