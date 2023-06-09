using System.Collections;
using UnityEngine;

public class Slicer : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Ingredient ingredient))
        {
            ingredient.Slice();
        }
    }
}



