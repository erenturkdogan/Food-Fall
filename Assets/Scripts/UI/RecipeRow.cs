using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tarif panelindeki tek bir satır.
/// Prefab: Image (malzeme), TextMeshProUGUI (count)
/// </summary>
public class RecipeRow : MonoBehaviour
{
    public Image ingredientImage;
    public TextMeshProUGUI countText;

    private int _required;

    public void Init(IngredientData data, int current, int required)
    {
        _required = required;
        ingredientImage.sprite = data.sprite;
        SetCount(current, required);
    }

    public void SetCount(int current, int required)
    {
        countText.text = $"{current}/{required}";
        // Tamamlandıysa yeşil, değilse kırmızı — tasarım eklenince renkler ayarlanır
        countText.color = current >= required ? Color.green : Color.red;
    }
}
