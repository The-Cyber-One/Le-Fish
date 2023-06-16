using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Recipe Data", menuName = "ScriptableObjects/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string Name;
    public List<DataStateSlice> Ingredients;
    public GameObject DishPrefab;
    [TextArea(3, 5)] public string Description;
    [SerializeField] Image Image;

    [Serializable]
    public class DataStateSlice
    {
        [SerializeField] public IngredientData Ingredient;
        [SerializeField] public IngredientData.IngredientState CookingState;
        [SerializeField] public int SliceState;
    }
}
