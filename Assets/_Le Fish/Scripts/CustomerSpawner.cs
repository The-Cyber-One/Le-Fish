using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : Singleton<CustomerSpawner>
{
    [HideInInspector] public List<bool> AvailableSeats = new();
    [HideInInspector] public GameObject WaitingDish;

    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] customers;

    public List<Transform> EatPoints = new();
    public Transform ingredientSpawn;
    public Transform OrderPoint, AwayPoint;

    void Start()
    {
        //Spawns the first customer to start the sequence, if the number of customers is smaller than the max amount of customers
        for (int i = 0; i < EatPoints.Count; i++)
            AvailableSeats.Add(true);
    }

    [ContextMenu(nameof(SpawnCustomers))]
    public void SpawnCustomers()
    {
        int random = Random.Range(0, customers.Length);
        CustomerBehavior instance = Instantiate(customers[random], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
        instance.GetSpawner(this);
    }

    public void UpdateDish(Collider collider)
    {
        if (!collider.TryGetComponent(out RecipeData dish))
            return;
        
        if (dish != null)
            WaitingDish = collider.gameObject;
    }
}
