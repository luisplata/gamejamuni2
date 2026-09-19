using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minimal static result view for a resolved dish: a background Image tinted
/// by the dish kind color, a TMP dish name, and a TMP icon. Deliberately NOT
/// draggable and free of CardView's drag/move state machine — the cooking
/// result is display-only.
/// </summary>
public class DishView : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text iconText;

    /// <summary>Applies the resolved dish's identity and kind color.</summary>
    public void SetDish(string dishName, string icon, Color color)
    {
        if (image != null) image.color = color;
        if (nameText != null) nameText.text = dishName;
        if (iconText != null) iconText.text = icon;
    }
}