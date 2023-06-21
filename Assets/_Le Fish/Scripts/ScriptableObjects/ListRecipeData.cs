using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListRecipe", menuName = "ScriptableObjects/List of Recipes")]
public class ListRecipeData : ScriptableObject
{
    public List<RecipeData> ListRecipes;
}


