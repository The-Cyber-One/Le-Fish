using System.Collections;
using System.Collections.Generic;
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



