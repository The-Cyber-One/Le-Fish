using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    Recipe _recipeReady;

    // Start is called before the first frame update
    public void AddStuff(Collider collider)
    {
        collider.TryGetComponent<Recipe>(out _recipeReady);
    }

    // Update is called once per frame
    public void RemoveStuff()
    {
        
    }
}
