using UnityEngine;

public class PlushyShot : MonoBehaviour
{
    private Scorekeeper scoreInfo;
    public GameObject plasmaExplosion;

    public void Awake()
    {
        scoreInfo = FindFirstObjectByType<Scorekeeper>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plushy"))
        {
            Collider hitCollider = collision.collider;
            PlushyInfo plushyInfo = collision.gameObject.GetComponent<PlushyInfo>();

            if (hitCollider is SphereCollider)
            {
                Debug.Log("Bullet hit a Sphere Collider!");
                plushyInfo.health = Mathf.Max(plushyInfo.health - 2, 0);
                AchievementManager.Instance?.PlayerDealtWuvlieDamage();
                if (plushyInfo.health <= 0) 
                {
                    AchievementManager.Instance?.WuvlieDefeated();
                    switch (plushyInfo.type)
                    {
                        case PlushyInfo.PlushType.Pinchypup:
                            scoreInfo.score += 1;
                            break;
                        case PlushyInfo.PlushType.Shellshark:
                            scoreInfo.score += 2;
                            break;
                        case PlushyInfo.PlushType.Hoppo:
                            scoreInfo.score += 1;
                            break;
                        case PlushyInfo.PlushType.Rattlemeowser:
                            scoreInfo.score += 3;
                            break;
                        default:
                            Debug.Log("You killed an innocent civilian.... Fantastic!");
                            break;
                    };
                    GameObject explosion = Instantiate(plasmaExplosion, collision.gameObject.transform.position, Quaternion.identity);

                    AudioSource poofSound = explosion.GetComponent<AudioSource>();

                    if (poofSound != null)
                    {
                        poofSound.Play();
                    }

                    Destroy(collision.gameObject);
                }             
            }
            else if (hitCollider is BoxCollider)
            {
                Debug.Log("Bullet hit a Box Collider!");
                plushyInfo.health = Mathf.Max(plushyInfo.health - 1, 0);
                AchievementManager.Instance?.PlayerDealtWuvlieDamage();
                if (plushyInfo.health <= 0)
                {
                    AchievementManager.Instance?.WuvlieDefeated();
                    switch (plushyInfo.type)
                    {
                        case PlushyInfo.PlushType.Pinchypup:
                            scoreInfo.score += 1;
                            break;
                        case PlushyInfo.PlushType.Shellshark:
                            scoreInfo.score += 2;
                            break;
                        case PlushyInfo.PlushType.Hoppo:
                            scoreInfo.score += 1;
                            break;
                        case PlushyInfo.PlushType.Rattlemeowser:
                            scoreInfo.score += 3;
                            break;
                        default:
                            Debug.Log("You killed an innocent civilian.... Fantastic!");
                            break;
                    };

                    GameObject explosion = Instantiate(plasmaExplosion, collision.gameObject.transform.position, Quaternion.identity);

                    AudioSource poofSound = explosion.GetComponent<AudioSource>();

                    if (poofSound != null)
                    {
                        poofSound.Play();
                    }

                    Destroy(collision.gameObject);
                }
            }

        }

        Destroy(this.gameObject);
    }

}
