using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "List_Recipe", menuName = "ScriptableObjects/List of Blocks Propositions")]
public class ListBlockPropositions : ScriptableObject
{
    public List<Options> options = new();

    Dictionary<string, Options> mRecipes = new();
    [System.Serializable]
    public class Options
    {
        [SerializeField] public string name;
        [SerializeField] public CustomerProposition_A recipe_A;
        [SerializeField] public CustomerProposition_B recipe_B;
        [SerializeField] public CustomerProposition_C recipe_C;
    }

    private void OnValidate()
    {
        foreach (var item in options)
        {
            mRecipes.Add(item.name, item);
        }
    }

    public Options GetOption(string name)
    {
        return mRecipes[name];
    }
}
