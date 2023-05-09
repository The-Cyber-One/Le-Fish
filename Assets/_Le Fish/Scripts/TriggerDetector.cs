using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> onEnterTrigger;
    [SerializeField] UnityEvent<Collider> onExitTrigger;
    [SerializeField, Tooltip("If empty will be ignored !")] private string objectTag;
    
    private void OnTriggerEnter(Collider collider)
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
}
