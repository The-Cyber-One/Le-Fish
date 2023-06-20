using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class ConchyAI : Singleton<ConchyAI>
{
    [SerializeField] float waypointDistance = 0.1f;
    [SerializeField] float movementSpeed = 0.5f, maxRotationDegree = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] Transform lookPointPlayer;
    [SerializeField] ParentConstraint LookConstraint;

    [Header("Proposition")]
    [SerializeField] GameObject[] propositionsContent;
    [SerializeField] GameObject propositionHologramContent;
    [SerializeField] PropositionHologram[] propositionHolograms;
    Canvas[] _propositionCanvas;

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
        LookConstraint.weight = 0;

        // Rotate towards destination
        Vector3 direction = (waypoint.position - transform.position).normalized;
        yield return Rotate(direction, animator.GetFloat("Move") != 1);

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
            yield return Rotate(playerDirection, true);
        }

        IEnumerator Rotate(Vector3 direction, bool animate)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            float startAngle = animate ? Quaternion.Angle(transform.rotation, rotation) : 0;
            while (transform.rotation != rotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, maxRotationDegree);
                if (animate)
                {
                    float t = Mathf.InverseLerp(startAngle, 0, Quaternion.Angle(transform.rotation, rotation));
                    LookConstraint.weight = t;
                    animator.SetFloat("Move", isLast ? 1 - t : t);
                }
                yield return null;
            }
        }

    }

    public void NewProposition(RecipeData[] recipes)
    {
        propositionHologramContent.SetActive(false);

        int[] indecies = Enumerable.Range(0, recipes.Length).OrderBy(i => UnityEngine.Random.value).ToArray();
        for (int i = 0; i < recipes.Length; i++)
        {
            //RecipeData recipe = recipes[indecies[i]];
            //propositionHolograms[i].Title.text = recipes[i].Name;
            //propositionHolograms[i].Description.text = recipes[i].Description;
            //propositionHolograms[i].Instructions = recipes[i].Image;

            //Dialog instructions = recipe.Instructions;
            //StringBuilder stringBuilder = new();
            //for (int j = 0; j < instructions.Length; j++)
            //{
            //    stringBuilder.AppendLine($"{j + 1} - {instructions[j].Content}");
            //}
            //propositionHolograms[i].Instructions.text = stringBuilder.ToString();
        }
    }

    public void ToggleProposition(bool active)
    {
        propositionHologramContent.SetActive(active);
    }

    // Cyril update 
    public void AssignProposition(string propositionName)
    {
        switch (propositionName)
        {
            case "KaripapProposition":
                propositionHologramContent = propositionsContent[0];
                _propositionCanvas = propositionsContent[0].GetComponentsInChildren<Canvas>();
                break;

            case "SteakProposition":
                propositionHologramContent = propositionsContent[1];
                _propositionCanvas = propositionsContent[1].GetComponentsInChildren<Canvas>();
                break;

            case "PastaProposition":
                propositionHologramContent = propositionsContent[2];
                _propositionCanvas = propositionsContent[2].GetComponentsInChildren<Canvas>();
                break;
        }
    }

    // Used by ButtonsProposition prefab 
    public void ShowFirstProposition()
    {
        _propositionCanvas[0].gameObject.SetActive(true);
        _propositionCanvas[0].transform.position = new(0.380f, -0.380f, 0.0f);

        _propositionCanvas[1].gameObject.SetActive(false);
        _propositionCanvas[2].gameObject.SetActive(false);
    }

    public void ShowSecondProposition()
    {
        _propositionCanvas[1].gameObject.SetActive(true);
        _propositionCanvas[1].transform.position = new(0.0f, 0.0f, 0.0f);

        _propositionCanvas[0].gameObject.SetActive(false);
        _propositionCanvas[2].gameObject.SetActive(false);
    }

    public void ShowThirdProposition()
    {
        _propositionCanvas[2].gameObject.SetActive(true);
        _propositionCanvas[2].transform.position = new(-0.380f, 0.380f, 0.0f);

        _propositionCanvas[0].gameObject.SetActive(false);
        _propositionCanvas[1].gameObject.SetActive(false);
    }
}
