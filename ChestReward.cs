using UnityEngine;
using System.Collections;

public class ChestReward : MonoBehaviour
{
    public enum RewardType
    {
        Points,
        Bomb,
        Gun
    }

    [Header("Reward")]
    public RewardType rewardType;

    [Header("Point Settings")]
    public float pointsCleanupDelay = 2f;

    [Header("Bomb Settings")]
    public GameObject explosionParticleSystem;
    public float explosionDelay = 2f;
    public float explosionDuration = 1f;
    public float explosionRadius = 2f;

    private bool activated = false;

    [Header("Reward Audio")]
    public AudioSource audioSource;
    public AudioClip explosionSound;
    public AudioClip victorySound;

    public void OpenReward(ChestManager chestManager)
    {
        if (activated)
            return;

        activated = true;

        AchievementManager.Instance?.ChestOpened(GetInstanceID(), rewardType == RewardType.Bomb);

        switch (rewardType)
        {
            case RewardType.Points:
                chestManager.chestBonusOpened = true;

                Debug.Log("Bonus chest collected!");

                if (audioSource != null && victorySound != null)
                {
                    audioSource.PlayOneShot(victorySound);
                }

                // Remove the point reward after a short delay.
                Destroy(gameObject, pointsCleanupDelay);
                break;

            case RewardType.Bomb:
                StartCoroutine(Explode());
                break;

            case RewardType.Gun:
                Debug.Log("Gun chest opened!");
                break;
        }
    }

    private IEnumerator Explode()
    {
        // Wait before detonating.
        yield return new WaitForSeconds(explosionDelay);
        // Activate the explosion particles.
        if (explosionParticleSystem != null)
        {
            explosionParticleSystem.SetActive(true);
            GetComponent<MeshRenderer>().enabled = false;
        }
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Check whether the player is within explosion range.
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        foreach (Collider hit in hits)
        {
            PlayerManager player =
                hit.GetComponentInParent<PlayerManager>();

            if (player != null)
            {
                AchievementManager.Instance?.PlayerTookDamage();
                AchievementManager.Instance?.BombKilledPlayer();
                player.EndGame();

                Debug.Log("Player hit by chest bomb!");
                break;
            }
        }

        // Allow the explosion and sound to finish.
        float cleanupDelay = explosionDuration;

        if (explosionSound != null)
        {
            cleanupDelay = Mathf.Max(
                cleanupDelay,
                explosionSound.length
            );
        }

        yield return new WaitForSeconds(cleanupDelay);

        Destroy(gameObject);
    }
}
