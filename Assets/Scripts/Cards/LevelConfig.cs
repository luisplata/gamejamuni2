using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Per-level cooking configuration: display identity, the ingredient that must
/// be present to avoid AutoFail, and the exact recipes (Star + Normal only)
/// that resolve in this level. Loaded dynamically by HandController so the
/// same scene serves both levels.
/// </summary>
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Cards/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Tooltip("Level display name (e.g. 'El Ahuizotl').")]
    public string levelName;

    [Tooltip("Region display name (e.g. 'México').")]
    public string regionName;

    [Tooltip("Ingredient that must be present in a cook to avoid AutoFail.")]
    public CardData requiredIngredient;

    [Tooltip("Exact recipes resolvable in this level (Star + Normal only).")]
    public List<RecipeData> recipes;
}