using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandednessManager : MonoBehaviour
{
    [Header("Ray Interactors")]
    public XRRayInteractor leftRay;
    public XRRayInteractor rightRay;

    [Header("Direct Interactors")]
    public XRDirectInteractor leftDirect;
    public XRDirectInteractor rightDirect;

    [Header("Activate Actions")]
    public InputActionReference leftActivate;
    public InputActionReference rightActivate;

    [Header("Starting Hand")]
    public bool rightHanded = true;

    private void OnEnable()
    {
        leftActivate.action.performed += OnLeftActivate;
        rightActivate.action.performed += OnRightActivate;
    }

    private void Start()
    {
        SetActiveHand(rightHanded);
    }

    private void OnDisable()
    {
        leftActivate.action.performed -= OnLeftActivate;
        rightActivate.action.performed -= OnRightActivate;
    }

    private void OnLeftActivate(InputAction.CallbackContext context)
    {
        if (!IsHandHoldingSomething(leftDirect, leftRay))
            SetActiveHand(false);
    }

    private void OnRightActivate(InputAction.CallbackContext context)
    {
        if (!IsHandHoldingSomething(rightDirect, rightRay))
            SetActiveHand(true);
    }

    private bool IsHandHoldingSomething(
        XRDirectInteractor direct,
        XRRayInteractor ray)
    {
        return direct.hasSelection || ray.hasSelection;
    }

    private void SetActiveHand(bool useRightHand)
    {
        rightHanded = useRightHand;

        rightRay.enabled = useRightHand;
        leftRay.enabled = !useRightHand;
    }
}
