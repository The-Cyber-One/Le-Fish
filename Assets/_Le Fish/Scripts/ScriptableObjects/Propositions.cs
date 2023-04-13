using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Propositions Data", menuName = "ScriptableObjects/Propositions Data")]
public class Propositions : ScriptableObject
{
    public string Name;
    public IngredientData SpecialIngredient;

    [SerializeField] private RecipeData[] recipes = new RecipeData[3];
}
