using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] public UnityEvent<Collider> OnEnterTrigger;
    [SerializeField] public UnityEvent<Collider> OnExitTrigger;
    [SerializeField, Tooltip("If empty will be ignored !")] private string objectTag;
    
    private void OnTriggerStay(Collider collider)
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
}
