using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeightLever : MonoBehaviour
{
    public Transform targetObject; // The object whose position changes
    public Transform referenceObject; // The object whose rotation determines the position change
    public float positionScale = 1f; // Scaling factor for position change
    public float maxRotation = 45f; // Maximum rotation in degrees
    public float minRotation = -45f; // Minimum rotation in degrees
    public float maxOffset = 1f; // Maximum offset to be added
    public float minOffset = -1f; // Minimum offset to be added

    private float startingRotation;
    private float startingYPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = referenceObject.localRotation.eulerAngles.x;
        startingYPosition = targetObject.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        float rotationChange = referenceObject.localRotation.eulerAngles.x - startingRotation;
        float offsetChange = 0f;

        if (rotationChange >= 0f && rotationChange <= maxRotation)
        {
            offsetChange = Mathf.LerpUnclamped(0f, maxOffset, Mathf.InverseLerp(0f, maxRotation, rotationChange));
        }
        else if (rotationChange < 0f && rotationChange >= minRotation)
        {
            offsetChange = Mathf.LerpUnclamped(0f, minOffset, Mathf.InverseLerp(0f, minRotation, Mathf.Abs(rotationChange)));
        }

        Vector3 newPosition = targetObject.localPosition;
        newPosition.y = startingYPosition + (offsetChange * positionScale);
        targetObject.localPosition = newPosition;
    }
}
