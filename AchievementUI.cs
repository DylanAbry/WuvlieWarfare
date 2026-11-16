using UnityEngine;
using System;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    [Serializable]
    public class AchievementEntry
    {
        public Achievement achievement;
        public TMP_Text achievementText;
        public GameObject checkmark;
    }

    [Header("Achievement UI")]
    public AchievementEntry[] achievements;

    [Header("Colors")]
    public Color lockedColor = Color.white;
    public Color unlockedColor = Color.yellow;

    private void OnEnable()
    {
        RefreshAchievements();

        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementsChanged += RefreshAchievements;
    }

    private void OnDisable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementsChanged -= RefreshAchievements;
    }

    public void RefreshAchievements()
    {
        if (AchievementManager.Instance == null)
            return;

        foreach (AchievementEntry entry in achievements)
        {
            bool unlocked =
                AchievementManager.Instance.IsUnlocked(entry.achievement);

            if (entry.checkmark != null)
                entry.checkmark.SetActive(unlocked);

            if (entry.achievementText != null)
                entry.achievementText.color =
                    unlocked ? unlockedColor : lockedColor;
        }
    }
}
