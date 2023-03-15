using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Ingredient : MonoBehaviour
{
    public IngredientState CurrentState { get; private set; }
    public int CurrentSlice { get; private set; }

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] List<IngredientData> ingredients = new();
    [SerializeField] public int cookingTime;
    [SerializeField] public int burnTime;
    public bool IsCooking = false;

    [Serializable]
    public class IngredientData
    {
        [SerializeField] public IngredientState[] states;
        [SerializeField] public Mesh[] meshes;
    }

    public enum IngredientState
    {
        Raw,
        Cooked,
        Burnt
    }

    public void NextState(IngredientState state)
    {
        CurrentState = state;
        meshFilter.mesh = ingredients[CurrentSlice].meshes[(int)state];
    }

    public void Slice()
    {
        if (CurrentSlice < ingredients.Count - 1)
        {
            CurrentSlice++;
            meshFilter.mesh = ingredients[CurrentSlice].meshes[(int)CurrentState];
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Ingredient.IngredientData))]
public class IngredientDataDrawer : PropertyDrawer
{
    const float PROPERTY_HEIGHT = 18;
    const float PROPERTY_SPACING = 2;
    const float TOTAL_PROPERTY_HEIGHT = PROPERTY_HEIGHT + PROPERTY_SPACING;

    readonly Ingredient.IngredientState[] _ingredientStates = (Ingredient.IngredientState[])Enum.GetValues(typeof(Ingredient.IngredientState));

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        TOTAL_PROPERTY_HEIGHT * _ingredientStates.Length + TOTAL_PROPERTY_HEIGHT;

    private void Setup(SerializedProperty property)
    {
        SerializedProperty meshesField = property.FindPropertyRelative(nameof(Ingredient.IngredientData.meshes));
        SerializedProperty statesField = property.FindPropertyRelative(nameof(Ingredient.IngredientData.states));
        for (int i = 0; i < _ingredientStates.Length; i++)
        {
            meshesField.InsertArrayElementAtIndex(i);
            statesField.InsertArrayElementAtIndex(i);
            statesField.GetArrayElementAtIndex(i).enumValueIndex = i;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.FindPropertyRelative(nameof(Ingredient.IngredientData.meshes)).arraySize < _ingredientStates.Length)
            Setup(property);

        EditorGUI.BeginProperty(position, label, property);

        var labelPosition = new Rect(position.x, position.y, position.width, PROPERTY_HEIGHT);
        var labelStyle = new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        labelStyle.normal.textColor = Color.white;
        EditorGUI.LabelField(labelPosition, label.text.Replace("Element", "Slice"), labelStyle);

        SerializedProperty meshesField = property.FindPropertyRelative(nameof(Ingredient.IngredientData.meshes));

        for (int i = 0; i < _ingredientStates.Length; i++)
        {
            var ingredientPosition = new Rect(position.x, position.y + (1 + i) * TOTAL_PROPERTY_HEIGHT, position.width, PROPERTY_HEIGHT);
            meshesField.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUI.ObjectField(
                position: ingredientPosition,
                label: new GUIContent(_ingredientStates[i].ToString()),
                obj: meshesField.GetArrayElementAtIndex(i).objectReferenceValue,
                objType: typeof(Mesh),
                allowSceneObjects: false);
        }

        EditorGUI.EndProperty();
    }
}
#endif