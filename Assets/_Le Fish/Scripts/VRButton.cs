using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnActivated;

    [SerializeField] float activationDistance = 0.1f, deactivationDistance = 0.01f;
    [SerializeField] Transform pusher;

    Vector3 _startPosition;
    float _maxY;
    bool _activated = false;

    private void Start()
    {
        _startPosition = pusher.localPosition;
        _maxY = pusher.GetComponent<ConfigurableJoint>().linearLimit.limit;
    }

    private void LateUpdate()
    {
        Clamp();
    }

    private void FixedUpdate()
    {
        Clamp();

        if (!_activated && Vector3.Distance(pusher.localPosition, _startPosition) >= activationDistance)
        {
            _activated = true;
            OnActivated?.Invoke();
        }

        if (_activated && Vector3.Distance(pusher.localPosition, _startPosition) <= deactivationDistance)
            _activated = false;
    }

    private void Clamp()
    {
        float y = Mathf.Clamp(pusher.localPosition.y, -_maxY + _startPosition.y, _startPosition.y);
        pusher.localPosition = new Vector3(_startPosition.x, y, _startPosition.z);
    }
}
