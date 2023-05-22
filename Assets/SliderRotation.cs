using UnityEngine;
using UnityEngine.UI;

public class SliderRotation : MonoBehaviour
{
    public Slider rotationSlider;
    public GameObject objectToRotate;
    public float minRotation = -45f;
    public float maxRotation = 45f;
    public float sliderMinValue = 0f;
    public float sliderMaxValue = 10f;
    public AudioSource audioSource;

    void Start()
    {
        // Set the initial rotation of the object based on the starting slider value
        float initialRotation = Mathf.Lerp(minRotation, maxRotation, Mathf.InverseLerp(sliderMinValue, sliderMaxValue, rotationSlider.value));
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

        // Update the rotation slider value
        rotationSlider.value = sliderValue;

        // Set the volume based on the rotation slider value
        float volume = rotationSlider.value / 10f; // Normalize slider value to volume range (0 to 1)
        audioSource.volume = volume;
    }

    public void OnRotationSliderValueChanged()
    {
        // Get the current slider value
        float sliderValue = rotationSlider.value;

        // Map the slider value to the rotation range
        float normalizedValue = Mathf.InverseLerp(sliderMinValue, sliderMaxValue, sliderValue);
        float rotation = Mathf.Lerp(minRotation, maxRotation, normalizedValue);

        // Update the rotation of the object
        objectToRotate.transform.eulerAngles = new Vector3(rotation, 0f, 0f);
    }
    public void QuitApplication()
    {
        // Quit the application
        Application.Quit();
    }
}
