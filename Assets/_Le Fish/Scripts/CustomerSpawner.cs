using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerSpawner : Singleton<CustomerSpawner>
{
    [HideInInspector] public DishData FinishedDish;

    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] customers;

    public Transform ingredientSpawn;
    public Transform OrderPoint, AwayPoint;

    private int _customerIndex = 0;

    private void Start()
    {
        customers = customers.OrderBy(_ => Random.value).ToArray<GameObject>();
    }

    [ContextMenu(nameof(SpawnCustomers))]
    public void SpawnCustomers()
    {
        CustomerBehavior instance = Instantiate(customers[_customerIndex], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
        _customerIndex = (_customerIndex + 1) % customers.Length;
        instance.GetSpawner(this);
    }

    public void UpdateDish(DishData dishData) => FinishedDish = dishData;
}
