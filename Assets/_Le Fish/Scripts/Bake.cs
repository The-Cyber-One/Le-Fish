using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bake : MonoBehaviour
{
    public bool DoorOpened { get; set; }                       
    private List<Ingredient> _ingredients = new();  // new List<Ingredient>();
    int i;

    public void StartTimer()
    {
        if (!DoorOpened && _ingredients != null)
        {
            for (i = 0; i < _ingredients.Count; i++)
            {
                if (_ingredients[i].CurrentState == Ingredient.IngredientState.Raw && _ingredients[i].IsCooking == false)
                {
                    _ingredients[i].timer.onTimerUpdate.AddListener(Percentage);
                    _ingredients[i].timer.onTimerFinished.AddListener(() => StateCook(i));
                    _ingredients[i].timer.StartTimer(_ingredients[i].cookingTime);
                    _ingredients[i].IsCooking = true;
                }

                if (_ingredients[i].CurrentState == Ingredient.IngredientState.Cooked && _ingredients[i].IsCooking == false)
                {
                    _ingredients[i].timer.onTimerFinished.AddListener(() => StateBurn(i));
                    _ingredients[i].timer.onTimerUpdate.AddListener(Percentage);
                    _ingredients[i].timer.StartTimer(_ingredients[i].burnTime);
                    _ingredients[i].IsCooking = true;
                }
            }
        }
    }

    public void StopTimer()
    {
        if (DoorOpened)
        {
            
            for (i = 0; i < _ingredients.Count; i++)
            {
                _ingredients[i].timer.StopAllCoroutines();
                _ingredients[i].IsCooking = false;
            }
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

    void StateCook(int i)
    {
        Debug.Log("Ingredient is Cooked !");
        _ingredients[i].timer.onTimerFinished.RemoveAllListeners();
        _ingredients[i].timer.onTimerUpdate.RemoveAllListeners();
        _ingredients[i].IsCooking = false;
        _ingredients[i].NextState(Ingredient.IngredientState.Cooked);
    }

    void StateBurn(int i)
    {
        Debug.Log("Ingredient is Burned !!!");
        _ingredients[i].timer.onTimerFinished.RemoveAllListeners();
        _ingredients[i].timer.onTimerUpdate.RemoveAllListeners();
        _ingredients[i].IsCooking = false;
        _ingredients[i].NextState(Ingredient.IngredientState.Burnt);
    }

    void Percentage(float percentage)
    {
        Debug.Log(percentage);
    }
}
