using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishMakingScript : MonoBehaviour
{
    public ColliderDetector ColliderDetector;
    public GameObject[] ingredients;
    public GameObject mainIngredient;

    private List<GameObject> collidedObjects = new List<GameObject>();
    // Start is called before the first frame update
    private void OnEnable()
    {
        ColliderDetector ColliderDetector = GetComponent<ColliderDetector>();
        ColliderDetector.OnEnterTrigger.AddListener(MakeMainIngredient);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MakeMainIngredient(Collider other)
    {
        if (other.gameObject.GetComponent("Ingredient") != null) 
        {
            
         
        }
    }
}
