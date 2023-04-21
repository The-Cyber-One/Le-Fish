using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ListRecipe", menuName = "ScriptableObjects/List of Recipes")]
public class ListRecipeData : ScriptableObject
{
    public List<MyKeyValuePair> ListRecipes = new();

    [Serializable]
    public class MyKeyValuePair
    {
        [SerializeField] public string key;
        [SerializeField] public RecipeData data;
    }

    public bool TryFindDish(List<IngredientData> ingredients, out RecipeData dish)
    {
        //TODO: Find the dish that is available with the ingredients
        dish = ListRecipes[0].data;
        return true;
    }
}
