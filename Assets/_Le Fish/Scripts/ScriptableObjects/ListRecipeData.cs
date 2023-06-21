using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ListRecipe", menuName = "ScriptableObjects/List of Recipes")]
public class ListRecipeData : ScriptableObject
{
    public List<RecipeData> ListRecipes = new();

    private void OnValidate()
    {
        for (int i = 0; i < ListRecipes.Count; i++)
            ListRecipes[i].Ingredients = ListRecipes[i].Ingredients.OrderBy(i => i.Ingredient.Name).ToList();
    }

    public bool TryFindDish(List<Ingredient> ingredients, out RecipeData dish)
    {
        List<Ingredient> sortedIngredients = ingredients.OrderBy(ingredient => ingredient.Data.Name).ToList();
        bool _isActual = false;

        for (int i = 0; i < ListRecipes.Count; i++)
        {
            if (ingredients.Count != ListRecipes[i].Ingredients.Count)
                continue;

            _isActual = true;

            for (int j = 0; j < ingredients.Count; j++)
            {
                if (sortedIngredients[j].Data.Name != ListRecipes[i].Ingredients[j].Ingredient.Name)
                    _isActual = false;

                if (sortedIngredients[j].CurrentState != ListRecipes[i].Ingredients[j].CookingState)
                    _isActual = false;

                if (sortedIngredients[j].CurrentSlice != ListRecipes[i].Ingredients[j].SliceState)
                    _isActual = false;

                if (!_isActual) break;
            }

            if (_isActual)
            {
                dish = ListRecipes[i];
                return true;
            }
        }

        dish = null;
        return false;
    }
}


