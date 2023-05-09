using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float orderingTime = 5.0f;
    [SerializeField] float spawnCooldown = 1.0f;
    [SerializeField] bool doneEating;

    private CustomerSpawner _customerSpawner;
    private RecipeData _proposition;
    private IngredientData _specialIngredient;
    private GameObject _spawnedSpecialIngredient;
    private NavMeshAgent _navMeshAgent;

    private CustomerType _randomCustomerType;
    private bool _customerWaiting = false;
    private bool _isSatisfied;

    public enum CustomerType
    {
        STRICT,
        HAPPY,
        MELANCHOLIC,
        FUNNY,
        CLASSIC
    }

    // Get the position given in the customer spawner to move the customer around
    public void GetSpawner(CustomerSpawner spawner)
    {
        _customerSpawner = spawner;
    }

    private void Start()
    {
        // Initialization
        Array values = Enum.GetValues(typeof(CustomerType));
        _randomCustomerType = (CustomerType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        _proposition = AssociateRandomRecipe();
        // serialize
        //_orderingTime = UnityEngine.Random.Range(40.0f, 80.0f);

        if (gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent navMeshAgent))
        {
            _navMeshAgent = navMeshAgent;
        }
        else
        {
            Debug.Log("No NavMeshAgent Component !");
            return;
        }

        Debug.Log("Going to order");
        StartCoroutine(MoveToOrder());
    }

    public void Update()
    {
        // Probably can be optimized
        if (_customerWaiting)
            DetectDish();
    }

    public RecipeData AssociateRandomRecipe()
    {
        PropositionData _proposition = UnityEngine.Random.Range(0, 4) switch
        {
            0 => Resources.Load<PropositionData>("Propositions/BeefPropositions"),
            1 => Resources.Load<PropositionData>("Propositions/CarrotDogPropositions"),
            2 => Resources.Load<PropositionData>("Propositions/KaripapPropositions"),
            3 => Resources.Load<PropositionData>("Propositions/KatsuPropositions"),
            4 => Resources.Load<PropositionData>("Propositions/PennePropositions"),
            _ => throw new NotImplementedException()
        };

        // Assign at the same time which special ingredient we will instantiate
        _specialIngredient = _proposition.SpecialIngredient;

        RecipeData _dish = new();
        int j = UnityEngine.Random.Range(0, 2);
        switch (j)
        {
            case 0:
                _dish.Name = _proposition.Recipes[0].Name;
                return _dish;

            case 1:
                _dish.Name = _proposition.Recipes[1].Name;
                return _dish;

            case 2:
                _dish.Name = _proposition.Recipes[2].Name;
                return _dish;

            default:
                return null;
        }
    }

    IEnumerator MoveToOrder()
    {
        _navMeshAgent.SetDestination(_customerSpawner.OrderPoint.position);
        yield return IsDoneMoving();
        Debug.Log("Waiting for order");
        StartCoroutine(WaitForOrder());
    }

    IEnumerator IsDoneMoving()
    {
        yield return null;
        yield return new WaitUntil(() => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance);
    }

    IEnumerator WaitForOrder()
    {
        if (!_specialIngredient || !_specialIngredient.IngredientPrefab)
        {
            Debug.Log("Cannot instanciate special ingredient");
            yield break;
        }

        _spawnedSpecialIngredient = Instantiate(_specialIngredient.IngredientPrefab, _customerSpawner.ingredientSpawn, false);

        TellStory();
        _customerWaiting = true;
        yield return new WaitForSeconds(orderingTime);

        _isSatisfied = false;
        // Do something when he leaves the restaurant
        Debug.Log("Too Late !!!");

        if (_spawnedSpecialIngredient)
        {
            Destroy(_spawnedSpecialIngredient);
        }

        _navMeshAgent.SetDestination(_customerSpawner.AwayPoint.position);
        yield return LeaveRestaurant();

        _customerSpawner.SpawnCustomers();
    }

    IEnumerator LeaveRestaurant()
    {
        yield return IsDoneMoving();
        Destroy(gameObject);
    }

    public void DetectDish()
    {
        if (_customerSpawner.WaitingDish == null)
            return;

        Debug.Log("dish detected");

        StopCoroutine(nameof(WaitForOrder));
        _customerWaiting = false;

        if (MatchDish(_customerSpawner.WaitingDish.GetComponent<RecipeData>()))
        {
            // change
            int randomNumber;
            do
                randomNumber = UnityEngine.Random.Range(0, _customerSpawner.EatPoints.Count);
            while (_customerSpawner.AvailableSeats[randomNumber] == false);

            _navMeshAgent.SetDestination(_customerSpawner.EatPoints[randomNumber].position);
            Destroy(_customerSpawner.WaitingDish);
            StartCoroutine(EatingTable());
        }
        else
        {
            // Not happy, leave
            StartCoroutine(LeaveRestaurant());
            Destroy(_customerSpawner.WaitingDish);
        }
    }

    public bool MatchDish(RecipeData receivedDish)
    {
        if (!receivedDish)
            return false;

        if (receivedDish.Ingredients.Count != _proposition.Ingredients.Count)
            return false;

        _proposition.Ingredients.OrderBy(ingredients => ingredients.Ingredient.Name).ToList();
        receivedDish.Ingredients.OrderBy(ingredients => ingredients.Ingredient.Name).ToList();

        for (int i = 0; i < _proposition.Ingredients.Count; i++)
        {
            if (_proposition.Ingredients[i].Ingredient.Name != receivedDish.Ingredients[i].Ingredient.Name)
                return false;
        }

        return true;
    }

    public void TellStory()
    {
        switch (_proposition.Name)
        {
            case "BeefOne":
                break;

            case "BeefTwo":
                break;

            case "BeefThree":
                break;

            case "CarrotDogsOne":
                break;

            case "CarrotDogsTwo":
                break;

            case "CarrotDogsThree":
                break;

            case "KaripapOne":
                break;

            case "KaripapTwo":
                break;

            case "KaripapThree":
                break;

            case "KatsuOne":
                break;

            case "KatsuTwo":
                break;

            case "KatsuThree":
                break;

            case "PenneOne":
                break;

            case "PenneTwo":
                break;

            case "PenneThree":
                break;
        }
    }

    IEnumerator EatingTable()
    {
        _customerSpawner.SpawnCustomers();
        yield return new WaitForSeconds(UnityEngine.Random.Range(20.0f, 40.0f));
        yield return LeaveRestaurant();
    }

    //public void CheckReadytoGo()
    //{
    //    if (!_isSatisfied)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.awayPoint.transform.position, speed * Time.deltaTime);
    //        if (transform.position == _customerSpawner.awayPoint.transform.position)
    //        {
    //            _currentState = CustomerState.DESPAWNING;
    //            StartCoroutine(SpawnCooldown());
    //        }
    //        return;
    //    }

    //    int i = 0;
    //    bool endLoop = false;
    //    bool seatFull = false;

    //    if (_customerSpawner.eatPoint != null)
    //    {
    //        while (endLoop == false && _customerSpawner.seatAvailable[i] == false)
    //        {
    //            i++;
    //            if (i >= _customerSpawner.seatAvailable.Count)
    //            {
    //                endLoop = true;
    //                seatFull = true;
    //                i = 0;
    //            }
    //        }
    //    }

    //    if (seatFull)
    //    {
    //        //TODO: Maybe order another customer to be done and take that seat instead of moving outside

    //        transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.awayPoint.transform.position, speed * Time.deltaTime);
    //        if (transform.position == _customerSpawner.awayPoint.transform.position)
    //        {
    //            _currentState = CustomerState.DESPAWNING;
    //            StartCoroutine(SpawnCooldown());
    //        }
    //    }
    //    else
    //    {
    //        do
    //        {
    //            i = UnityEngine.Random.Range(0, _customerSpawner.seatAvailable.Count - 1);
    //        }
    //        while (_customerSpawner.seatAvailable[i] == false);

    //        transform.position = Vector3.MoveTowards(transform.position, _customerSpawner.eatPoint[i].transform.position, speed * Time.deltaTime);
    //        if (transform.position == _customerSpawner.eatPoint[i].transform.position)
    //        {
    //            _customerSpawner.seatAvailable[i] = false;
    //            _currentState = CustomerState.EATING;
    //            StartCoroutine(Eating(i));
    //        }
    //    }
    //}

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        _customerSpawner.SpawnCustomers();
        Destroy(gameObject);
    }

    IEnumerator Eating(int index)
    {
        _customerSpawner.SpawnCustomers();
        yield return new WaitForSeconds(orderingTime);
        _customerSpawner.AvailableSeats[index] = true;
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

    //public void CheckProposedRecipe(Collider collider)
    //{
    //    RecipeData dishRecipe = collider.GetComponent<RecipeData>();
    //    receivedDish = false;

    //    if (dishRecipe.Name == _proposition.Name)
    //        _isSatisfied = true;

    //    else _isSatisfied = false;

    //    _currentState = CustomerState.READYTOSEAT;
    //}
}
