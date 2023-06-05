using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeIngredient : MonoBehaviour
{
    private ListRecipeData _listRecipe;
    private readonly List<Ingredient> _ingredients = new();

    void Awake()
    {
        _listRecipe = Resources.Load<ListRecipeData>("ListRecipes");

        if (_listRecipe == null) Debug.Log("ListRecipes not define !");
    }

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
        if (_listRecipe.TryFindDish(_ingredients, out var dish))
        {
            Instantiate(dish.DishPrefab, _ingredients[^1].transform.position, Quaternion.identity);
            _ingredients.ForEach(ingredient => Destroy(ingredient.gameObject));
            _ingredients.Clear();
        }
    }
}