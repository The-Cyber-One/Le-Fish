using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] float orderingTime = 5.0f;
    [SerializeField] float spawnCooldown = 1.0f;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;

    private CustomerSpawner _customerSpawner;
    private RecipeData _proposition;
    private IngredientData _specialIngredient;
    private GameObject _spawnedSpecialIngredient;

    private bool _customerWaiting = false;
    private bool _isSatisfied;

    // Get the position given in the customer spawner to move the customer around
    public void GetSpawner(CustomerSpawner spawner)
    {
        _customerSpawner = spawner;
    }

    private void Start()
    {
        _proposition = AssociateRandomRecipe();

        StartCoroutine(MoveToOrder());
    }

    public void Update()
    {
        if (_customerWaiting)
        {
            DetectDish();
            if (_spawnedSpecialIngredient == null)
            {
                StopCoroutine(WaitForOrder());
                StartCoroutine(LeaveRestaurant());
            }
        }

        animator.SetFloat("Velocity", navMeshAgent.velocity.sqrMagnitude / (navMeshAgent.speed * navMeshAgent.speed));
    }

    public RecipeData AssociateRandomRecipe()
    {
        PropositionData _proposition = UnityEngine.Random.Range(0, 3) switch // Only beef is implemented
        {
            0 => Resources.Load<PropositionData>("Propositions/BeefPropositions"),
            1 => Resources.Load<PropositionData>("Propositions/KaripapPropositions"),
            2 => Resources.Load<PropositionData>("Propositions/PennePropositions"),
            _ => throw new NotImplementedException()
        };

        ConchyAI.Instance.NewProposition(_proposition.Recipes);

        // Assign at the same time which special ingredient we will instantiate
        _specialIngredient = _proposition.SpecialIngredient;

        RecipeData _dish = new()
        {
            Name = _proposition.Recipes[UnityEngine.Random.Range(0, 2)].Name
        };
        return _dish;
    }

    IEnumerator MoveToOrder()
    {
        navMeshAgent.SetDestination(_customerSpawner.OrderPoint.position);
        yield return IsDoneMoving();
        StartCoroutine(WaitForOrder());
    }

    IEnumerator IsDoneMoving()
    {
        yield return null;
        yield return new WaitUntil(() => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
    }

    IEnumerator WaitForOrder()
    {
        if (!_specialIngredient || !_specialIngredient.IngredientPrefab)
        {
            Debug.Log("Cannot instanciate special ingredient");
            yield break;
        }

        _spawnedSpecialIngredient = Instantiate(_specialIngredient.IngredientPrefab, _customerSpawner.ingredientSpawn, false);
        _spawnedSpecialIngredient.GetComponent<Rigidbody>().isKinematic = false;
        _customerWaiting = true;

        yield return TellStory();
        ConchyAI.Instance.ShowProposition();
        yield return new WaitForSeconds(orderingTime);

        _isSatisfied = false;
        Debug.Log("Too Late !!!");

        if (_spawnedSpecialIngredient)
        {
            Destroy(_spawnedSpecialIngredient);
        }

        navMeshAgent.SetDestination(_customerSpawner.AwayPoint.position);
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
            List<int> OnlyAvailableSeats = new();

            for (int i = 0; i < _customerSpawner.AvailableSeats.Count(); i++)
            {
                if (_customerSpawner.AvailableSeats[i])
                    OnlyAvailableSeats.Add(i);
            }

            int randIndex = UnityEngine.Random.Range(0, OnlyAvailableSeats.Count);
            int randomNumber = OnlyAvailableSeats[randIndex];

            navMeshAgent.SetDestination(_customerSpawner.EatPoints[randomNumber].position);
            _customerSpawner.AvailableSeats[randomNumber] = false;

            Destroy(_customerSpawner.WaitingDish);
            StartCoroutine(EatingTable());
        }
        else
        {
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

    IEnumerator TellStory()
    {
        yield return new WaitForSeconds(1);
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
}
