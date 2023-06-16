using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingFix : MonoBehaviour
{
    private GameObject currentIngredient;
    public bool kinematic;
    private Rigidbody currentRigidbody;
    // Start is called before the first frame update
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ingredient")
        {
            currentIngredient = other.gameObject;
           currentRigidbody = currentIngredient.GetComponent<Rigidbody>();
        }
    }
    public void SetKinematicFalse()
    {
        currentRigidbody.isKinematic = false;
    }
}
