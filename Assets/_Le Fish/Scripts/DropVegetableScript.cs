using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropVegetableScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            // Remove parent
            other.transform.parent = null;
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }
    }
}
