using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerTarget : MonoBehaviour
{
    [SerializeField] ChainIKConstraint chainIK;
    [SerializeField] Transform rootRay;
    [SerializeField] float poseSmoothingSpeed = 0.05f;

    public bool IsPosing { get; private set; }

    private Transform desiredTarget, startTarget;
    private float _offset;

    private void Start()
    {
        desiredTarget = new GameObject("Desired Target").transform;
        desiredTarget.SetParent(transform.parent);
        desiredTarget.position = transform.position = chainIK.data.tip.position;
        startTarget = new GameObject("Start Target").transform;
        startTarget.SetParent(transform.parent);
        startTarget.position = transform.position = chainIK.data.tip.position;
        _offset = chainIK.data.tip.parent.GetComponentsInChildren<CapsuleCollider>().Last().radius;
    }

    private void Update()
    {
        if (rootRay == null || chainIK.weight == 0)
        {
            transform.position = desiredTarget.position = startTarget.position;
            return;
        }

        Transform ray = rootRay;
        while (ray.childCount > 0)
        {
            Transform child = ray.GetChild(0);
            Vector3 direction = child.position - ray.position;
            if (Physics.Raycast(ray.position, direction, out RaycastHit hitInfo, direction.magnitude, ~(-1 << gameObject.layer), QueryTriggerInteraction.Ignore))
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
        if (!showGizmos || rootRay == null)
            return;

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
