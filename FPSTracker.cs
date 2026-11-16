using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    private float timer;
    private int frames;

    void Update()
    {
        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= 1f)
        {
            float fps = frames / timer;
            Debug.Log("FPS: " + Mathf.RoundToInt(fps));

            frames = 0;
            timer = 0f;
        }
    }
}
