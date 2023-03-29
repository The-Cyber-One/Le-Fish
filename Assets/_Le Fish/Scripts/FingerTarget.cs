using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerTarget : MonoBehaviour
{
    [SerializeField] ChainIKConstraint chainIK;
    [SerializeField] Transform rootRay;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform activationCenter;
    [SerializeField] float activationDistance = 0.1f, activationSmoothing = 0.01f;
    [SerializeField] float poseSmoothingSpeed = 0.05f;

    private Transform desiredTarget;
    private float _offset;
    private float ActivationSmoothingEnd => activationDistance - activationSmoothing;

    private void Start()
    {
        desiredTarget = new GameObject("Desired Target").transform;
        desiredTarget.SetParent(transform.parent);
        desiredTarget.position = transform.position = chainIK.data.tip.position;
        _offset = chainIK.data.tip.parent.GetComponentsInChildren<CapsuleCollider>().Last().radius;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(activationCenter.position, activationDistance, layerMask, QueryTriggerInteraction.Ignore);
        chainIK.weight = 0;
        if (rootRay == null || colliders.Length == 0)
        {
            transform.position = desiredTarget.position = chainIK.data.tip.position;
            return;
        }

        float weight = 0;
        foreach (Collider collider in colliders)
        {
            Vector3 hitPoint = collider.ClosestPoint(activationCenter.position);
            float distance = Vector3.Distance(hitPoint, activationCenter.position);
            float hitWeight = Mathf.InverseLerp(activationDistance, ActivationSmoothingEnd, distance);
            weight = Mathf.Max(weight, hitWeight);
            
            if (weight == 1) 
                break;
        }
        chainIK.weight = weight;

        Transform ray = rootRay;
        while (ray.childCount > 0)
        {
            Transform child = ray.GetChild(0);
            Vector3 direction = child.position - ray.position;
            if (Physics.Raycast(ray.position, direction, out RaycastHit hitInfo, direction.magnitude, layerMask, QueryTriggerInteraction.Ignore))
            {
                desiredTarget.position = hitInfo.point + hitInfo.normal * _offset;
                break;
            }
            ray = child;
        }

        transform.position = Vector3.MoveTowards(transform.position, desiredTarget.position, poseSmoothingSpeed);
    }

    [SerializeField] private bool showGizmos;
    private void OnDrawGizmos()
    {
        if (!showGizmos || rootRay == null || activationCenter == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(activationCenter.position, activationDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(activationCenter.position, ActivationSmoothingEnd);

        Gizmos.color = Color.magenta;
        Transform ray = rootRay;
        while (ray.childCount > 0)
        {
            Transform child = ray.GetChild(0);
            Gizmos.DrawRay(ray.position, child.position - ray.position);
            ray = child;
        }
    }
}
