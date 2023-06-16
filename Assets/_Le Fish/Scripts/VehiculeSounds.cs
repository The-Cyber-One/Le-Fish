using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiculeSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float min, max;

    IEnumerator Start()
    {
        AssignClip();

        WaitForSeconds waitForClip = new(audioSource.clip.length);
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(Random.Range(min, max));
            AssignClip();
            audioSource.Play();
            yield return waitForClip;
        }
    }

    private void AssignClip()
    {
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
    }
}
