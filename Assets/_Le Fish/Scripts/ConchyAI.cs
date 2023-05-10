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
    [SerializeField] DialogShower speechBubble;
    [SerializeField] Dialog tutorialDialog;
    [SerializeField] Waypoint[] tutorialTextWaypoints;

    [Header("Speach")]
    [SerializeField] bool useSpeech = true;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] tutorialSpeechClips;
    [SerializeField] Waypoint[] tutorialSpeechWaypoints;

    [Serializable]
    public class Waypoint
    {
        [SerializeField] public int DialogIndex;
        [SerializeField] public Transform[] SubWaypoints;
    }

    private void Start()
    {
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
                yield return C_Move(waypoint.SubWaypoints);
            }
        }
        else
        {
            int waypointIndex = 0;
            for (int i = 0; i < tutorialSpeechClips.Length; i++)
            {
                if (tutorialSpeechWaypoints.Length > 0 && tutorialSpeechWaypoints[waypointIndex].DialogIndex == i)
                    yield return C_Move(tutorialSpeechWaypoints[waypointIndex++].SubWaypoints);

                audioSource.clip = tutorialSpeechClips[i];
                audioSource.Play();
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
        }
    }

    private IEnumerator C_Move(Transform[] subWaypoints)
    {
        foreach (Transform subWaypoint in subWaypoints)
        {
            // Move towards destination
            Vector3 waypointDirection = (subWaypoint.position - transform.position).normalized;
            Quaternion waypointRotation = Quaternion.LookRotation(waypointDirection);
            bool inRange;
            animator.SetFloat("Move", 1);
            do
            {
                float distance = Vector3.Distance(transform.position, subWaypoint.position);
                inRange = distance <= waypointDistance;
                transform.position += waypointDirection * (inRange ? distance : movementSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, waypointRotation, maxRotationDegree);
                yield return null;
            }
            while (!inRange);
            animator.SetFloat("Move", 0);
        }

        // Rotate towards player
        Vector3 playerDirection = lookPointPlayer.position - transform.position;
        playerDirection.y = 0;
        Quaternion playerRotation = Quaternion.LookRotation(playerDirection);
        while (transform.rotation != playerRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, maxRotationDegree);
            yield return null;
        }
    }
}
