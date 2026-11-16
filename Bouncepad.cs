using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class Bouncepad : MonoBehaviour
{
    public float bounceVelocity = 12f;
    public float bounceGravity = 20f;
    public GravityProvider gravityProvider;

    private bool bouncing = false;

    public AudioSource bounceSound;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player =
            other.GetComponent<CharacterController>();

        if (player == null || bouncing || gravityProvider == null)
            return;

        StartCoroutine(Bounce(player));
    }

    private IEnumerator Bounce(CharacterController player)
    {
        bouncing = true;

        // Let the trampoline control vertical movement.
        gravityProvider.useGravity = false;

        float velocity = bounceVelocity;
        bounceSound.Play();

        while (velocity > 0f && player != null)
        {
            player.Move(Vector3.up * velocity * Time.deltaTime);

            velocity -= bounceGravity * Time.deltaTime;

            yield return null;
        }

        // Restore normal XR gravity at the top of the bounce.
        gravityProvider.useGravity = true;

        bouncing = false;
    }
}
