using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerSpawner : Singleton<CustomerSpawner>
{
    [HideInInspector] public GameObject WaitingDish;

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
        _customerIndex = _customerIndex + 1 % customers.Length;
        CustomerBehavior instance = Instantiate(customers[_customerIndex], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
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
