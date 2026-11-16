using UnityEngine;

public class ChestOpener : MonoBehaviour
{
    [Header("Chest References")]
    public HingeJoint lidHinge;
    public Transform chestHolder;
    public ChestManager chestManager;

    public AudioSource audioSource;
    public AudioClip doorOpenSound;

    [Header("Opening Settings")]
    public float openingAngle = 35f;

    private bool opened = false;

    private void Update()
    {
        if (opened || lidHinge == null)
            return;

        // Works for positive or negative hinge rotation.
        if (Mathf.Abs(lidHinge.angle) >= openingAngle)
        {
            opened = true;
            OpenChest();
        }
    }

    private void OpenChest()
    {
        // Play the chest opening sound.
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        if (chestHolder.childCount == 0)
            return;

        ChestReward reward =
            chestHolder.GetComponentInChildren<ChestReward>();

        if (reward != null)
            reward.OpenReward(chestManager);
    }
}
