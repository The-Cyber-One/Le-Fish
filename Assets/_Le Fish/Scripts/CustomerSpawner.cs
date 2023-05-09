using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] customers;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] float maxNumberOfCustomers = 8;
    [SerializeField] public Transform orderPoint, awayPoint;
    [SerializeField] public List<Transform> eatPoint = new();
    [SerializeField] public Transform ingredientSpawn;
    public List<bool> seatAvailable = new();
    public GameObject waitingDish;
    private int _customerNumber;

    void Start()
    {
        //Spawns the first customer to start the sequence, if the number of customers is smaller than the max amount of customers
        int i;
        for (i = 0; i < eatPoint.Count; i++)
            seatAvailable.Add(true);

        SpawnCustomers();
    }

    public void SpawnCustomers()
    {
        if (_customerNumber++ < maxNumberOfCustomers)
        {
            int random = Random.Range(0, customers.Length);
            CustomerBehavior instance = Instantiate(customers[random], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
            instance.gameObject.AddComponent<NavMeshAgent>();
            instance.GetSpawner(this);
        }
    }

    public void UpdateDish(Collider collider)
    {
        if (!collider.TryGetComponent(out RecipeData dish))
            return;
        
        if (dish != null)
            waitingDish = collider.gameObject;
    }
}
