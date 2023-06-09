using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Slicer : MonoBehaviour
{
     [SerializeField] XRGrabInteractable interactable;

    void OnCollisionEnter(Collision collision)
    {
        if (interactable.isSelected && collision.collider.TryGetComponent(out Ingredient ingredient))
        {
            ingredient.Slice();
        }
    }
}



