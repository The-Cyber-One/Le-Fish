using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class VRLever : MonoBehaviour
{
    public UnityEvent<Vector2> OnValueChanged2D;

    [SerializeField] private ConfigurableJoint joint;
    [SerializeField] private XRGrabInteractable interactable;

    [SerializeField, Range(0, 1)] private float deadZone = 0.1f;

    private float _previousX, _previousY;

    private void Update()
    {
        float x = GetJoyStickAngle(-transform.forward, joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit);
        float y = GetJoyStickAngle(transform.right, -joint.angularZLimit.limit, joint.angularZLimit.limit);
        if (x != _previousX || y != _previousY)
        {
            _previousX = x;
            _previousY = y;
            OnValueChanged2D?.Invoke(new Vector2(x, y));
        }
    }

    private float GetJoyStickAngle(Vector3 planeNormal, float minAngle, float maxAngle)
    {
        Vector3 projectedOnPlane = Vector3.ProjectOnPlane(joint.transform.up, planeNormal);
        float angle = Vector3.SignedAngle(transform.up, projectedOnPlane, planeNormal);
        angle = Mathf.InverseLerp(minAngle, maxAngle, angle);
        angle = Mathf.Clamp01(angle);
        angle = Mathf.Lerp(-1, 1, angle);
        if (angle > -deadZone && angle < deadZone) return 0;
        return angle;
    }
}
