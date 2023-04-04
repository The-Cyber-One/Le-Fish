using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    [SerializeField] InputDeviceCharacteristics controllerCharacteristics;
    [SerializeField] Animator handAnimator;
    [SerializeField] ChainIKConstraint[] fingerChains;
    [SerializeField] Transform activationCenter;
    [SerializeField] float activationDistance = 0.1f, activationSmoothing = 0.02f;
    [SerializeField] LayerMask layerMask;

    private InputDevice targetDevice;
    private bool _useButtons, _usePoser;
    private HandPoser _handPoser;

    private void TryInitialize()
    {
        List<InputDevice> devices = new();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    private void Start()
    {
        TryInitialize();
    }

    private readonly Collider[] _colliders = new Collider[5];
    private float GetFingerWeight()
    {
        Physics.OverlapSphereNonAlloc(activationCenter.position, activationDistance, _colliders, layerMask, QueryTriggerInteraction.Ignore);
        float weight = 0;
        foreach (Collider collider in _colliders)
        {
            Vector3 hitPoint = collider.ClosestPoint(activationCenter.position);
            float distance = Vector3.Distance(hitPoint, activationCenter.position);
            float hitWeight = Mathf.InverseLerp(activationDistance, activationDistance - activationSmoothing, distance);
            weight = Mathf.Max(weight, hitWeight);

            if (weight == 1)
                break;
        }
        return weight;
    }

    public void StartPosing(Collider collider)
    {
        _useButtons = false;
        _usePoser = collider.TryGetComponent(out _handPoser);
        handAnimator.SetFloat("Trigger", 0);
        handAnimator.SetFloat("Grip", 0);
    }

    public void StopPosing(Collider collider)
    {
        _useButtons = true;
        _handPoser = null;
    }

    private void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
            return;
        }

        if (_useButtons)
        {
            UpdateHandAnimation();
        }
        else if (_usePoser)
        {
            
        }
        else
        {
            float weight = GetFingerWeight();
            foreach (var chain in fingerChains)
            {
                chain.weight = weight;
            }
        }
    }

    void UpdateHandAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    [SerializeField] private bool showGizmos;
    private void OnDrawGizmos()
    {
        if (!showGizmos || activationCenter == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(activationCenter.position, activationDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(activationCenter.position, activationDistance - activationSmoothing);
    }
}
