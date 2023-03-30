using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListIngredients", menuName = "ScriptableObjects/List of Ingredients")]

public class ListIngredients : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, IngredientData> listIngredients = new();
    
    //// SPICES AND HERBS

    //Ingredient salt = new()
    //{
    //    ingredient_name = "salt",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 0,
    //    timer = new()
    //};

    //Ingredient pepper = new()
    //{
    //    ingredient_name = "pepper",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 0,
    //    timer = new()
    //};

    //Ingredient basil = new()
    //{
    //    ingredient_name = "basil",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 0,
    //    timer = new()
    //};

    //Ingredient coriander = new()
    //{
    //    ingredient_name = "coriander",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 0,
    //    timer = new()
    //};

    //Ingredient curry = new()
    //{
    //    ingredient_name = "curry",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 0,
    //    timer = new()
    //};

    //listIngredients.Add(salt.ingredient_name, salt);
    //listIngredients.Add(pepper.ingredient_name, pepper);
    //listIngredients.Add(basil.ingredient_name, basil);
    //listIngredients.Add(coriander.ingredient_name, coriander);
    //listIngredients.Add(curry.ingredient_name, curry);

    //// VEGGIES

    //Ingredient onion = new()
    //{
    //    ingredient_name = "onion",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient carrot = new()
    //{
    //    ingredient_name = "carrot",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient tomato = new()
    //{
    //    ingredient_name = "tomato",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient mushroom = new()
    //{
    //    ingredient_name = "mushroom",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient cucumber = new()
    //{
    //    ingredient_name = "cucumber",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient cabbage = new()
    //{
    //    ingredient_name = "cabbage",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //listIngredients.Add(onion.ingredient_name, onion);
    //listIngredients.Add(carrot.ingredient_name, carrot);
    //listIngredients.Add(tomato.ingredient_name, tomato);
    //listIngredients.Add(mushroom.ingredient_name, mushroom);
    //listIngredients.Add(cucumber.ingredient_name, cucumber);
    //listIngredients.Add(cabbage.ingredient_name, cabbage);

    //// OTHER STUFF

    //Ingredient tofu = new()
    //{
    //    ingredient_name = "tofu",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient rice = new()
    //{
    //    ingredient_name = "rice",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //Ingredient olive_oil = new()
    //{
    //    ingredient_name = "olive_oil",
    //    cookingTime = 0,
    //    burnTime = 0,
    //    category = 1,
    //    timer = new()
    //};

    //listIngredients.Add(tofu.ingredient_name, tofu);
    //listIngredients.Add(rice.ingredient_name, rice);
    //listIngredients.Add(olive_oil.ingredient_name, olive_oil);
    
}
