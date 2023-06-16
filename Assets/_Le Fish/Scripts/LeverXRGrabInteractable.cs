using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] float maxInteractionDistance = 1;

    private void Update()
    {
        if (firstInteractorSelecting == null)
            return;

        if (GetDistanceSqrToInteractor(firstInteractorSelecting) >= maxInteractionDistance * maxInteractionDistance)
            interactionManager.CancelInteractableSelection(this as IXRSelectInteractable);
    }
}
