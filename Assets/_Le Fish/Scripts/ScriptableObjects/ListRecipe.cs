using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ListRecipe", menuName = "ScriptableObjects/List of Recipes")]
public class ListRecipe : ScriptableObject
{
    [SerializeField] public List<MyKeyValuePair> listRecipes = new();

    [Serializable]
    public class MyKeyValuePair
    {
        [SerializeField] public string key;
        [SerializeField] public RecipeData data;
    }
}
