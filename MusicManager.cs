using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource theme;
    public float loopStart;
    public float loopEnd;
    private bool introPlayed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theme.time = 0f;
        theme.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!introPlayed && theme.time >= loopStart)
        {
            introPlayed = true;
        }

        if (introPlayed && theme.time >= loopEnd)
        {
            theme.time = loopStart;
        }
    }
}
