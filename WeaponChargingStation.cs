using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class WeaponChargingStation : MonoBehaviour
{
    [Header("Station")]
    public XRSocketInteractor socket;
    public Renderer sphereRenderer;

    [Header("Station Materials")]
    public Material emptyMaterial;      
    public Material lowMaterial;        
    public Material halfMaterial;       
    public Material fullMaterial;       

    [Header("Charging")]
    public float secondsPerBullet = 1f;

    private PistolBullet currentWeapon;
    private float chargeTimer;

    public AudioSource chargingSound;
    public AudioClip chargingDoneSound;

    private void Awake()
    {
        if (socket == null)
            socket = GetComponent<XRSocketInteractor>();
    }

    private void Start()
    {
        SetMaterial(emptyMaterial);
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(WeaponInserted);
        socket.selectExited.AddListener(WeaponRemoved);
    }

    private void OnDisable()
    {
        StopChargingSound();

        if (socket == null)
            return;

        socket.selectEntered.RemoveListener(WeaponInserted);
        socket.selectExited.RemoveListener(WeaponRemoved);
    }

    private void Update()
    {
        if (currentWeapon == null)
            return;

        // Already fully charged.
        if (currentWeapon.ammo >= currentWeapon.maxBullets)
        {
            currentWeapon.ammo = currentWeapon.maxBullets;
            SetMaterial(fullMaterial);
            StopChargingSound();
            chargeTimer = 0f;
            return;
        }

        // Continue charging.
        StartChargingSound();

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= secondsPerBullet)
        {
            currentWeapon.ammo++;

            currentWeapon.ammo = Mathf.Min(
                currentWeapon.ammo,
                currentWeapon.maxBullets
            );

            chargeTimer -= secondsPerBullet;

            Debug.Log(
                "Charging: " +
                currentWeapon.ammo +
                "/" +
                currentWeapon.maxBullets
            );

            // Weapon just reached full ammo.
            if (currentWeapon.ammo >= currentWeapon.maxBullets)
            {
                CompleteCharging();
                return;
            }
        }

        UpdateStationMaterial();
    }

    private void WeaponInserted(SelectEnterEventArgs args)
    {
        currentWeapon =
            args.interactableObject.transform.GetComponent<PistolBullet>();

        chargeTimer = 0f;

        if (currentWeapon == null)
        {
            Debug.LogWarning("Inserted object is not a weapon!");
            SetMaterial(emptyMaterial);
            return;
        }

        Debug.Log(
            "Weapon inserted with " +
            currentWeapon.ammo +
            "/" +
            currentWeapon.maxBullets +
            " bullets."
        );

        UpdateStationMaterial();

        if (currentWeapon.ammo < currentWeapon.maxBullets)
        {
            StartChargingSound();
        }
        else
        {
            StopChargingSound();
        }
    }

    private void WeaponRemoved(SelectExitEventArgs args)
    {
        StopChargingSound();

        currentWeapon = null;
        chargeTimer = 0f;

        // Empty charging station = blue.
        SetMaterial(emptyMaterial);
    }

    private void UpdateStationMaterial()
    {
        if (currentWeapon == null)
        {
            SetMaterial(emptyMaterial);
            return;
        }

        float chargePercentage =
            (float)currentWeapon.ammo /
            currentWeapon.maxBullets;

        if (chargePercentage >= 1f)
        {
            // 100% = Green
            SetMaterial(fullMaterial);
        }
        else if (chargePercentage >= 0.5f)
        {
            // 50-99% = Yellow
            SetMaterial(halfMaterial);
        }
        else
        {
            // 0-49% = Red
            SetMaterial(lowMaterial);
        }
    }

    private void SetMaterial(Material material)
    {
        if (sphereRenderer != null && material != null)
        {
            sphereRenderer.material = material;
        }
    }

    private void StartChargingSound()
    {
        if (chargingSound != null && !chargingSound.isPlaying)
        {
            chargingSound.Play();
        }
    }

    private void StopChargingSound()
    {
        if (chargingSound != null && chargingSound.isPlaying)
        {
            chargingSound.Stop();
        }
    }

    private void CompleteCharging()
    {
        StopChargingSound();

        if (chargingSound != null && chargingDoneSound != null)
        {
            chargingSound.PlayOneShot(chargingDoneSound);
        }

        SetMaterial(fullMaterial);
        chargeTimer = 0f;
    }
}
