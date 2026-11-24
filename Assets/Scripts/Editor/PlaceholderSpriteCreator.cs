using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Unity Editor menüsünden renkli placeholder sprite'lar üretir.
/// Tools > Create Placeholder Sprites
/// </summary>
public class PlaceholderSpriteCreator : Editor
{
    private static readonly (string name, Color color)[] Ingredients = new[]
    {
        ("Tomato",    new Color(0.9f, 0.2f, 0.2f)),
        ("Lettuce",   new Color(0.3f, 0.8f, 0.3f)),
        ("Meat",      new Color(0.7f, 0.3f, 0.2f)),
        ("Cheese",    new Color(1.0f, 0.85f, 0.2f)),
        ("Mushroom",  new Color(0.8f, 0.7f, 0.6f)),
        ("Carrot",    new Color(1.0f, 0.55f, 0.1f)),
        ("Onion",     new Color(0.6f, 0.3f, 0.7f)),
        ("Poison",    new Color(0.4f, 0.1f, 0.8f)),
        ("Pot",       new Color(0.3f, 0.3f, 0.35f)),
    };

    [MenuItem("Tools/Create Placeholder Sprites")]
    public static void CreateAll()
    {
        string folder = "Assets/Sprites/Placeholders";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets/Sprites", "Placeholders");
        }

        foreach (var (name, color) in Ingredients)
        {
            string path = $"{folder}/{name}.png";
            if (File.Exists(Application.dataPath + path.Replace("Assets", "")))
            {
                Debug.Log($"Skipped (already exists): {path}");
                continue;
            }

            Texture2D tex = CreateCircleTexture(128, color);
            byte[] png = tex.EncodeToPNG();
            DestroyImmediate(tex);

            File.WriteAllBytes(Application.dataPath + "/" + path.Replace("Assets/", ""), png);
            Debug.Log($"Created: {path}");
        }

        AssetDatabase.Refresh();

        // Import ayarlarını Sprite olarak set et
        foreach (var (name, _) in Ingredients)
        {
            string path = $"Assets/Sprites/Placeholders/{name}.png";
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        Debug.Log("Placeholder sprites ready in Assets/Sprites/Placeholders/");
    }

    private static Texture2D CreateCircleTexture(int size, Color fillColor)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        float radius = size / 2f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius - 1f)
                    tex.SetPixel(x, y, fillColor);
                else if (dist <= radius)
                    tex.SetPixel(x, y, Color.Lerp(fillColor, transparent, dist - (radius - 1f))); // kenar yumuşatma
                else
                    tex.SetPixel(x, y, transparent);
            }
        }

        tex.Apply();
        return tex;
    }
}
