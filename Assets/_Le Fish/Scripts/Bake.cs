using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IngredientData;

public class Bake : MonoBehaviour
{
    private List<Ingredient> _ingredients = new();

    public void StartTimer()
    {
        foreach (Ingredient ingredient in _ingredients)
        {
            if (ingredient.CurrentState != IngredientState.Burnt &&
                !ingredient.IsCooking &&
                ingredient.Data.Slices[ingredient.CurrentSlice].Meshes[(int)ingredient.CurrentState + 1] != null)
            {
                ingredient.Timer.onTimerUpdate.AddListener(Percentage);
                switch (ingredient.CurrentState)
                {
                    case IngredientState.Raw:
                        ingredient.Timer.onTimerFinished.AddListener(() => StateCook(ingredient));
                        ingredient.Timer.StartTimer(ingredient.Data.CookingTime);
                        break;
                    case IngredientState.Cooked:
                        ingredient.Timer.onTimerFinished.AddListener(() => StateBurn(ingredient));
                        ingredient.Timer.StartTimer(ingredient.Data.BurnTime);
                        break;
                }
                ingredient.IsCooking = true;
            }
        }
    }

    public void StopTimer()
    {
        for (int i = 0; i < _ingredients.Count; i++)
        {
            _ingredients[i].Timer.StopAllCoroutines();
            _ingredients[i].IsCooking = false;
        }
    }

    public void AddIngredient(Collider collider)
    {
        _ingredients.Add(collider.gameObject.GetComponent<Ingredient>());
    }

    public void RemoveIngredient(Collider collider)
    {
        _ingredients.Remove(collider.gameObject.GetComponent<Ingredient>());
    }

    void StateCook(Ingredient ingredient)
    {
        Debug.Log("Ingredient is Cooked !");
        ingredient.Timer.onTimerFinished.RemoveAllListeners();
        ingredient.Timer.onTimerUpdate.RemoveAllListeners();

        ingredient.Timer.onTimerUpdate.AddListener(Percentage);
        ingredient.Timer.onTimerFinished.AddListener(() => StateBurn(ingredient));
        ingredient.Timer.StartTimer(ingredient.Data.BurnTime);

        ingredient.IsCooking = false;
        ingredient.SetState(IngredientState.Cooked);
    }

    void StateBurn(Ingredient ingredient)
    {
        Debug.Log("Ingredient is Burned !!!");
        ingredient.Timer.onTimerFinished.RemoveAllListeners();
        ingredient.Timer.onTimerUpdate.RemoveAllListeners();
        ingredient.IsCooking = false;
        ingredient.SetState(IngredientState.Burnt);
    }

    void Percentage(float percentage)
    {
        Debug.Log(percentage);
    }
}
