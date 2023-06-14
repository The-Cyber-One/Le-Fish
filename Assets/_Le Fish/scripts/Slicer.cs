using System.Collections;
using UnityEngine;

public class Slicer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Ingredient ingredient))
        {
            ingredient.Slice();
            audioSource.Play();
        }
    }
}



