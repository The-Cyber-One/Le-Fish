using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class ConchyAI : Singleton<ConchyAI>
{
    [SerializeField] float waypointDistance = 0.1f;
    [SerializeField] float movementSpeed = 0.5f, maxRotationDegree = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] Transform lookPointPlayer;
    [SerializeField] MultiAimConstraint LookConstraint;

    [Header("Proposition")]
    [SerializeField] Animator propositionHologramAnimator;
    [SerializeField] GameObject buttons;
    [SerializeField] PropositionHologram[] propositionHolograms;

    private RecipeData[] _currentProposition;
    private int _currentPropositionIndex = 0;
    private int[] _propositionRandomIndecies;

    [Header("Text")]
    [SerializeField] bool useText = true;
    [SerializeField] Dialog tutorialDialog;

    [Header("Speach")]
    [SerializeField] bool useSpeech = true;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] tutorialSpeechClips;

    [Header("Tutorial")]
    [SerializeField] ElevatorScript elevator;
    [SerializeField] Transform waypointRoot;
    [SerializeField] Waypoint[] tutorialWaypoints;
    [SerializeField] Transform tutorialEndpoint;
    bool _tutorialAvailible = true;

    Transform _waypointRoot;

    public List<RecipeData> MergableRecipes
    {
        get
        {
            if (_currentProposition != null && _currentProposition.Length > 0)
                return new List<RecipeData>() { _currentProposition[_currentPropositionIndex] };

            return Resources.Load<ListRecipeData>("ListRecipes").ListRecipes;
        }
    }

    [Serializable]
    public class Waypoint
    {
        [SerializeField] public int SpeechIndex, TextIndex;
        [SerializeField] public Transform[] SubWaypoints;
    }

    [Serializable]
    public class PropositionHologram
    {
        [SerializeField] public TextMeshProUGUI Title, Description;
        [SerializeField] public Image Instructions;
    }

    [ContextMenu(nameof(UpdateWaypoints))]
    private void UpdateWaypoints()
    {
        if (_waypointRoot != waypointRoot)
        {
            _waypointRoot = waypointRoot;
            tutorialWaypoints = new Waypoint[waypointRoot.childCount];
            for (int i = 0; i < waypointRoot.childCount; i++)
            {
                tutorialWaypoints[i] = new Waypoint { SubWaypoints = waypointRoot.GetChild(i).GetComponentsInChildren<Transform>().Skip(1).ToArray() };
            }
        }
    }

    [ContextMenu(nameof(Tutorial))]
    public void Tutorial()
    {
        if (_tutorialAvailible)
        {
            _tutorialAvailible = false;
            StartCoroutine(C_Tutorial());
        }
    }

    private IEnumerator C_Tutorial()
    {
        yield return null;
        yield return new WaitUntil(() => elevator.NotMoving);
        if (useText)
        {
            SpeechBubble.Instance.ShowDialog(tutorialDialog, "Conchy");
            foreach (Waypoint waypoint in tutorialWaypoints)
            {
                yield return new WaitUntil(() => SpeechBubble.Instance.DialogIndex == waypoint.TextIndex);
                int subWaypointAmount = waypoint.SubWaypoints.Length;
                for (int i = 0; i < waypoint.SubWaypoints.Length; i++)
                    yield return C_Move(waypoint.SubWaypoints[i], subWaypointAmount - i == 1);
                SpeechBubble.Instance.PlayNextText();
            }
            yield return new WaitUntil(() => SpeechBubble.Instance.DialogIndex == tutorialDialog.Length);
        }
        else if (useSpeech)
        {
            int waypointIndex = 0;
            for (int i = 0; i < tutorialSpeechClips.Length; i++)
            {
                if (tutorialWaypoints.Length > 0 && waypointIndex < tutorialWaypoints.Length && tutorialWaypoints[waypointIndex].SpeechIndex == i)
                {
                    int subWaypointAmount = tutorialWaypoints[waypointIndex].SubWaypoints.Length;
                    for (int j = 0; j < subWaypointAmount; j++)
                        yield return C_Move(tutorialWaypoints[waypointIndex].SubWaypoints[j], subWaypointAmount - j == 1);
                    waypointIndex++;
                }

                audioSource.clip = tutorialSpeechClips[i];
                audioSource.Play();
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
        }

        yield return C_Move(tutorialEndpoint, true);

        CustomerSpawner.Instance.SpawnCustomers();
    }

    private IEnumerator C_Move(Transform waypoint, bool isLast = true)
    {
        // Rotate towards destination
        Vector3 direction = (waypoint.position - transform.position).normalized;
        yield return Rotate(direction, animator.GetFloat("Move") != 1, false);

        // Move towards destination
        bool inRange;
        do
        {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            inRange = distance <= waypointDistance;
            transform.position += direction * (inRange ? distance : movementSpeed);
            yield return null;
        }
        while (!inRange);

        if (isLast)
        {
            // Rotate towards player
            Vector3 playerDirection = lookPointPlayer.position - transform.position;
            playerDirection.y = 0;
            yield return Rotate(playerDirection, true, true);
        }

        IEnumerator Rotate(Vector3 direction, bool animate, bool finish)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            float startAngle = animate ? Quaternion.Angle(transform.rotation, rotation) : 0;
            while (transform.rotation != rotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, maxRotationDegree);
                if (animate)
                {
                    float t = Mathf.InverseLerp(startAngle, 0, Quaternion.Angle(transform.rotation, rotation));
                    LookConstraint.weight = finish ? t : 1 - t;
                    animator.SetFloat("Move", isLast ? 1 - t : t);
                }
                yield return null;
            }
        }

    }

    public void NewProposition(RecipeData[] recipes)
    {
        ToggleProposition(false);

        _propositionRandomIndecies = Enumerable.Range(0, recipes.Length).OrderBy(i => UnityEngine.Random.value).ToArray();
        for (int i = 0; i < recipes.Length; i++)
        {
            RecipeData recipe = recipes[_propositionRandomIndecies[i]];
            propositionHolograms[i].Title.text = recipes[i].Name;
            propositionHolograms[i].Title.transform.parent.gameObject.SetActive(true);
            propositionHolograms[i].Description.text = recipes[i].Description;
            propositionHolograms[i].Instructions.sprite = recipes[i].Sprite;
            propositionHolograms[i].Instructions.gameObject.SetActive(false);
        }

        _currentProposition = recipes;
    }

    public void ToggleProposition(bool active)
    {
        propositionHologramAnimator.SetBool("Active", active);
        if (active)
            buttons.SetActive(false);
    }

    public void ShowRecipe(int index)
    {
        for (int i = 0; i < propositionHolograms.Length; i++)
        {
            propositionHolograms[i].Title.transform.parent.gameObject.SetActive(false);
        }
        propositionHolograms[index].Instructions.gameObject.SetActive(true);

        _currentPropositionIndex = _propositionRandomIndecies[index];
    }
}
