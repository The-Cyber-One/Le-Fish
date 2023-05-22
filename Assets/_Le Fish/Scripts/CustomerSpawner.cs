using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : Singleton<CustomerSpawner>
{
    public List<Transform> EatPoints = new();
    public Transform ingredientSpawn;
    [HideInInspector] public List<bool> AvailableSeats = new();
    [HideInInspector] public GameObject WaitingDish;
    public Transform OrderPoint, AwayPoint;
    public PropositionHologram[] PropositionHolograms;

    [SerializeField] Transform spawnPoint;
    [SerializeField] int maxNumberOfCustomers = 8;
    [SerializeField] GameObject[] customers;

    int _customerNumber;

    [System.Serializable]
    public class PropositionHologram
    {
        [SerializeField] public TextMeshProUGUI Title, Description, Instructions;
    }

    void Start()
    {
        //Spawns the first customer to start the sequence, if the number of customers is smaller than the max amount of customers
        for (int i = 0; i < EatPoints.Count; i++)
            AvailableSeats.Add(true);
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
            WaitingDish = collider.gameObject;
    }
}
