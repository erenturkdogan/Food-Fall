using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Sprite klasöründeki görselleri ismine göre IngredientData asset'lerine bağlar.
/// Sprite dosya adı = IngredientData adı olmalı (case-insensitive).
/// </summary>
public class AutoBindSprites
{
    [MenuItem("Tools/Auto Bind Sprites To Ingredients")]
    public static void Bind()
    {
        // Tüm IngredientData'ları bul
        string[] ingGuids = AssetDatabase.FindAssets("t:IngredientData");
        // Tüm sprite'ları bul
        string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite");

        int bound = 0;
        foreach (var guid in ingGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ing = AssetDatabase.LoadAssetAtPath<IngredientData>(path);
            if (ing == null) continue;

            string targetName = ing.name.ToLower();
            Sprite match = null;

            foreach (var sg in spriteGuids)
            {
                string sp = AssetDatabase.GUIDToAssetPath(sg);
                string fname = Path.GetFileNameWithoutExtension(sp).ToLower();
                if (fname == targetName)
                {
                    match = AssetDatabase.LoadAssetAtPath<Sprite>(sp);
                    break;
                }
            }

            if (match != null)
            {
                ing.sprite = match;
                EditorUtility.SetDirty(ing);
                bound++;
                Debug.Log($"✓ {ing.name} ← {match.name}");
            }
            else
            {
                Debug.LogWarning($"✗ {ing.name} için sprite bulunamadı (dosya adı '{ing.name}.png' olmalı)");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Toplam {bound} sprite bağlandı.");
    }
}
