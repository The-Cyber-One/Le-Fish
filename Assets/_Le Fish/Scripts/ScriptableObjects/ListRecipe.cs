using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu(fileName = "ListRecipe", menuName = "ScriptableObjects/List of Recipes")]
public class ListRecipe : ScriptableObject
{
    [SerializeField] private Dictionary<string, Recipe> list_recipe;

    public void Initialize()
    {
        list_recipe = new();

        Recipe banana_split = new();
        banana_split.recipe_name = "banana_split";
        //banana_split.ingredients = 

        list_recipe.Add("banana_split", banana_split);
        
    }
}
