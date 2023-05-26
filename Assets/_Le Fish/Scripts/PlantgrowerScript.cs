using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantgrowerScript : MonoBehaviour
{
    private Animator plantAnimator;
    public Transform vegetablePlace;
    public GameObject vegetablePrefab;

    void Start()
    {
        plantAnimator = GetComponent<Animator>();
    }

    public void PickVegetable()
    {
        if (!plantAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pick Vegetable"))
        {
            GameObject newVegetable = Instantiate(vegetablePrefab, vegetablePlace.position, Quaternion.identity);
            newVegetable.transform.parent = vegetablePlace;
            plantAnimator.SetTrigger("Pick Vegetable");
        }
    }
}
