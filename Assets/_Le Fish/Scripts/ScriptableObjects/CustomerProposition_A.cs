using UnityEngine;

[CreateAssetMenu(fileName = "CustomerProposition_A", menuName = "ScriptableObjects/CustomerProposition_A")]
public class CustomerProposition_A : ScriptableObject
{
    // precise real name we will use : salmon for example
    public Recipe name_A;
    public Recipe name_B;
    public Recipe name_C;
    public Ingredient ingredient_A;
}
