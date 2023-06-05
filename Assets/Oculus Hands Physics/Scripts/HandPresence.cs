using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresence : MonoBehaviour
{
    [SerializeField] InputDeviceCharacteristics controllerCharacteristics;
    [SerializeField] Animator handAnimator;
    [SerializeField] ChainIKConstraint[] fingerChains;
    [SerializeField] Transform activationCenter;
    [SerializeField] float activationDistance = 0.1f, activationSmoothing = 0.02f;
    [SerializeField] bool ignoreControllers;

    private InputDevice targetDevice;
    private bool _useButtons = true;
    private bool _isPosing = false;
    private bool _isGrabbing = false;
    private int _previousLayer;

    private int InvertedLayer => ~(-1 << gameObject.layer);

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
        foreach (var chain in fingerChains)
        {
            chain.weight = 0;
        }

        TryInitialize();
    }

    private void StartPosing()
    {
        _useButtons = false;
        handAnimator.SetFloat("Trigger", 0);
        handAnimator.SetFloat("Grip", 0);
    }

    private void StopPosing()
    {
        _useButtons = true;
        foreach (ChainIKConstraint chain in fingerChains)
        {
            chain.weight = 0;
        }
    }

    public void Grab(SelectEnterEventArgs args)
    {
        _isGrabbing = true;
        _previousLayer = args.interactableObject.transform.gameObject.layer;
        args.interactableObject.transform.gameObject.layer = gameObject.layer;
    }

    public void Release(SelectExitEventArgs args)
    {
        _isGrabbing = false;
        args.interactableObject.transform.gameObject.layer = _previousLayer;
    }

    private float GetFingerWeight()
    {
        Collider[] colliders = Physics.OverlapSphere(activationCenter.position, activationDistance, InvertedLayer, QueryTriggerInteraction.Ignore);
        float weight = 0;
        foreach (Collider collider in colliders)
        {
            if (collider is MeshCollider)
                continue;

            Vector3 hitPoint = collider.ClosestPoint(activationCenter.position);
            float distance = Vector3.Distance(hitPoint, activationCenter.position);
            float hitWeight = Mathf.InverseLerp(activationDistance, activationDistance - activationSmoothing, distance);
            weight = Mathf.Max(weight, hitWeight);

            if (weight == 1)
                break;
        }
        return weight;
    }

    private void Update()
    {
        if (!ignoreControllers && !targetDevice.isValid)
        {
            TryInitialize();
            return;
        }

        bool inRange = !_isGrabbing && Physics.CheckSphere(activationCenter.position, activationDistance, InvertedLayer, QueryTriggerInteraction.Ignore);
        if (inRange && !_isPosing)
        {
            _isPosing = true;
            StartPosing();
        }
        else if (!inRange && _isPosing)
        {
            _isPosing = false;
            StopPosing();
        }

        if (_useButtons)
        {
            UpdateHandAnimation();
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
