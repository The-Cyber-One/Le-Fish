using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomerBehavior;

public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float orderingTime = 1.0f;
    [SerializeField] float spawnCooldown = 1.0f;
    [SerializeField] bool doneEating;
    private CustomerSpawner _customerSpawner;
    private Recipe _proposition;

    private CustomerType _randomCustomerType;
    private bool _isSatisfied = true;
    private bool WaitingDish = true;
    private float eatingTime = 40.0f;

    // Define the possible states for the customer.
    public enum CustomerState
    {
        Spawned,
        Ordering,
        ReadyToGo,
        Eating,
        Despawning
    }

    public enum CustomerType
    {
        STRICT,
        HAPPY,
        MELANCHOLIC,
        FUNNY,
        CLASSIC
    }

    // The current state of the customer.
    private CustomerState _currentState = CustomerState.Spawned;
    //Gets the position given in the customer spawner to move the customer around
    public void Setup(CustomerSpawner spawner)
    {
        _customerSpawner = spawner;
    }

    private void Start()
    {
        // Give a random type to the customer : Melancholic, happy...
        Array values = Enum.GetValues(typeof(CustomerType));
        _randomCustomerType = (CustomerType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        _proposition = AssociateRandomRecipe();
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
    //If the customer is spawned 
    public void CheckIfSpawned()
    {
        transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.orderPoint.position, speed * Time.deltaTime);
        if (transform.position == _customerSpawner.orderPoint.position)
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
        if (_isSatisfied)
        {
            int i = 0;
            bool endLoop = false;
            bool seatFull = false;

            if (_customerSpawner.eatPoint != null)
                while (endLoop == false && _customerSpawner.seatAvailable[i] == false)
                {
                    i++;
                    if (i >= _customerSpawner.seatAvailable.Count)
                    {
                        endLoop = true;
                        seatFull = true;
                        i = 0;
                    }
                }

            if (seatFull == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.awayPoint.transform.position, speed * Time.deltaTime);
                if (transform.position == _customerSpawner.awayPoint.transform.position)
                {
                    _currentState = CustomerState.Despawning;
                    StartCoroutine(SpawnCooldown());
                }
            }

            else
            {
                do
                {
                    i = UnityEngine.Random.Range(0, _customerSpawner.seatAvailable.Count - 1);
                }
                while (_customerSpawner.seatAvailable[i] == false);


                transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.eatPoint[i].transform.position, speed * Time.deltaTime);
                if (transform.position == _customerSpawner.eatPoint[i].transform.position)
                {
                    _customerSpawner.seatAvailable[i] = false;
                    _currentState = CustomerState.Eating;
                    StartCoroutine(Eating(i));
                }
            }
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.awayPoint.transform.position, speed * Time.deltaTime);
            if (transform.position == _customerSpawner.awayPoint.transform.position)
            {
                _currentState = CustomerState.Despawning;
                StartCoroutine(SpawnCooldown());
            }
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

    IEnumerator Eating(int index)
    {
        _customerSpawner.SpawnCustomers();
        yield return new WaitForSeconds(eatingTime);
        _customerSpawner.seatAvailable[index] = true;
        Destroy(gameObject);
    }

    public void CustomerReaction(CustomerType customerType)
    {
        switch (customerType)
        {
            case CustomerType.STRICT:

                break;

            case CustomerType.MELANCHOLIC:

                break;

            case CustomerType.FUNNY:

                break;

            case CustomerType.HAPPY:

                break;

            case CustomerType.CLASSIC:

                break;

            default:
                Debug.Log("No Customer Type defined");
                break;
        }
    }

    public Recipe AssociateRandomRecipe()
    {
        int i = UnityEngine.Random.Range(0, 4);
        int j = UnityEngine.Random.Range(0, 2);

        Propositions _proposition = new();
        Recipe _recipe = new();

        switch (i)
        {
            case 0:
                _proposition = Resources.Load<Propositions>("Propositions/BeefPropositions");
                break;

            case 1:
                _proposition = Resources.Load<Propositions>("Propositions/CarrotDogPropositions");
                break;

            case 2:
                _proposition = Resources.Load<Propositions>("Propositions/KaripapPropositions");
                break;

            case 3:
                _proposition = Resources.Load<Propositions>("Propositions/KatsuPropositions");
                break;

            case 4:
                _proposition = Resources.Load<Propositions>("Propositions/PennePropositions");
                break;

            default:
                break;
        };

        switch (j)
        {
            case 0:
                _recipe.Name = _proposition.Recipes[0].Name;
                return _recipe;

            case 1:
                _recipe.Name = _proposition.Recipes[1].Name;
                return _recipe;

            case 2:
                _recipe.Name = _proposition.Recipes[2].Name;
                return _recipe;

            default:
                return null;
        }
    }

    public IngredientData GenerateSpecialIngredient(int index)
    {
        return index switch
        {
            0 => Resources.Load<IngredientData>("IngredientData/SpecialIngredients/CoteBoeuf"),
            1 => Resources.Load<IngredientData>("IngredientData/SpecialIngredients/Bread"),
            2 => Resources.Load<IngredientData>("IngredientData/SpecialIngredients/SweetPotato"),
            3 => Resources.Load<IngredientData>("IngredientData/SpecialIngredients/Mirin"),
            4 => Resources.Load<IngredientData>("IngredientData/SpecialIngredients/Pasta"),
            _ => null,
        };
    }

    public void CheckProposedRecipe(Collider collider)
    {
        Recipe dishRecipe = collider.GetComponent<Recipe>();
        WaitingDish = false;

        if (dishRecipe.Name == _proposition.Name)            
            _isSatisfied = true;
        
        else _isSatisfied = false;

        _currentState = CustomerState.ReadyToGo;
    }
}
