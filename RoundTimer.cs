using UnityEngine;
using TMPro;

public class RoundTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float roundDuration = 180f; 

    [Header("UI")]
    public TMP_Text timerText;

    [Header("References")]
    public PlayerManager playerManager;

    private float timeRemaining;
    private bool timerRunning = true;

    private void Start()
    {
        timeRemaining = roundDuration;
        AchievementManager.Instance?.BeginRound();
        UpdateTimerUI();
    }

    private void Update()
    {
        // Stop counting if the game has already ended
        if (!timerRunning || playerManager.gameOver)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            UpdateTimerUI();
            AchievementManager.Instance?.CompleteRound();
            playerManager.EndGame();
            return;
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
