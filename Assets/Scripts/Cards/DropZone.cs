using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A drop target for cards: holds its kind (Center = Cook, Corner = Trash)
/// plus the rect geometry needed to test whether a screen point lands inside
/// it. The dragged card sits under the pointer and would block zone raycasts,
/// so detection is a pure position check against the zone rect (no raycast).
/// A zone also HOLDS parked cards as a visible queue: Accept parks a card
/// (locked/capacity-checked against the config by kind), ReleaseAll detaches them
/// for the consume flow, and GetParkedSlot lays out slots horizontally.
/// </summary>
public class DropZone : MonoBehaviour
{
    /// <summary>Center = cook queue; Corner = trash queue (semantic only; names kept for minimal churn).</summary>
    public enum ZoneKind { Center, Corner }

    [SerializeField] CardVisualConfig config;
    [SerializeField] ZoneKind kind;

    /// <summary>
    /// Tutorial gating: when true, Accept() rejects every park so no card can
    /// land here (e.g. the cook zone stays closed until the tutorial teaches
    /// it). Serialized default false — Prototype and normal play are untouched.
    /// </summary>
    [SerializeField] bool locked;

    readonly List<CardView> held = new();

    /// <summary>Which queue this zone is (Center = cook, Corner = trash).</summary>
    public ZoneKind Kind => kind;

    /// <summary>
    /// Locks/unlocks this zone. While locked, Accept() rejects all parks (the
    /// dropped card returns to the hand). Default false (unlocked).
    /// </summary>
    public bool Locked { get => locked; set => locked = value; }

    /// <summary>This zone's RectTransform (the hit-test geometry).</summary>
    public RectTransform Rect => (RectTransform)transform;

    /// <summary>Number of cards currently parked in this zone.</summary>
    public int Count => held.Count;

    /// <summary>
    /// Read-only view of the cards currently parked in this zone. Used by the
    /// cooking flow to gather the cooked CardData before the zone is consumed.
    /// </summary>
    public IReadOnlyList<CardView> Held => held;

    /// <summary>Capacity of this zone, derived from config by kind.</summary>
    int MaxCount => kind == ZoneKind.Center ? config.maxCook : config.maxTrashPerGame;

    /// <summary>
    /// True if <paramref name="screenPoint"/> (screen px) is inside this
    /// zone's rect. null camera = Screen Space - Overlay canvas.
    /// </summary>
    public bool Contains(Vector2 screenPoint)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screenPoint, null, out var local)
               && Rect.rect.Contains(local);
    }

    /// <summary>
    /// Parks <paramref name="card"/> in this zone. False (no change) when the
    /// zone is locked or already at capacity — the caller rejects the drop.
    /// </summary>
    public bool Accept(CardView card)
    {
        if (locked || held.Count >= MaxCount) return false;
        held.Add(card);
        return true;
    }

    /// <summary>
    /// Removes <paramref name="card"/> from the held list. Used by the consume
    /// tail as defense-in-depth (the normal detach path is ReleaseAll).
    /// </summary>
    public void Release(CardView card)
    {
        held.Remove(card);
    }

    /// <summary>
    /// Detaches and clears all parked cards, returning them so the caller can
    /// drive the consume animation / destroy them.
    /// </summary>
    public List<CardView> ReleaseAll()
    {
        var all = new List<CardView>(held);
        held.Clear();
        return all;
    }

    /// <summary>
    /// Zone-local anchored position of the <paramref name="index"/>-th parked
    /// slot: horizontally centered across MaxCount slots (so a half-full queue
    /// sits centered in the zone) and spaced by <c>parkedSpacing</c>.
    /// </summary>
    public Vector2 GetParkedSlot(int index)
    {
        float x = (index - (MaxCount - 1) / 2f) * config.parkedSpacing;
        return new Vector2(x, 0f);
    }

    void Start()
    {
        GetComponent<Image>().color = kind == ZoneKind.Center ? config.centerZoneColor : config.cornerZoneColor;
    }
}