using System;
using UnityEngine;
using static IngredientData;
using static RecipeData;

[Serializable, RequireComponent(typeof(Timer))]
public class Ingredient : MonoBehaviour
{
    public IngredientState CurrentState { get; private set; }
    public int CurrentSlice { get; private set; }
    public bool IsCooking = false;
    public IngredientData Data;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject ashPrefab;
    [SerializeField] ParticleSystem starParticlePrefab;

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
}