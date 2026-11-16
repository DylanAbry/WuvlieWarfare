using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer)
            return;

        PlayerManager player =
            other.GetComponentInParent<PlayerManager>();

        if (player != null)
        {
            hasHitPlayer = true;

            // Tell the player to take damage.
            player.TakePlushyDamage();

            // Destroy the entire plushy, not just the trigger.
            Destroy(transform.root.gameObject);
        }
    }
}
