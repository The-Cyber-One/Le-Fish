using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] float orderingTime = 5.0f;
    [SerializeField] float spawnCooldown = 1.0f;

    private CustomerSpawner _customerSpawner;
    private RecipeData _proposition;
    private IngredientData _specialIngredient;
    private GameObject _spawnedSpecialIngredient;
    private NavMeshAgent _navMeshAgent;

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

        if (gameObject.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            _navMeshAgent = navMeshAgent;
        }
        else
        {
            Debug.Log("No NavMeshAgent Component !");
            return;
        }

        StartCoroutine(MoveToOrder());
    }

    public void Update()
    {
        if (_customerWaiting)
            DetectDish();
    }

    public RecipeData AssociateRandomRecipe()
    {
        PropositionData _proposition = 0 /*UnityEngine.Random.Range(0, 4)*/ switch // Only beef is implemented
        {
            0 => Resources.Load<PropositionData>("Propositions/BeefPropositions"),
            1 => Resources.Load<PropositionData>("Propositions/CarrotDogPropositions"),
            2 => Resources.Load<PropositionData>("Propositions/KaripapPropositions"),
            3 => Resources.Load<PropositionData>("Propositions/KatsuPropositions"),
            4 => Resources.Load<PropositionData>("Propositions/PennePropositions"),
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
        _navMeshAgent.SetDestination(_customerSpawner.OrderPoint.position);
        yield return IsDoneMoving();
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

        yield return TellStory();
        ConchyAI.Instance.ShowProposition();
        _customerWaiting = true;
        yield return new WaitForSeconds(orderingTime);

        _isSatisfied = false;
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
            List<int> OnlyAvailableSeats = new();

            for(int i = 0; i < _customerSpawner.AvailableSeats.Count(); i++)
            {
                if (_customerSpawner.AvailableSeats[i])
                    OnlyAvailableSeats.Add(i);
            }

            int randIndex = UnityEngine.Random.Range(0, OnlyAvailableSeats.Count);
            int randomNumber = OnlyAvailableSeats[randIndex];

            _navMeshAgent.SetDestination(_customerSpawner.EatPoints[randomNumber].position);
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
