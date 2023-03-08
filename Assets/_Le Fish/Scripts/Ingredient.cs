using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private IngredientState currentState;
    [SerializeField] private List<IngredientData> ingredientData;

    private Dictionary<IngredientState, Mesh> _ingredients;

    [Serializable]
    private class IngredientData
    {
        [SerializeField] public IngredientState state;
        [SerializeField] public Mesh mesh;
    }

    private void OnValidate()
    {
        var possibleEnums = Enum.GetValues(typeof(IngredientState)).ConvertTo<List<IngredientState>>();
        int maxLength = possibleEnums.Count;
        if (ingredientData.Count > maxLength)
        {
            ingredientData = ingredientData.Take(maxLength).ToList();
            LogEnumWarning();
        }
        else
        {
            for (int i = 0; i < ingredientData.Count; i++)
            {
                if (possibleEnums.Contains(ingredientData[i].state))
                {
                    possibleEnums.Remove(ingredientData[i].state);
                }
                else
                {
                    LogEnumWarning();
                    ingredientData[i] = new IngredientData { state = possibleEnums[0] };
                    possibleEnums.RemoveAt(0);
                }
            }
        }

        static void LogEnumWarning() => Debug.LogWarning("There are no more unique states");

        _ingredients = ingredientData.ToDictionary(ingredientPair => ingredientPair.state, ingredientPair => ingredientPair.mesh);
    }

    public enum IngredientState
    {
        Raw,
        Cooked,
        Burnt
    }

    public void UpdateState(IngredientState state)
    {
        currentState = state;
        meshFilter.mesh = _ingredients[state];
    }
}
