using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IngredientData;

public class Recipe : MonoBehaviour
{ 
    public string Name;

    public List<CurrentIngredient> IngredientInside;

    public struct CurrentIngredient
    {
        public IngredientData IngredientInfo;
        public IngredientData.IngredientState CurrentCookingState;
        public int CurrentSliceState;
    }
}
