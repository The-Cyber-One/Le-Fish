using System;
using UnityEngine;
using static IngredientData;
using static RecipeData;

[Serializable, RequireComponent(typeof(Timer))]
public class Ingredient : MonoBehaviour
{
    public IngredientState CurrentState { get; private set; }
    public int CurrentSlice { get; private set; }
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject ashPrefab;
    public bool IsCooking = false;
    public IngredientData Data;

    Timer _timer;
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
            Instantiate(ashPrefab);
            Destroy(gameObject);
        }
        else
            UpdateMesh();
    }

    public void Bake()
    {
        if ((int)CurrentState < Data.Slices[CurrentSlice].States.Length - 1)
        {
            bool hasNextState = Data.Slices[CurrentSlice].Meshes[(int)CurrentState + 1] != null;
            SetState(hasNextState ? ++CurrentState : IngredientState.Burnt);
        }
    }

    public void Slice()
    {
        if (CurrentSlice < Data.Slices.Length - 1 && Data.Slices[CurrentSlice + 1].Meshes[(int)CurrentState] != null)
        {
            CurrentSlice++;
            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        var data = Data.Slices[CurrentSlice].Meshes[(int)CurrentState];
        meshFilter.mesh = data.Mesh;
        meshRenderer.materials = data.Materials;
    }
}