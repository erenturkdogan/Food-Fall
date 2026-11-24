using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Sprite'ı eksik ya da placeholder olan IngredientData'ları listeler.
/// Tools > Find Bad Ingredients
/// </summary>
public class FindBadIngredients
{
    [MenuItem("Tools/Find Bad Ingredients")]
    public static void Find()
    {
        string[] guids = AssetDatabase.FindAssets("t:IngredientData");
        int problems = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ing = AssetDatabase.LoadAssetAtPath<IngredientData>(path);
            if (ing == null) continue;

            if (ing.sprite == null)
            {
                Debug.LogError($"<color=red>SPRITE YOK:</color> {ing.name}  ({path})", ing);
                problems++;
                continue;
            }

            string spritePath = AssetDatabase.GetAssetPath(ing.sprite);
            // Placeholder klasöründen veya placeholder isimli mi?
            if (spritePath.ToLower().Contains("placeholder"))
            {
                Debug.LogWarning($"<color=orange>PLACEHOLDER kullanıyor:</color> {ing.name} → {Path.GetFileName(spritePath)}", ing);
                problems++;
            }
            else
            {
                Debug.Log($"✓ {ing.name} → {ing.sprite.name}", ing);
            }
        }

        if (problems == 0)
            Debug.Log("<color=green>Tüm ingredient'ların sprite'ı düzgün!</color>");
        else
            Debug.LogWarning($"Toplam {problems} sorunlu ingredient bulundu. Yukarıdaki satırlara tıkla, asset seçilir.");
    }
}
