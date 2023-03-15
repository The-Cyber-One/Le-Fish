using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bake : MonoBehaviour
{
    public bool DoorOpened { get; set; }              
    [SerializeField] Timer timer;           
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
                    timer.onTimerUpdate.AddListener(Percentage);
                    timer.onTimerFinished.AddListener(() => StateCook(i));
                    timer.StartTimer(4);
                    _ingredients[i].IsCooking = true;
                }

                if (_ingredients[i].CurrentState == Ingredient.IngredientState.Cooked && _ingredients[i].IsCooking == false)
                {
                    timer.onTimerFinished.AddListener(() => StateBurn(i));
                    timer.onTimerUpdate.AddListener(Percentage);
                    timer.StartTimer(4);
                    _ingredients[i].IsCooking = true;
                }
            }
        }
    }

    public void StopTimer()
    {
        if (DoorOpened)
        {
            timer.StopAllCoroutines();
            for (i = 0; i < _ingredients.Count; i++)
            {
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
        timer.onTimerFinished.RemoveAllListeners();
        timer.onTimerUpdate.RemoveAllListeners();
        _ingredients[i].IsCooking = false;
        _ingredients[i].NextState(Ingredient.IngredientState.Cooked);
    }

    void StateBurn(int i)
    {
        Debug.Log("Ingredient is Burned !!!");
        timer.onTimerFinished.RemoveAllListeners();
        timer.onTimerUpdate.RemoveAllListeners();
        _ingredients[i].IsCooking = false;
        _ingredients[i].NextState(Ingredient.IngredientState.Burnt);
    }

    void Percentage(float percentage)
    {
        Debug.Log(percentage);
    }
}
