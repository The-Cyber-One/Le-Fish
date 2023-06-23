using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeightRotation : MonoBehaviour
{
    [SerializeField] private VRLever lever;
    [SerializeField] private float minOffset = -1, maxOffset = 1;
    [SerializeField] Transform speechBubbleObject; // The object whose position changes

    private float _startingYPosition;
    // Start is called before the first frame update
    void Start()
    {
        _startingYPosition = speechBubbleObject.localPosition.y;
        lever.OnValueChanged2D.AddListener(LeverChanged);
    }

    void LeverChanged(Vector2 valueIn)
    {
  
        float offsetChange = 0f;
        float value = Mathf.InverseLerp(-1, 1, valueIn.y);
        offsetChange = Mathf.Lerp(minOffset, maxOffset, value);
        Vector3 newPosition = speechBubbleObject.localPosition;
        newPosition.y = _startingYPosition + (offsetChange);
        speechBubbleObject.localPosition = newPosition;
    }
}
