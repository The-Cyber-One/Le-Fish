using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Recipe Data", menuName = "ScriptableObjects/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string Name;
    [TextArea(3, 5)] public string Description;
    [SerializeField] Image Image;
    [SerializeField] GameObject DishPrefab;
    [SerializeField] List<DataStateSlice> Ingredients;

    [Serializable]
    public class DataStateSlice
    {
        [SerializeField] public IngredientData Ingredient;
        [SerializeField] public IngredientData.IngredientState CookingState;
        [SerializeField] public int SliceState;
    }
}
