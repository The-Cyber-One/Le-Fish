using System.IO;
using UnityEditor;
using UnityEngine;

public class GradientDrawer : MaterialPropertyDrawer
{
    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        EditorGUILayout.LabelField(label);
        position.x += position.width - EditorGUIUtility.labelWidth;
        position.width = EditorGUIUtility.labelWidth;

        Texture2D texture = (Texture2D)prop.textureValue;

        foreach (Object target in prop.targets)
            if (!File.Exists(GetGradientPath(target, label)))
                SaveGradientTexture(CreateTexture(), target, label);

        if (prop.hasMixedValue)
        {
            EditorGUI.LabelField(position, "___Can't edit multiple different gradients___");
            return;
        }

        if (texture == null)
        {
            texture = LoadTexture(editor.target, label);
            prop.textureValue = texture;
        }

        Gradient gradient = new();
        Color[] texturePixels = texture.GetPixels();
        int textureWidth = texture.width;
        GradientColorKey[] colorKeys = new GradientColorKey[textureWidth];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[textureWidth];
        for (int i = 0; i < textureWidth; i++)
        {
            colorKeys[i] = new GradientColorKey(texturePixels[i], texturePixels[i + textureWidth].r);
            alphaKeys[i] = new GradientAlphaKey(texturePixels[i].a, texturePixels[i + textureWidth].r);
        }
        gradient.SetKeys(colorKeys, alphaKeys);
        gradient.mode = (GradientMode)texturePixels[textureWidth].g;

        EditorGUI.BeginChangeCheck();
        gradient = EditorGUILayout.GradientField(gradient);
        texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false);

        if (EditorGUI.EndChangeCheck())
        {
            int gradientWidth = gradient.colorKeys.Length;
            Color32[] gradientPixels = new Color32[gradientWidth * 2];
            for (int i = 0; i < gradientWidth; i++)
            {
                Color color = gradient.colorKeys[i].color;
                float time = gradient.colorKeys[i].time;
                color.a = gradient.Evaluate(time).a;
                gradientPixels[i] = color;
                gradientPixels[i + gradientWidth] = new Color(time, i == 0 ? (int)gradient.mode : 0, 0, 1);
            }

            texture = new(gradientWidth, 2)
            {
                filterMode = FilterMode.Point,
                alphaIsTransparency = true
            };
            texture.SetPixels32(gradientPixels);
            texture.Apply();
            prop.textureValue = texture;
            //SaveGradientTexture(texture, editor.target, label);
        }

        //prop.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(GetGradientPath(editor.target, label));

        //prop.applyPropertyCallback = () => 
        if (GUILayout.Button("Save"))
        {
            SaveGradientTexture(texture, editor.target, label);
            prop.textureValue = LoadTexture(editor.target, label);
        }
    }

    private string GetGradientPath(Object target, string label) =>
        $"{Path.ChangeExtension(AssetDatabase.GetAssetPath(target), null)} {label}.png";

    private Texture2D CreateTexture()
    {
        Texture2D texture = new(2, 2)
        {
            filterMode = FilterMode.Point,
            alphaIsTransparency = true
        };
        texture.SetPixels32(new Color32[] { Color.white, Color.white, Color.black, Color.red });
        texture.Apply();
        return texture;
    }

    private Texture2D LoadTexture(Object target, string label) =>
        AssetDatabase.LoadAssetAtPath<Texture2D>(GetGradientPath(target, label));

    private void SaveGradientTexture(Texture2D texture, Object target, string label)
    {
        string texturePath = GetGradientPath(target, label);
        File.WriteAllBytes(texturePath, texture.EncodeToPNG());

        AssetDatabase.ImportAsset(texturePath);
        var tImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        tImporter.isReadable = true;
        tImporter.alphaIsTransparency = true;
        tImporter.filterMode = FilterMode.Point;
        tImporter.SaveAndReimport();
        AssetDatabase.Refresh();
    }
}
