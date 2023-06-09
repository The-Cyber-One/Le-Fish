using UnityEngine;

public class LeavingParticleDeleter : MonoBehaviour
{
    private ParticleSystem leavingParticleSystem;

    void Start()
    {
        leavingParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!leavingParticleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
