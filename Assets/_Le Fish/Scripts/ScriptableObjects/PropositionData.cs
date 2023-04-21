using UnityEngine;

[CreateAssetMenu(fileName = "Propositions Data", menuName = "ScriptableObjects/Propositions Data")]
public class PropositionData : ScriptableObject
{
    public string Name;
    public IngredientData SpecialIngredient;

    [SerializeField] public RecipeData[] Recipes = new RecipeData[3];
}
