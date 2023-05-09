using System;
using UnityEditor;
using UnityEngine;
using static IngredientData;

[CreateAssetMenu(fileName = "Ingredient Data", menuName = "ScriptableObjects/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public string Name;
    public bool CanChangeState;
    public GameObject IngredientPrefab;

    [HideInInspector] public IngredientSlice[] Slices;
    [HideInInspector] public int CookingTime;
    [HideInInspector] public int BurnTime;
    
    [Serializable]
    public class IngredientSlice
    {
        [SerializeField] public IngredientState[] States;
        [SerializeField] public Mesh[] Meshes;
    }

    public enum IngredientState
    {
        Raw,
        Cooked,
        Burnt
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IngredientData))]
public class IngredientDataEditor : Editor
{
    SerializedProperty _canChangeState, _slices, _cookingTime, _burnTime;

    private void OnEnable()
    {
        _canChangeState = serializedObject.FindProperty(nameof(IngredientData.CanChangeState));
        _slices = serializedObject.FindProperty(nameof(IngredientData.Slices));
        _cookingTime = serializedObject.FindProperty(nameof(IngredientData.CookingTime));
        _burnTime = serializedObject.FindProperty(nameof(IngredientData.BurnTime));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_canChangeState.boolValue)
        {
            EditorGUI.indentLevel++;
            serializedObject.Update();
            EditorGUILayout.PropertyField(_cookingTime);
            EditorGUILayout.PropertyField(_burnTime);
            EditorGUILayout.PropertyField(_slices);
            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }
    }
}

[CustomPropertyDrawer(typeof(IngredientSlice))]
public class IngredientSliceDrawer : PropertyDrawer
{
    const float PROPERTY_HEIGHT = 18;
    const float PROPERTY_SPACING = 2;
    const float TOTAL_PROPERTY_HEIGHT = PROPERTY_HEIGHT + PROPERTY_SPACING;

    readonly IngredientState[] _ingredientStates = (IngredientState[])Enum.GetValues(typeof(IngredientState));

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        TOTAL_PROPERTY_HEIGHT * _ingredientStates.Length + TOTAL_PROPERTY_HEIGHT;

    private void Setup(SerializedProperty property)
    {
        SerializedProperty meshesField = property.FindPropertyRelative(nameof(IngredientSlice.Meshes));
        SerializedProperty statesField = property.FindPropertyRelative(nameof(IngredientSlice.States));
        for (int i = 0; i < _ingredientStates.Length; i++)
        {
            meshesField.InsertArrayElementAtIndex(i);
            statesField.InsertArrayElementAtIndex(i);
            statesField.GetArrayElementAtIndex(i).enumValueIndex = i;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.FindPropertyRelative(nameof(IngredientSlice.Meshes)).arraySize < _ingredientStates.Length)
            Setup(property);

        EditorGUI.BeginProperty(position, label, property);

        var labelPosition = new Rect(position.x, position.y, position.width, PROPERTY_HEIGHT);
        var labelStyle = new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        labelStyle.normal.textColor = Color.white;
        EditorGUI.LabelField(labelPosition, label.text.Replace("Element", "Slice"), labelStyle);

        SerializedProperty meshesField = property.FindPropertyRelative(nameof(IngredientSlice.Meshes));

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