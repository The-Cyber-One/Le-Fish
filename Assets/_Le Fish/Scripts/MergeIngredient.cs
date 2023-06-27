using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeIngredient : MonoBehaviour
{
    private readonly List<Ingredient> _ingredients = new();
    [SerializeField] Transform spawnDishTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ingredient newIngredient))
            return;

        _ingredients.Add(newIngredient);
        newIngredient.transform.SetParent(transform);

        CreateDish();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ingredient ingredient))
            return;

        _ingredients.Remove(ingredient);
        ingredient.transform.SetParent(null);
    }

    private void CreateDish()
    {
        if (TryFindDish(_ingredients, out var dish))
        {
            DishData dishData = Instantiate(dish.DishPrefab, spawnDishTransform.position, Quaternion.identity).AddComponent<DishData>();
            dishData.Data = dish;
            CustomerSpawner.Instance.UpdateDish(dishData);
            _ingredients.ForEach(ingredient => Destroy(ingredient.gameObject));
            _ingredients.Clear();
        }
    }

    public bool TryFindDish(List<Ingredient> ingredients, out RecipeData dish)
    {
        List<RecipeData> listRecipes = ConchyAI.Instance.MergableRecipes;
        for (int i = 0; i < listRecipes.Count; i++)
            listRecipes[i].Ingredients = listRecipes[i].Ingredients.OrderBy(i => i.Ingredient.Name).ToList();

        List<Ingredient> sortedIngredients = ingredients.OrderBy(ingredient => ingredient.Data.Name).ToList();
        bool _isActual = false;

        for (int i = 0; i < listRecipes.Count; i++)
        {
            if (ingredients.Count != listRecipes[i].Ingredients.Count)
                continue;

            _isActual = true;

            for (int j = 0; j < ingredients.Count; j++)
            {
                if (sortedIngredients[j].Data.Name != listRecipes[i].Ingredients[j].Ingredient.Name)
                    _isActual = false;

                if (sortedIngredients[j].CurrentState != listRecipes[i].Ingredients[j].CookingState)
                    _isActual = false;

                if (sortedIngredients[j].CurrentSlice != listRecipes[i].Ingredients[j].SliceState)
                    _isActual = false;

                if (!_isActual) break;
            }

            if (_isActual)
            {
                dish = listRecipes[i];
                return true;
            }
        }

        dish = null;
        return false;
    }
}