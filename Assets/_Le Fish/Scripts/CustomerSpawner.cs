using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] customers;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] float maxNumberOfCustomers = 8;
    [SerializeField] public Transform orderPoint, awayPoint;
    [SerializeField] public List<Transform> eatPoint = new();
    public List<bool> seatAvailable = new List<bool>();
    private int _customerNumber;

    void Start()
    {
        //Spawns the first customer to start the sequence, if the number of customers is smaller than the max amount of customers
        if(eatPoint != null)
        {
            int i;
            for (i = 0; i < eatPoint.Count; i++)
                seatAvailable.Add(true);

            SpawnCustomers();
        }
    }

    public void SpawnCustomers()
    {
        if (_customerNumber++ < maxNumberOfCustomers)
        {
            CustomerBehavior instance = Instantiate(customers[Random.Range(0, customers.Length)], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
            instance.Setup(this);
        }
    }
}
