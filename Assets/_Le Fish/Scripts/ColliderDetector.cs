using UnityEngine;
using UnityEngine.Events;

public class ColliderDetector : MonoBehaviour
{
    [SerializeField, Tooltip("If empty will be ignored!")] string objectTag;
    [SerializeField] UnityEvent<Collider> onEnterTrigger;
    [SerializeField] UnityEvent<Collider> onExitTrigger;
    [SerializeField] UnityEvent<Collision> onEnterCollider;
    [SerializeField] UnityEvent<Collision> onExitCollider;

    private void OnTriggerStay(Collider collider)
    {
        if (objectTag == string.Empty || collider.gameObject.CompareTag(objectTag))
        {
            onEnterTrigger.Invoke(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (objectTag == string.Empty || collider.gameObject.CompareTag(objectTag))
        {
            onExitTrigger.Invoke(collider);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            onEnterTrigger.Invoke(GetComponent<Collider>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            onExitTrigger.Invoke(GetComponent<Collider>());
        }
    }
}
