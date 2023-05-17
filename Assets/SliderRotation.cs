using UnityEngine;
using UnityEngine.UI;

public class SliderRotation : MonoBehaviour
{
    public Slider slider;
    public GameObject objectToRotate;
    public float minRotation = -45f;
    public float maxRotation = 45f;
    public float sliderMinValue = 0f;
    public float sliderMaxValue = 10f;

    void Start()
    {
        // Set the initial rotation of the object based on the starting slider value
        float initialRotation = Mathf.Lerp(minRotation, maxRotation, Mathf.InverseLerp(sliderMinValue, sliderMaxValue, slider.value));
        objectToRotate.transform.eulerAngles = new Vector3(initialRotation, 0f, 0f);
    }

    void Update()
    {
        // Get the rotation of the object
        float rotation = objectToRotate.transform.eulerAngles.x;

        // Normalize the rotation within the defined range (-180 to 180)
        float normalizedRotation = (rotation > 180f) ? rotation - 360f : rotation;

        // Map the normalized rotation to the slider value range
        float sliderValue = Mathf.Lerp(sliderMinValue, sliderMaxValue, Mathf.InverseLerp(minRotation, maxRotation, normalizedRotation));

        // Update the slider value
        slider.value = sliderValue;
    }

    public void OnSliderValueChanged()
    {
        // Get the current slider value
        float sliderValue = slider.value;

        // Map the slider value to the rotation range
        float normalizedValue = Mathf.InverseLerp(sliderMinValue, sliderMaxValue, sliderValue);
        float rotation = Mathf.Lerp(minRotation, maxRotation, normalizedValue);

        // Update the rotation of the object
        objectToRotate.transform.eulerAngles = new Vector3(rotation, 0f, 0f);
    }
}
