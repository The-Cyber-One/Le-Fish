using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class LocomotionController : MonoBehaviour
{
    public XRController rightControllerRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] XRRayInteractor rayInteractor;

    private bool _hovering;
    private bool _previousActive;

    // Update is called once per frame
    void Update()
    {
        bool isActivated = CheckIfActivated(rightControllerRay);
        if (rightControllerRay)
        {
            rightControllerRay.gameObject.SetActive(isActivated);
        }
        if (_previousActive && !isActivated && _hovering)
            audioSource.Play();

        _hovering = rayInteractor.TryGetCurrent3DRaycastHit(out _);
        _previousActive = isActivated;
    }
    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice,teleportActivationButton,out bool isActivated, activationThreshold);
        return isActivated;
    }
}
