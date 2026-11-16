using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    public float kickForce = 6f;
    public float liftForce = 1.2f;
    private AudioSource ballBounce;

    void OnCollisionEnter(Collision col)
    {
        ballBounce = this.gameObject.GetComponent<AudioSource>();
        if (col.gameObject.CompareTag("Plushy"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            Vector3 dir = (transform.position - col.transform.position).normalized;
            dir.y = 0;

            rb.AddForce(dir * kickForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        }
        ballBounce.Play();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ballBounce = this.gameObject.GetComponent<AudioSource>();
        if (hit.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            Vector3 dir = (transform.position - hit.transform.position).normalized;
            dir.y = 0;

            rb.AddForce(dir * kickForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        }

        ballBounce.Play();
    }
}
