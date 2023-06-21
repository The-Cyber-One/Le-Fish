using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBehavior : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private PropositionData proposition;
    [SerializeField] private Dialog introductionDialog, satisfiedDialog, unsatisfiedDialog, ruinedDialog;
    [SerializeField] private string iconName;

    private RecipeData _correctDish;
    private CustomerSpawner _customerSpawner;
    private GameObject _spawnedSpecialIngredient;

    private bool _customerWaiting = false, _specialIngredientSpawned = false;

    // Get the position given in the customer spawner to move the customer around
    public void GetSpawner(CustomerSpawner spawner)
    {
        _customerSpawner = spawner;
    }

    private void Start()
    {
        _correctDish = proposition.Recipes[0];

        StartCoroutine(MoveToOrder());
    }

    public void Update()
    {
        if (_customerWaiting)
        {
            DetectDish();

            if (_customerWaiting && _specialIngredientSpawned && _spawnedSpecialIngredient == null)
            {
                _specialIngredientSpawned = false;
                StopAllCoroutines();
                StartCoroutine(Ruined());
            }
        }

        animator.SetFloat("Velocity", navMeshAgent.velocity.sqrMagnitude / (navMeshAgent.speed * navMeshAgent.speed));
    }

    IEnumerator MoveToOrder()
    {
        navMeshAgent.SetDestination(_customerSpawner.OrderPoint.position);
        yield return IsDoneMoving();
        StartCoroutine(WaitForOrder());
    }

    IEnumerator IsDoneMoving()
    {
        yield return new WaitUntil(() => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
    }

    IEnumerator WaitForOrder()
    {
        if (!proposition.SpecialIngredient || !proposition.SpecialIngredient.IngredientPrefab)
        {
            Debug.Log("Cannot instanciate special ingredient");
            yield break;
        }

        animator.SetTrigger("Give Ingredient");
        yield return new WaitForSeconds(0.5f);
        _specialIngredientSpawned = true;
        _spawnedSpecialIngredient = Instantiate(proposition.SpecialIngredient.IngredientPrefab, _customerSpawner.ingredientSpawn, false);
        _spawnedSpecialIngredient.GetComponent<Rigidbody>().isKinematic = false;
        _customerWaiting = true;

        bool dialogDone = false;
        SpeechBubble.Instance.ShowDialog(introductionDialog, iconName, () => dialogDone = true);
        yield return new WaitUntil(() => dialogDone);

        ConchyAI.Instance.AssignProposition(proposition.Name);
        ConchyAI.Instance.NewProposition(proposition.Recipes);
        ConchyAI.Instance.ToggleProposition(true);
    }

    IEnumerator Ruined()
    {
        ConchyAI.Instance.ToggleProposition(false);
        animator.SetTrigger("Unsatisfied");
        bool dialogDone = false;
        SpeechBubble.Instance.ShowDialog(ruinedDialog, iconName, () => dialogDone = true);
        yield return new WaitUntil(() => dialogDone);
        yield return LeaveRestaurant();
    }

    IEnumerator LeaveRestaurant()
    {
        animator.SetTrigger("Leave");
        navMeshAgent.SetDestination(_customerSpawner.AwayPoint.position);
        yield return IsDoneMoving();
        ConchyAI.Instance.ToggleProposition(false);
        Destroy(gameObject);
        _customerSpawner.SpawnCustomers();
    }

    private void DetectDish()
    {
        if (_customerSpawner.FinishedDish == null)
            return;

        _customerWaiting = false;
        StopCoroutine(WaitForOrder());
        SpeechBubble.Instance.StopAllCoroutines();
        StartCoroutine(EatDish(MatchDish(_customerSpawner.FinishedDish.Data)));
    }

    private bool MatchDish(RecipeData receivedDish)
    {
        if (!receivedDish)
            return false;

        if (receivedDish.Ingredients.Count != _correctDish.Ingredients.Count)
            return false;

        _correctDish.Ingredients.OrderBy(ingredients => ingredients.Ingredient.Name).ToList();
        receivedDish.Ingredients.OrderBy(ingredients => ingredients.Ingredient.Name).ToList();

        for (int i = 0; i < _correctDish.Ingredients.Count; i++)
        {
            if (_correctDish.Ingredients[i].Ingredient.Name != receivedDish.Ingredients[i].Ingredient.Name)
                return false;
        }

        return true;
    }

    IEnumerator EatDish(bool satisfied)
    {
        ConchyAI.Instance.ToggleProposition(false);

        if (satisfied)
        {
            animator.SetTrigger("Satisfied");
            bool dialogDone = false;
            SpeechBubble.Instance.ShowDialog(satisfiedDialog, iconName, () => dialogDone = true);
            yield return new WaitUntil(() => dialogDone);
        }
        else
        {
            animator.SetTrigger("Unsatisfied");
            bool dialogDone = false;
            SpeechBubble.Instance.ShowDialog(unsatisfiedDialog, iconName, () => dialogDone = true);
            yield return new WaitUntil(() => dialogDone);
        }
        Destroy(_customerSpawner.FinishedDish.gameObject);
        yield return LeaveRestaurant();

    }
}
