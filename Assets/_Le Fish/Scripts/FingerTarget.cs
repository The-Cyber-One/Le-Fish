using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerTarget : MonoBehaviour
{
    [SerializeField] ChainIKConstraint chainIK;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform rayDirection;
    [SerializeField] float activationDistance = 0.5f, activationSmoothing;

    private void Update()
    {
        if (Physics.Raycast(rayDirection.position, rayDirection.forward, out RaycastHit hitInfo, activationDistance, layerMask))
        {
            transform.position = hitInfo.point;
            chainIK.weight = 1;
        }
        else
        {
            chainIK.weight = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (rayDirection == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(rayDirection.position, rayDirection.forward * activationDistance);
    }
}
