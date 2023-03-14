using System.Collections;
using UnityEngine;

public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float orderingTime = 6;
    [SerializeField] float spawnCooldown = 3;
    [SerializeField] bool doneEating;
    private Transform _orderPoint, _customerDonePoint;
    private CustomerSpawner _customerSpawner;

    // Define the possible states for the customer.
    public enum CustomerState
    {
        Spawned,
        Ordering,
        ReadyToGo,
        Despawning
    }

    // The current state of the customer.
    private CustomerState _currentState = CustomerState.Spawned;

    public void Setup(Transform orderPoint, Transform donePoint, CustomerSpawner spawner)
    {
        _orderPoint = orderPoint;
        _customerDonePoint = donePoint;
        _customerSpawner = spawner;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case CustomerState.Spawned:
                CheckIfSpawned();
                break;
            case CustomerState.Ordering:
                CheckIfDishReady();
                break;
            case CustomerState.ReadyToGo:
                CheckReadytoGo();
                break;
        }
    }

    public void CheckIfSpawned()
    {
        transform.position = Vector3.MoveTowards(transform.position, _orderPoint.transform.position, speed * Time.deltaTime);
        if (transform.position == _orderPoint.transform.position)
        {
            _currentState = CustomerState.Ordering;
            StartCoroutine(Ordering());
        }
    }

    public void CheckIfDishReady()
    {
        //Check if the customer gets a dish
        if (doneEating)
        {
            _currentState = CustomerState.ReadyToGo;
        }
    }

    public void CheckReadytoGo()
    {
        transform.position = Vector3.MoveTowards(transform.position, _customerDonePoint.transform.position, speed * Time.deltaTime);
        if (transform.position == _customerDonePoint.transform.position)
        {
            _currentState = CustomerState.Despawning;
            StartCoroutine(SpawnCooldown());
        }
    }

    IEnumerator Ordering()
    {
        yield return new WaitForSeconds(orderingTime);
        _currentState = CustomerState.ReadyToGo;
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        _customerSpawner.SpawnCustomers();
        Destroy(gameObject);
    }
}
