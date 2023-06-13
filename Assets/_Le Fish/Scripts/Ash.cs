using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ash : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable interactable;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private ParticleSystem dustParticles;

    private void Start()
    {
        interactable.selectEntered.AddListener(Grabbed);
    }

    private void Grabbed(SelectEnterEventArgs enterEventArgs)
    {
        meshFilter.mesh = null;
        dustParticles.Play();
        Destroy(gameObject, dustParticles.main.duration);
    }
}
