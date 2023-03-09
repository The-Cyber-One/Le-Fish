using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customers;
    public GameObject spawnPoint;
    public bool noCustomers;
    [SerializeField] float maxNumberOfCustomers = 8;
    private float customerNumber;

    void Start()
    {
        noCustomers = true;
    }

    public void SpawnCustomers(Vector3 spawnPointPosition)
    {
        if (noCustomers && customerNumber < maxNumberOfCustomers)
        {
            Instantiate(customers[Random.Range(0, customers.Length)], spawnPointPosition, Quaternion.identity);
            customerNumber += 1f;
            noCustomers = false;
        }
    }

    void Update()
    {
        SpawnCustomers(spawnPoint.transform.position);
    }
}
