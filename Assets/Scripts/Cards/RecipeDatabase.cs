using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global catalog of ALL level configs. RecipeResolver matches cooked cards
/// (1-3) against the FULL catalog: a recipe owned by the current level resolves
/// as a Winner, a recipe owned by ANOTHER level resolves as Presented (blue).
/// </summary>
[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Cards/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    [Tooltip("Full catalog of levels; resolver matches against ALL, not just the active level.")]
    public List<LevelConfig> levels;
}