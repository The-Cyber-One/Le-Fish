using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Recipe;
using static RecipeData;
using static UnityEditor.Progress;

public class MergeIngredient : MonoBehaviour
{
    private Recipe _recipeInProgress;
    private ListRecipe _listRecipe;
    private List<GameObject> _ingredients = new();
    private GameObject recipeObject;

    // Start is called before the first frame update
    void Start()
    {
        _recipeInProgress = new()
        {
            IngredientInside = new()
        };

        _listRecipe = Resources.Load<ListRecipe>("ListRecipes");
    }

    public void UpdateIngredientRecipe(Collider collider)
    {
        _ingredients.Add(collider.gameObject);

        Ingredient currentIngredient = collider.gameObject.GetComponent<Ingredient>();

        CurrentIngredient currentIngredientStruct = new()
        {
            IngredientInfo = currentIngredient.Data,
            CurrentCookingState = currentIngredient.CurrentState,
            CurrentSliceState = currentIngredient.CurrentSlice
        };

        _recipeInProgress.IngredientInside.Add(currentIngredientStruct);

        CheckRecipeMatch();
    }
        
    public void CheckRecipeMatch()
    {
        foreach (var recipe in _listRecipe.listRecipes)
        {        
            if (CompareLists(_recipeInProgress.IngredientInside, recipe.data.Ingredients) == true)
            {
                _recipeInProgress.Name = recipe.data.Name;
                
                Debug.Log("Recipe match !");
                Debug.Log(_recipeInProgress.Name);
                
                recipeObject = new GameObject("Recipe");
                recipeObject.AddComponent<MeshFilter>();
                recipeObject.AddComponent<MeshRenderer>().material = _ingredients[0].GetComponent<MeshRenderer>().sharedMaterial;  // change the material associated to any possible situation?
                _recipeInProgress = recipeObject.AddComponent<Recipe>(); 

                MergeMesh(recipeObject.GetComponent<MeshFilter>().mesh);                    

                //recipeObject.transform.position = AveragePosition(_ingredients);

                foreach (var gameObject in _ingredients){
                    Destroy(gameObject);
                }

                break;                                                  // probably not a good way to use break like this
            }
            else
            {
                // add later
                _recipeInProgress.Name = "RandomCombinaison";
            }
        }

        Debug.Log(_recipeInProgress.Name);
    }

    public bool CompareLists(List<CurrentIngredient> list1, List<DataStateSlice> list2)
    {
        List<CurrentIngredient> convertedList2 = new();
        CurrentIngredient convertedItem = new();

        foreach (var item in list2)
        {
            convertedItem.IngredientInfo = item.Ingredient;
            convertedItem.CurrentCookingState = item.CookingState;
            convertedItem.CurrentSliceState = item.SliceState;

            convertedList2.Add(convertedItem);
        }

        bool Check;
        return Check = list1.All(convertedList2.Contains) && list1.Count == convertedList2.Count;
    }

    public void MergeMesh(Mesh resultMesh)
    {
        CombineInstance[] combine = new CombineInstance[_ingredients.Count];

        for (int i = 0; i < _ingredients.Count; i++)
        {
            combine[i].mesh = _ingredients[i].GetComponent<MeshFilter>().sharedMesh;
            combine[i].transform = _ingredients[i].transform.localToWorldMatrix;
        }

        resultMesh.CombineMeshes(combine);
    }

    public Vector3 AveragePosition(List<GameObject> _ingredients)
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (GameObject ingredient in _ingredients)
        {
            averagePosition += ingredient.transform.position;
        }

        averagePosition /= _ingredients.Count;

        return averagePosition;
    }

    public Recipe ReturnRecipe()
    {
        if (_recipeInProgress != null)
            return _recipeInProgress;

        else
        {
            Debug.Log("_recipeInProgress null");
            return null;
        }
    }
}