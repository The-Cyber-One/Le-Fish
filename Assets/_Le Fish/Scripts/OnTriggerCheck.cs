using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCheck : MonoBehaviour
{
   public MenuScreenScript MenuScreenScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            MenuScreenScript.MoveToRestaurant();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger!");
            MenuScreenScript.MoveToMenu();
        }
    }
}
