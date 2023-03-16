using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] customers;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] float maxNumberOfCustomers = 8;
    [SerializeField] Transform orderPoint, customerDonePoint;
    private int _customerNumber;

    void Start()
    {
        //Spawns the first customer to start the sequence, if the number of customers is smaller than the max amount of customers
        SpawnCustomers();
    }

    public void SpawnCustomers()
    {
        if (_customerNumber++ < maxNumberOfCustomers)
        {
            CustomerBehavior instance = Instantiate(customers[Random.Range(0, customers.Length)], spawnPoint.transform.position, Quaternion.identity).GetComponent<CustomerBehavior>();
            instance.Setup(orderPoint, customerDonePoint, this);
        }
    }
}
