using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Slicer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)

    {
        other.GetComponent<Ingredient>().Slice();

    }
}



