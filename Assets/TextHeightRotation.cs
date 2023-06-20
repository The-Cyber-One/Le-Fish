using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeightRotation : MonoBehaviour
{
    [SerializeField] private VRLever lever;
    [SerializeField] private float minOffset = -1, maxOffset = 1;
    [SerializeField] Transform targetObject; // The object whose position changes
    [SerializeField] Transform referenceObject; // The object whose rotation determines the position change

    private float _startingYPosition;
    // Start is called before the first frame update
    void Start()
    {
        _startingYPosition = targetObject.localPosition.y;
        lever.OnValueChanged2D.AddListener(LeverChanged);
    }

    void LeverChanged(Vector2 valueIn)
    {
  
        float offsetChange = 0f;
        float value = Mathf.InverseLerp(-1, 1, valueIn.y);
        offsetChange = Mathf.Lerp(minOffset, maxOffset, value);
        Vector3 newPosition = targetObject.localPosition;
        newPosition.y = _startingYPosition + (offsetChange);
        targetObject.localPosition = newPosition;
    }
}
