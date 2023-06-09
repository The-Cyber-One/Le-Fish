using UnityEngine;
using System.Collections.Generic;

public class FallenObjectDetector : MonoBehaviour
{
    public GameObject conchyHelper;
    private HashSet<Collider> enteredObjects = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient")||other.CompareTag("Utensil"))
        {
            if (other.transform.parent == null)
            {
                if (!enteredObjects.Contains(other))
                {
                    // Get the position of the entering object
                    Vector3 enteringObjectPosition = other.transform.position;

                    // Calculate the position for the conchyHelper below the entering object
                    Vector3 conchyPosition = enteringObjectPosition - new Vector3(0, 0.2f, 0);

                    // Instantiate the conchyHelper at the calculated position
                    GameObject conchyInstance = Instantiate(conchyHelper, conchyPosition, Quaternion.identity);

                    // Set the entering object as the child of the conchy instance
                    other.transform.SetParent(conchyInstance.transform);

                    // Make the entering object's Rigidbody kinematic and zero out the y-velocity
                    Rigidbody enteringObjectRigidbody = other.GetComponent<Rigidbody>();
                    if (enteringObjectRigidbody != null)
                    {
                        enteringObjectRigidbody.useGravity = false;
                        enteringObjectRigidbody.isKinematic = true;
                        Vector3 velocity = enteringObjectRigidbody.velocity;
                        velocity.y = 0f;
                        enteringObjectRigidbody.velocity = velocity;
                    }
                    // Disable the entering object's collider
                    Collider enteringCollider = other.GetComponent<Collider>();
                    if (enteringCollider != null)
                    {
                        enteringCollider.enabled = false;
                    }
                    // Add the object to the set of entered objects
                    enteredObjects.Add(other);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ingredient") || other.CompareTag("Utensil"))
        {
            enteredObjects.Remove(other);
        }
    }
}
