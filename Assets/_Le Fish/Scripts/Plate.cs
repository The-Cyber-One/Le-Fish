using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private RecipeData _recipeReady;
    private IngredientData _ingredient;

    // Do it with item socket
    public void AddStuff(Collider collider)
    {
        if(collider.TryGetComponent<RecipeData>(out _recipeReady))
        {
            return;
        }

        if (collider.TryGetComponent<IngredientData>(out _ingredient))
        {
            return;
        }
    }

    public void RemoveStuff()
    {
        
    }
}
