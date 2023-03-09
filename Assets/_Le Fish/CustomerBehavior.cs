using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerBehavior : MonoBehaviour
{
    public Transform orderPoint, customerDonePoint;
    private CustomerSpawner customerSpawner;
    private Vector3 customerPosition, orderPointPosition, customerDonePosition;
    public float speed = 1f;
    public float orderingTime = 6;
    public float spawncooldown = 3;
   [SerializeField] public bool doneEating;

    // Define the possible states for the customer.
    public enum CustomerState
    {
        Spawned,
        Ordering,
        ReadyToGo,
        Leaving
    }

    // The current state of the customer.
    private CustomerState currentState = CustomerState.Spawned;

    // Start is called before the first frame update
    void Start()
    {
        orderPoint = GameObject.Find("OrderPoint").transform;
        customerDonePoint = GameObject.Find("DoneEatingPoint").transform;
        customerSpawner = GameObject.Find("CustomerSpawner").GetComponent<CustomerSpawner>();
        SetPositions(orderPoint.transform.position, customerDonePoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
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

    public void SetPositions(Vector3 orderPointPos, Vector3 customerDonePos)
    {
        orderPointPosition = orderPointPos;
        customerDonePosition = customerDonePos;
    }

    public void CheckIfSpawned()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, orderPointPosition, speed * Time.deltaTime);
        if (gameObject.transform.position == orderPoint.transform.position)
        {
            currentState = CustomerState.Ordering;
            StartCoroutine(Ordering());
        }
    }

    public void CheckIfDishReady()
    {
        //Check if the customer gets a dish
        if (doneEating)
        {
            currentState = CustomerState.ReadyToGo;
        }
    }

    public void CheckReadytoGo()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, customerDonePosition, speed * Time.deltaTime);
        if (gameObject.transform.position == customerDonePosition)
        {
            StartCoroutine(SpawnCooldown());
        }
    }

    IEnumerator Ordering()
    {
        yield return new WaitForSeconds(orderingTime);
        currentState = CustomerState.ReadyToGo;
    }
    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawncooldown);
        customerSpawner.noCustomers = true;
        Destroy(gameObject);
    }
}
