using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ConchyAI : MonoBehaviour
{
    [SerializeField] float waypointDistance = 0.1f;
    [SerializeField] float movementSpeed = 0.5f, maxRotationDegree = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] Transform lookPointPlayer;

    [Header("Text")]
    [SerializeField] bool useText = true;
    [SerializeField] Dialog tutorialDialog;

    [Header("Speach")]
    [SerializeField] bool useSpeech = true;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] tutorialSpeechClips;

    [Header("Tutorial")]
    [SerializeField] Transform waypointRoot;
    [SerializeField] Waypoint[] tutorialWaypoints;
    [SerializeField] Transform tutorialEndpoint;

    Transform _waypointRoot;

    [Serializable]
    public class Waypoint
    {
        [SerializeField] public int SpeechIndex, TextIndex;
        [SerializeField] public Transform[] SubWaypoints;
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

    private void Start()
    {
        StartCoroutine(C_Tutorial());
    }

    private IEnumerator C_Tutorial()
    {
        if (useText)
        {
            SpeechBubble.Instance.ShowDialog(tutorialDialog);
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
        bool rotate = animator.GetFloat("Move") != 1;

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
                    animator.SetFloat("Move", isLast ? 1 - t : t);
                }
                yield return null;
            }
        }
    }
}
