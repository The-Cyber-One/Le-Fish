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
    public bool IsCooking = false;
    public IngredientData Data;

    Timer _timer;
    public Timer Timer => _timer = _timer != null ? _timer : GetComponent<Timer>();

    public static implicit operator DataStateSlice(Ingredient ingredient) => new() {
        Ingredient = ingredient.Data,
        CookingState = ingredient.CurrentState,
        SliceState = ingredient.CurrentSlice
    };

    public void SetState(IngredientState state)
    {
        CurrentState = state;
        meshFilter.mesh = Data.Slices[CurrentSlice].Meshes[(int)state];
    }

    public void Bake()
    {
        if ((int)CurrentState < Data.Slices[CurrentSlice].States.Length - 1)
        {
            CurrentState++;
            meshFilter.mesh = Data.Slices[CurrentSlice].Meshes[(int)CurrentState];
        }
    }

    public void Slice()
    {
        if (CurrentSlice < Data.Slices.Length - 1)
        {
            CurrentSlice++;
            meshFilter.mesh = Data.Slices[CurrentSlice].Meshes[(int)CurrentState];
        }
    }
}