using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bake : MonoBehaviour
{
    public bool DoorOpened { get; set; }            // check if door is opened/closed
    public bool ingredientInside { get; set; }      // check if ingredient is inside
    private bool waitIngredient = false;            // start one timer and wait for the end of it
    [SerializeField] Timer timer;                   // timer associated to the oven
    private int stateIngredient = 0;                // get the ingredient state later

    // check every frame if we have to put new timer because we have to know always if door is opened
    private void Update()
    {
        // maybe useless
        if (DoorOpened)                             
        {
            timer.StopAllCoroutines();
        }

        //  if door is closed ingredient is inside and no timer already up start a new timer
        if (!DoorOpened && waitIngredient == false && ingredientInside)
            // one timer to cook one to burn
            if (stateIngredient == 0 || stateIngredient == 1)   // state ingredient : 0 = raw / 1 = cooked / 2 = burned 
            {
                waitIngredient = true;   // timer will be set don't need another one for the moment

                // new timer with feedback about percentage of cooking and when its finished
                if (stateIngredient == 0)
                {
                    timer.onTimerFinished.AddListener(StateCook);
                    timer.onTimerUpdate.AddListener(percentage);       // for the moment we can remove function and put directly into the parameter
                    timer.StartTimer(6);                               // waiting seconds can be changed
                }

                // will start frame after ingredient will be cooked
                else if (stateIngredient == 1)
                {
                    timer.onTimerFinished.AddListener(StateBurn);       // need new listener
                    timer.onTimerFinished.RemoveListener(StateCook);    // don't need old listener anymore
                    timer.StartTimer(4);                               // waiting seconds can be changed
                }
            }
    }

    void StateCook()
    {
        Debug.Log("Ingredient is Cooked !");
        waitIngredient = false;                             // timer is finished 
        stateIngredient = 1;                                // change the state of the ingredient when it will be created
    }

    void StateBurn()
    {
        Debug.Log("Ingredient is Burned !!!");
        timer.onTimerFinished.RemoveListener(StateCook);
        timer.onTimerUpdate.RemoveListener(percentage);
        waitIngredient = false;                             // timer is finished 
        stateIngredient = 2;                                // change the state of the ingredient when it will be created
    }

    void percentage(float percentage)
    {
        Debug.Log(percentage);
    }
}
