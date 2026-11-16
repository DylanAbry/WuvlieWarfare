using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlushyVoice : MonoBehaviour
{
    [Header("Voice Clips")]
    public AudioClip[] voiceClips;

    [Header("Voice Timing")]
    public float minInterval = 12f;
    public float maxInterval = 25f;

    [Header("Audio")]
    public float voiceVolume = 0.8f;

    private AudioSource voiceAudio;
    private Coroutine voiceCoroutine;

    private void Awake()
    {
        voiceAudio = GetComponent<AudioSource>();

        voiceAudio.playOnAwake = false;
        voiceAudio.loop = false;
        voiceAudio.spatialBlend = 1f;
        voiceAudio.volume = voiceVolume;
    }

    private void OnEnable()
    {
        voiceCoroutine = StartCoroutine(PlayRandomVoices());
    }

    private IEnumerator PlayRandomVoices()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);

            yield return new WaitForSeconds(waitTime);

            if (voiceClips == null || voiceClips.Length == 0)
                continue;

            int randomIndex = Random.Range(0, voiceClips.Length);

            AudioClip selectedClip = voiceClips[randomIndex];

            if (selectedClip == null)
                continue;

            
            while (voiceAudio.isPlaying)
            {
                yield return null;
            }

            voiceAudio.PlayOneShot(selectedClip);

            
            yield return new WaitWhile(() => voiceAudio.isPlaying);
        }
    }

    private void OnDisable()
    {
        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }

        if (voiceAudio != null)
            voiceAudio.Stop();
    }
}
