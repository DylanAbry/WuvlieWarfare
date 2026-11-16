using UnityEngine;
using UnityEngine.UI;

public class UISoundEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Start()
    {
        Button[] buttons =
            GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
