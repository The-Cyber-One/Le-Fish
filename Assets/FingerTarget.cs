using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerTarget : MonoBehaviour
{
    [SerializeField] ChainIKConstraint chainIK;
    [SerializeField] LayerMask layerMask;

    Vector3 _tipDirection, _downDirection;

    private void Start()
    {
        _tipDirection = chainIK.data.tip.position - chainIK.data.root.position;
        _downDirection = -transform.up * _tipDirection.magnitude;
    }

    private void Update()
    {
        Vector3 rayDirection = _downDirection - _tipDirection;
        if (Physics.Raycast(_tipDirection + chainIK.data.root.position, rayDirection, out RaycastHit hitInfo, 1, layerMask))
        {
            transform.position = hitInfo.point;
            chainIK.weight = 1;
        }
        else
        {
            chainIK.weight = 0;
        }
    }
}
