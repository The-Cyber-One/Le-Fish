using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantgrowerScript : MonoBehaviour
{
    private Animator plantAnimator;
    public Transform vegetablePlace;
    public GameObject[] vegetablePrefabs;
    public ParticleSystem foodGeneration;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        plantAnimator = GetComponent<Animator>();
    }

    public void PickVegetable(int index)
    {
        if (!plantAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pick Vegetable"))
        {
            foodGeneration.Play();
            audioSource.Play();
            GameObject newVegetable = Instantiate(vegetablePrefabs[index], vegetablePlace.position, Quaternion.identity);
            newVegetable.transform.parent = vegetablePlace;
            newVegetable.GetComponent<Rigidbody>().isKinematic = true;
            plantAnimator.SetTrigger("Pick Vegetable");
        }
    }
}
