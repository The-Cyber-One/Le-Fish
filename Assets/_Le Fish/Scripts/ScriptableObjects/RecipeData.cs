using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Data", menuName = "ScriptableObjects/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string Name;

    [SerializeField] private List<DataStateSlice> ingredients;

    [Serializable]
    public class DataStateSlice
    {
        [SerializeField] public IngredientData Ingredient;
        [SerializeField] public IngredientData.IngredientState CookingState;
        [SerializeField] public int SliceState;
    }
}
