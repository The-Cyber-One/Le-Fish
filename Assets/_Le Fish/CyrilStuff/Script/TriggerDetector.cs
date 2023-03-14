using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] UnityEvent onEnterTrigger;
    [SerializeField] UnityEvent onExitTrigger;
    [SerializeField, Tooltip("If empty will be ignored !")] private string objectTag;
    
    private void OnTriggerStay(Collider collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            onEnterTrigger.Invoke();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (objectTag == string.Empty || collision.gameObject.CompareTag(objectTag))
        {
            onExitTrigger.Invoke();
        }
    }
}
