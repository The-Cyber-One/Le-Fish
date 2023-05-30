using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Data", menuName = "ScriptableObjects/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string Name;
    [TextArea(3, 5)] public string Description;
    public Dialog Instructions;
    public GameObject DishPrefab;
    public List<DataStateSlice> Ingredients;

    [Serializable]
    public class DataStateSlice
    {
        [SerializeField] public IngredientData Ingredient;
        [SerializeField] public IngredientData.IngredientState CookingState;
        [SerializeField] public int SliceState;
    }
}
