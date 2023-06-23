using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static IngredientData;
using static RecipeData;

[Serializable, RequireComponent(typeof(Timer))]
public class Ingredient : MonoBehaviour
{
    public IngredientState CurrentState { get => _currentState; private set => _currentState = value; }
    public int CurrentSlice { get => _currentSlice; private set => _currentSlice = value; }
    public bool IsCooking = false;
    public IngredientData Data;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject ashPrefab;
    [SerializeField] ParticleSystem starParticlePrefab;

    Timer _timer;
    [SerializeField, HideInInspector] private IngredientState _currentState;
    [SerializeField, HideInInspector] private int _currentSlice;

    public Timer Timer => _timer = _timer != null ? _timer : GetComponent<Timer>();

    public static implicit operator DataStateSlice(Ingredient ingredient) => new()
    {
        Ingredient = ingredient.Data,
        CookingState = ingredient.CurrentState,
        SliceState = ingredient.CurrentSlice
    };

    public void SetState(IngredientState state)
    {
        CurrentState = state;
        if (CurrentState == IngredientState.Burnt)
        {
            Instantiate(ashPrefab, transform.position, ashPrefab.transform.rotation);
            Destroy(gameObject);
        }
        else
            UpdateMesh();
    }

    [ContextMenu(nameof(Bake))]
    public void Bake()
    {
        if ((int)CurrentState < Data.Slices[CurrentSlice].States.Length - 1)
        {
            bool hasNextState = Data.Slices[CurrentSlice].Meshes[(int)CurrentState + 1] != null;
            SetState(hasNextState ? ++CurrentState : IngredientState.Burnt);
        }
    }

    private void SliceOrSpice(bool isSpice)
    {
        if (CurrentSlice < Data.Slices.Length - 1 &&
            Data.Slices[CurrentSlice + 1].IsSpice == isSpice &&
            Data.Slices[CurrentSlice + 1].Meshes[(int)CurrentState] != null)
        {
            CurrentSlice++;
            UpdateMesh();
        }
    }

    [ContextMenu(nameof(Slice))]
    public void Slice()
    {
        SliceOrSpice(false);

        if (CurrentSlice == Data.Slices.Length - 1)
            Instantiate(starParticlePrefab, transform);
    }

    [ContextMenu(nameof(Spice))]
    public void Spice() => SliceOrSpice(true);

    private void UpdateMesh()
    {
        var data = Data.Slices[CurrentSlice].Meshes[(int)CurrentState];
        meshFilter.mesh = data.Mesh;
        meshRenderer.materials = data.Materials;
    }

    [ContextMenu(nameof(Finish))]
    private void Finish()
    {
        for (int i = CurrentSlice; i < Data.Slices.Length; i++)
            Slice();
        if (CurrentState == IngredientState.Raw)
            Bake();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Ingredient))]
public class IngredientEditor : Editor
{
    SerializedProperty _currentSlice, _currentState;

    private void OnEnable()
    {
        _currentSlice = serializedObject.FindProperty("_currentSlice");
        _currentState = serializedObject.FindProperty("_currentState");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox($"Slice: {_currentSlice.intValue}\nState: {_currentState.enumDisplayNames[_currentState.intValue]}", MessageType.Info);
        base.OnInspectorGUI();
    }
}
#endif