using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderRotation : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private VRLever lever;
    [SerializeField] private float minVolume = -30, maxVolume = 10;
    [SerializeField] private AudioMixerGroup audioGroup;
    [SerializeField] private string VolumeParameterName;

    void Start()
    {
        lever.OnValueChanged2D.AddListener(LeverChanged);
    }

    void LeverChanged(Vector2 valueIn)
    {
        Debug.Log(valueIn);
        float value = Mathf.InverseLerp(-1, 1, valueIn.y);
        Debug.Log(value);
        audioGroup.audioMixer.SetFloat(VolumeParameterName, Mathf.Lerp(minVolume, maxVolume, value));
        Debug.Log(Mathf.Lerp(minVolume, maxVolume, value));
        slider.value = value;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
