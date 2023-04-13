using UnityEngine;
using UnityEngine.Events;

public class ColliderDetector : MonoBehaviour
{
    [SerializeField, Tooltip("If empty will be ignored!")] string objectTag;
    public UnityEvent<Collider> OnEnterTrigger;
    public UnityEvent<Collider> OnExitTrigger;
    public UnityEvent<Collision> OnEnterCollider;
    public UnityEvent<Collision> OnExitCollider;

    private void OnTriggerEnter(Collider collider)
    {
        if (objectTag == string.Empty || collider.gameObject.CompareTag(objectTag))
        {
            OnEnterTrigger.Invoke(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (objectTag == string.Empty || collider.gameObject.CompareTag(objectTag))
        {
            OnExitTrigger.Invoke(collider);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            OnEnterTrigger.Invoke(GetComponent<Collider>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            OnExitTrigger.Invoke(GetComponent<Collider>());
        }
    }
}
