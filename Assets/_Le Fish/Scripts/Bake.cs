using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bake : MonoBehaviour
{
    public bool DoorOpened { get; set; }            // check if door is opened/closed
    public bool IngredientInside { get; set; }      // check if ingredient is inside
    [SerializeField] Timer timer;                   // timer associated to the oven
    private List<GameObject/*Ingredient*/> _ingredients;
    private bool _waitIngredient = false;           // start one timer and wait for the end of it
    private int _stateIngredient = 0;               // get the ingredient state later

    // check every frame if we have to put new timer because we have to know always if door is opened
    private void Update()
    {
        // maybe useless
        if (DoorOpened)
        {
            timer.StopAllCoroutines();
        }

        //  if door is closed ingredient is inside and no timer already up start a new timer
        if (!DoorOpened && _waitIngredient == false && IngredientInside)
            // one timer to cook one to burn
            if (_stateIngredient == 0 || _stateIngredient == 1)   // state ingredient : 0 = raw / 1 = cooked / 2 = burned 
            {
                _waitIngredient = true;   // timer will be set don't need another one for the moment

                // new timer with feedback about percentage of cooking and when its finished
                if (_stateIngredient == 0)
                {
                    timer.onTimerFinished.AddListener(StateCook);
                    timer.onTimerUpdate.AddListener(Percentage);       // for the moment we can remove function and put directly into the parameter
                    timer.StartTimer(6);                               // waiting seconds can be changed
                }

                // will start frame after ingredient will be cooked
                else if (_stateIngredient == 1)
                {
                    timer.onTimerFinished.AddListener(StateBurn);       // need new listener
                    timer.onTimerFinished.RemoveListener(StateCook);    // don't need old listener anymore
                    timer.StartTimer(4);                               // waiting seconds can be changed
                }
            }
    }

    public void AddIngredient(Collider collider)
    {
        _ingredients.Add(collider.gameObject/*.GetComponent<Ingredient>()*/);
    }

    public void RemoveIngredient()
    {
        // Remove the ingredient
    }

    void StateCook()
    {
        Debug.Log("Ingredient is Cooked !");
        _waitIngredient = false;                             // timer is finished 
        _stateIngredient = 1;                                // change the state of the ingredient when it will be created
        // ingredient.NextState();
    }

    void StateBurn()
    {
        Debug.Log("Ingredient is Burned !!!");
        timer.onTimerFinished.RemoveListener(StateCook);
        timer.onTimerUpdate.RemoveListener(Percentage);
        _waitIngredient = false;                             // timer is finished 
        _stateIngredient = 2;                                // change the state of the ingredient when it will be created
    }

    void Percentage(float percentage)
    {
        Debug.Log(percentage);
    }
}
