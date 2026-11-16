using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOutput : MonoBehaviour
{
    public InputActionProperty triggerVal;
    public InputActionProperty gripVal;

    public Animator handimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float trigger = triggerVal.action.ReadValue<float>();
        float grip = gripVal.action.ReadValue<float>();

        handimator.SetFloat("Trigger", trigger);
        handimator.SetFloat("Grip", grip);
    }
}
