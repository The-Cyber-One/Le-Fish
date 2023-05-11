using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ConchyAI : MonoBehaviour
{
    [SerializeField] float waypointDistance = 0.1f;
    [SerializeField] float movementSpeed = 0.5f, maxRotationDegree = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] Transform lookPointPlayer;

    [Header("Text")]
    [SerializeField] bool useText = true;
    [SerializeField] SpeechBubble speechBubble;
    [SerializeField] Dialog tutorialDialog;
    [SerializeField] Waypoint[] tutorialTextWaypoints;

    [Header("Speach")]
    [SerializeField] bool useSpeech = true;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] tutorialSpeechClips;
    [SerializeField] Waypoint[] tutorialSpeechWaypoints;

    [SerializeField] Transform tutorialEndpoint;

    [Serializable]
    public class Waypoint
    {
        [SerializeField] public int DialogIndex;
        [SerializeField] public Transform[] SubWaypoints;
    }

    private void Start()
    {
        speechBubble.
        StartCoroutine(C_Tutorial());
    }

    private IEnumerator C_Tutorial()
    {
        if (useText)
        {
            speechBubble.ShowDialog(tutorialDialog);
            foreach (Waypoint waypoint in tutorialTextWaypoints)
            {
                yield return new WaitUntil(() => speechBubble.DialogIndex == waypoint.DialogIndex);
                for (int i = 0; i < waypoint.SubWaypoints.Length; i++)
                    yield return C_Move(waypoint.SubWaypoints[i]);
            }
        }
        else
        {
            int waypointIndex = 0;
            for (int i = 0; i < tutorialSpeechClips.Length; i++)
            {
                if (tutorialSpeechWaypoints.Length > 0 && waypointIndex < tutorialSpeechWaypoints.Length && tutorialSpeechWaypoints[waypointIndex].DialogIndex == i)
                {
                    for (int j = 0; j < tutorialSpeechWaypoints[waypointIndex].SubWaypoints.Length; j++)
                        yield return C_Move(tutorialSpeechWaypoints[waypointIndex].SubWaypoints[i]);
                    waypointIndex++;
                }

                audioSource.clip = tutorialSpeechClips[i];
                audioSource.Play();
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
        }

        yield return C_Move(tutorialEndpoint, true);
    }

    private IEnumerator C_Move(Transform waypoint, bool isLast = false)
    {
        bool rotate = animator.GetFloat("Move") != 1;

        // Rotate towards destination
        Vector3 waypointDirection = (waypoint.position - transform.position).normalized;
        Quaternion waypointRotation = Quaternion.LookRotation(waypointDirection);
        float startAngle = rotate ? Quaternion.Angle(transform.rotation, waypointRotation) : 0;
        while (transform.rotation != waypointRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, waypointRotation, maxRotationDegree);
            if (rotate)
            {
                float t = Mathf.InverseLerp(startAngle, 0, Quaternion.Angle(transform.rotation, waypointRotation));
                animator.SetFloat("Move", t);
            }
            yield return null;
        }

        // Move towards destination
        bool inRange;
        do
        {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            inRange = distance <= waypointDistance;
            transform.position += waypointDirection * (inRange ? distance : movementSpeed);
            yield return null;
        }
        while (!inRange);

        if (isLast)
        {
            // Rotate towards player
            Vector3 playerDirection = lookPointPlayer.position - transform.position;
            playerDirection.y = 0;
            Quaternion playerRotation = Quaternion.LookRotation(playerDirection);
            while (transform.rotation != playerRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, maxRotationDegree);
                yield return null;
            }

            animator.SetFloat("Move", 0.1f);
            yield return null;
            animator.SetFloat("Move", 0);
        }
    }
}
