using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(fileName = "ListIngredients", menuName = "ScriptableObjects/List of Ingredients")]

public class ListIngredients : ScriptableObject
{
    [SerializeField] public List<MyKeyValuePair> listIngredients = new();

    [Serializable]
    public class MyKeyValuePair
    {
        [SerializeField] public string key;
        [SerializeField] public IngredientData data;
    }
}