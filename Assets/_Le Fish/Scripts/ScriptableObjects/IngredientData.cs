using System;
using System.Linq;
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
        [SerializeField] public MeshData[] Meshes;

        [Serializable]
        public class MeshData
        {
            [SerializeField] public Mesh Mesh;
            [SerializeField] public Material[] Materials;
        }
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
    const float PROPERTY_INDENT = 15;
    const float PROPERTY_SPACING = 2;
    const float TOTAL_PROPERTY_HEIGHT = PROPERTY_HEIGHT + PROPERTY_SPACING;

    readonly IngredientState[] _ingredientStates = (IngredientState[])Enum.GetValues(typeof(IngredientState));

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var meshes = property.FindPropertyRelative(nameof(IngredientSlice.Meshes));
        int materialCount = 0;
        for (int i = 0; i < meshes.arraySize; i++)
        {
            materialCount += meshes.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(IngredientSlice.MeshData.Materials)).arraySize;
        }
        return TOTAL_PROPERTY_HEIGHT * (_ingredientStates.Length + materialCount) + TOTAL_PROPERTY_HEIGHT;
    }

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

        Vector2 ingredientPosition = position.position;
        for (int i = 0; i < _ingredientStates.Length; i++)
        {
            var element = meshesField.GetArrayElementAtIndex(i);
            var meshProperty = element.FindPropertyRelative(nameof(IngredientSlice.MeshData.Mesh));
            var materialsPropterty = element.FindPropertyRelative(nameof(IngredientSlice.MeshData.Materials));

            ingredientPosition.y += TOTAL_PROPERTY_HEIGHT;

            meshProperty.objectReferenceValue = EditorGUI.ObjectField(
                position: new Rect(ingredientPosition, new Vector2(position.width, PROPERTY_HEIGHT)),
                label: new GUIContent(_ingredientStates[i].ToString()),
                obj: meshProperty.objectReferenceValue,
                objType: typeof(Mesh),
                allowSceneObjects: false);

            if (meshProperty.objectReferenceValue == null)
            {
                materialsPropterty.arraySize = 0;
            }
            else
            {
                materialsPropterty.arraySize = ((Mesh)meshProperty.objectReferenceValue).subMeshCount;

                for (int j = 0; j < materialsPropterty.arraySize; j++)
                {
                    var material = materialsPropterty.GetArrayElementAtIndex(j);
                    ingredientPosition.y += TOTAL_PROPERTY_HEIGHT;
                    material.objectReferenceValue = EditorGUI.ObjectField(
                        position: new Rect(new Vector2(ingredientPosition.x + PROPERTY_INDENT, ingredientPosition.y), new Vector2(position.width - PROPERTY_INDENT, PROPERTY_HEIGHT)),
                        label: new GUIContent(j.ToString()),
                        obj: material.objectReferenceValue,
                        objType: typeof(Material),
                        allowSceneObjects: false);
                }
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif