using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PistolBullet : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float bulletLifetime = 5f;

    [Header("Ammo")]
    public int maxBullets;
    public int ammo;

    [Header("Fire Mode")]
    public bool automaticWeapon;
    public float fireRate = 0.1f;

    [Header("Collision (layers to ignore while held)")]
    [SerializeField] private int playerLayer = 2;
    [SerializeField] private int weaponLayer = 6;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private float nextFireTime;
    private bool isFiring;
    private bool isSubscribed;

    private Rigidbody rb;
    private bool firstPickupCompleted;

    public GameObject bulletChamber;
    public TextMeshProUGUI bulletCounter;

    public AudioSource gunshot;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        ammo = maxBullets;
        firstPickupCompleted = false;
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        bulletChamber.SetActive(false);
    }

    private void OnEnable()
    {
        if (isSubscribed)
            return;

        grabInteractable.activated.AddListener(StartFiring);
        grabInteractable.deactivated.AddListener(StopFiring);
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnDropped);

        isSubscribed = true;
    }

    private void OnDisable()
    {
        if (!isSubscribed)
            return;

        grabInteractable.activated.RemoveListener(StartFiring);
        grabInteractable.deactivated.RemoveListener(StopFiring);
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnDropped);

        isSubscribed = false;
        isFiring = false;
    }

    private void Update()
    {
        bulletCounter.text = string.Format(ammo + " / " + maxBullets);

        if (!automaticWeapon)
            return;

        if (isFiring &&
            grabInteractable.isSelected &&
            Time.time >= nextFireTime)
        {
            TryFire();
        }

        if (Time.timeScale == 0f)
        {
            StopGunSound();
            return;
        }
    }

    private void StartFiring(ActivateEventArgs args)
    {
        if (!grabInteractable.isSelected)
            return;

        if (isFiring)
            return;

        isFiring = true;

        // Fire immediately when trigger is pressed.
        TryFire();
    }

    private void StopFiring(DeactivateEventArgs args)
    {
        isFiring = false;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        bulletChamber.SetActive(true);

        if (!firstPickupCompleted)
        {
            firstPickupCompleted = true;
        }

        Physics.IgnoreLayerCollision(playerLayer, weaponLayer, true);
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        bulletChamber.SetActive(false);
        isFiring = false;

        Physics.IgnoreLayerCollision(playerLayer, weaponLayer, false);

        if (firstPickupCompleted)
        {
            StartCoroutine(EnableGravityAfterRelease());
        }
    }

    private System.Collections.IEnumerator EnableGravityAfterRelease()
    {
        // Wait until XR Grab Interactable finishes its release processing.
        yield return new WaitForFixedUpdate();

        // Don't override physics if the weapon was grabbed again.
        if (grabInteractable.isSelected)
            yield break;

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void TryFire()
    {
        if (Time.timeScale == 0f)
            return;

        if (Time.time < nextFireTime)
            return;

        if (ammo <= 0)
            return;

        FireBullet();

        nextFireTime = Time.time + fireRate;
    }

    private void FireBullet()
    {
        if (ammo <= 0 || bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        if (gunshot != null)
        {
            if (automaticWeapon)
            {
                if (!gunshot.isPlaying)
                    gunshot.Play();
            }
            else
            {
                gunshot.PlayOneShot(gunshot.clip);
            }
        }

        ammo--;

        if (automaticWeapon && ammo <= 0) StopGunSound();

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);
    }

    private void StopGunSound()
    {
        if (automaticWeapon && gunshot != null)
            gunshot.Stop();
    }
}
