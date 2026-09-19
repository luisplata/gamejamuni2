using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The 30-card deck as parametrizable entries (card type + copy count).
/// HandController shuffles the EXPANDED deck once per StartDeal into a
/// materialized draw order and deals from it with a wrap-around index.
/// Exactly 30 copies per spec: 8 Base / 11 Complemento / 11 Sazon.
/// </summary>
[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/Deck Data")]
public class DeckData : ScriptableObject
{
    /// <summary>One card type and how many copies it contributes to the deck.</summary>
    [System.Serializable]
    public class DeckEntry
    {
        public CardData card;
        public int count;
    }

    [Tooltip("Parametrizable deck entries. Expand() materializes each card count times.")]
    public List<DeckEntry> entries;

    [Tooltip("Negative = unseeded shuffle. >=0 = deterministic shuffle (debug).")]
    [SerializeField] int randomSeed = -1;

    /// <summary>Total card copies (sum of entry counts, null-safe).</summary>
    public int Count
    {
        get
        {
            if (entries == null) return 0;
            int total = 0;
            foreach (var e in entries)
                if (e != null && e.card != null) total += e.count;
            return total;
        }
    }

    /// <summary>Seeded-shuffle switch for HandController (negative = random).</summary>
    public int RandomSeed => randomSeed;

    /// <summary>Materializes the deck: each entry's card repeated count times.</summary>
    public List<CardData> Expand()
    {
        var list = new List<CardData>();
        if (entries == null) return list;
        foreach (var e in entries)
        {
            if (e == null || e.card == null || e.count <= 0) continue;
            for (int i = 0; i < e.count; i++) list.Add(e.card);
        }
        return list;
    }

    void OnValidate()
    {
        if (entries == null) return;

        if (Count != 30)
            Debug.LogWarning($"DeckData '{name}': total is {Count} cards (spec requires exactly 30).", this);

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e == null)
                Debug.LogWarning($"DeckData '{name}': null entry at index {i}.", this);
            else if (e.card == null)
                Debug.LogWarning($"DeckData '{name}': entry {i} has no card assigned.", this);
            else if (e.count <= 0)
                Debug.LogWarning($"DeckData '{name}': entry {i} ('{e.card.name}') has count {e.count} (must be > 0).", this);
        }
    }
}