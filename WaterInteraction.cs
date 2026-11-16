using UnityEngine;

public class WaterInteraction : MonoBehaviour
{
    [Header("Water Audio")]
    public AudioClip splashSound;
    public AudioClip waterMovementSound;

    [Header("Water Particles")]
    public GameObject splashParticlePrefab;

    [Header("Movement Detection")]
    public float movementThreshold = 0.15f;

    [Header("Splash Settings")]
    public float splashVolume = 1f;
    public float movementVolume = 1f;

    private AudioSource splashAudio;
    private AudioSource movementAudio;

    private bool isInWater;
    private Transform currentWater;

    private Vector3 previousPosition;

    private void Awake()
    {
        splashAudio = gameObject.AddComponent<AudioSource>();
        splashAudio.playOnAwake = false;
        splashAudio.spatialBlend = 1f;

        movementAudio = gameObject.AddComponent<AudioSource>();
        movementAudio.playOnAwake = false;
        movementAudio.loop = true;
        movementAudio.spatialBlend = 0f;
        movementAudio.clip = waterMovementSound;
        movementAudio.volume = movementVolume;

        previousPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        if (isInWater)
        {
            float movementSpeed =
                (currentPosition - previousPosition).magnitude
                / Mathf.Max(Time.deltaTime, 0.0001f);

            bool isMoving = movementSpeed > movementThreshold;

            if (isMoving)
            {
                if (!movementAudio.isPlaying &&
                    waterMovementSound != null)
                {
                    movementAudio.Play();
                }
            }
            else
            {
                if (movementAudio.isPlaying)
                {
                    movementAudio.Stop();
                }
            }
        }

        previousPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water"))
            return;

        if (isInWater)
            return;

        isInWater = true;
        currentWater = other.transform;

        PlaySplash();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water"))
            return;

        if (!isInWater)
            return;

        isInWater = false;

        if (movementAudio.isPlaying)
            movementAudio.Stop();

        PlaySplash();

        currentWater = null;
    }

    private void PlaySplash()
    {
        if (splashSound != null)
        {
            splashAudio.PlayOneShot(
                splashSound,
                splashVolume
            );
        }

        if (splashParticlePrefab != null)
        {
            Vector3 splashPosition = transform.position;

            if (currentWater != null)
            {
                splashPosition.y = currentWater.position.y;
            }

            GameObject splash = Instantiate(
                splashParticlePrefab,
                splashPosition,
                Quaternion.identity
            );

            Destroy(splash, 3f);
        }
    }

    private void OnDisable()
    {
        isInWater = false;

        if (movementAudio != null)
            movementAudio.Stop();
    }
}
