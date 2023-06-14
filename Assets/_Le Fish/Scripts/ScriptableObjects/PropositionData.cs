using UnityEngine;

[CreateAssetMenu(fileName = "Propositions Data", menuName = "ScriptableObjects/Propositions Data")]
public class PropositionData : ScriptableObject
{
    public string Name;
    public IngredientData SpecialIngredient;

    [SerializeField, Header("First out of the recipes is 'correct' dish")] public RecipeData[] Recipes = new RecipeData[3];
}
