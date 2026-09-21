using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Per-legend market catalog: the buyable items offered in the Market scene
/// before a run. One MarketItem sells `quantity` copies of a CardData for
/// `price` coins. Selected per level via LevelConfig.marketConfig (null ⇒
/// Listo-only market). Catalogs must only reference in-deck cards.
/// </summary>
[CreateAssetMenu(fileName = "MarketConfig", menuName = "Cards/Market Config")]
public class MarketConfig : ScriptableObject
{
    /// <summary>One buyable catalog entry: card + copies per purchase + coin price.</summary>
    [System.Serializable]
    public class MarketItem
    {
        [Tooltip("The card sold by this item (in-deck copies only).")]
        public CardData card;

        [Tooltip("Number of card copies granted per purchase.")]
        public int quantity = 1;

        [Tooltip("Coin cost per purchase.")]
        public int price = 10;
    }

    [Tooltip("Buyable items in this catalog.")]
    public List<MarketItem> items;
}