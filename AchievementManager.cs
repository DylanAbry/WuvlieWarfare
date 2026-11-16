using UnityEngine;
using System;
using System.Collections.Generic;

public enum Achievement
{
    Smithereens,
    Peacemaker,
    LuckyCharm,
    PointFarmer,
    VIP,
    MintCondition,
    SweetTooth,
    SugarRush,
    PlushyPurger,
    GunshotGuru,
    WuvlieWargod,
    Overachiever
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    public event Action OnAchievementsChanged;

    private const string Prefix = "Achievement_";

    private int wuvlieKills;
    private int treatBonuses;
    private int flawlessRounds;

    private bool tookDamageThisRound;
    private bool dealtWuvlieDamageThisRound;
    private bool openedBombThisRound;
    private bool roundFinished;

    private readonly HashSet<int> openedChests = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        wuvlieKills = PlayerPrefs.GetInt("TotalWuvlieKills", 0);
        treatBonuses = PlayerPrefs.GetInt("TotalTreatBonuses", 0);
        flawlessRounds = PlayerPrefs.GetInt("FlawlessRounds", 0);
    }

    public bool IsUnlocked(Achievement achievement)
    {
        return PlayerPrefs.GetInt(Prefix + achievement, 0) == 1;
    }

    public void Unlock(Achievement achievement)
    {
        if (IsUnlocked(achievement))
            return;

        PlayerPrefs.SetInt(Prefix + achievement, 1);
        PlayerPrefs.Save();

        Debug.Log("Achievement unlocked: " + achievement);

        if (achievement != Achievement.Overachiever)
            CheckOverachiever();

        OnAchievementsChanged?.Invoke();
    }

    private void CheckOverachiever()
    {
        foreach (Achievement achievement in Enum.GetValues(typeof(Achievement)))
        {
            if (achievement == Achievement.Overachiever)
                continue;

            if (!IsUnlocked(achievement))
                return;
        }

        Unlock(Achievement.Overachiever);
    }

    public void BeginRound()
    {
        tookDamageThisRound = false;
        dealtWuvlieDamageThisRound = false;
        openedBombThisRound = false;
        roundFinished = false;
        openedChests.Clear();
    }

    public void PlayerTookDamage()
    {
        tookDamageThisRound = true;
    }

    public void PlayerDealtWuvlieDamage()
    {
        dealtWuvlieDamageThisRound = true;
    }

    public void BombKilledPlayer()
    {
        Unlock(Achievement.Smithereens);
    }

    public void ChestOpened(int chestId, bool isBomb)
    {
        if (roundFinished)
            return;

        if (!openedChests.Add(chestId))
            return;

        if (isBomb)
            openedBombThisRound = true;
    }

    public void ScoreUpdated(int currentScore)
    {
        if (currentScore >= 50)
            Unlock(Achievement.PointFarmer);
    }

    public void AllMapsUnlocked()
    {
        Unlock(Achievement.VIP);
    }

    public void TreatBonusWon()
    {
        treatBonuses++;

        PlayerPrefs.SetInt("TotalTreatBonuses", treatBonuses);
        PlayerPrefs.Save();

        Unlock(Achievement.SweetTooth);

        if (treatBonuses >= 5)
            Unlock(Achievement.SugarRush);
    }

    public void WuvlieDefeated()
    {
        wuvlieKills++;

        PlayerPrefs.SetInt("TotalWuvlieKills", wuvlieKills);
        PlayerPrefs.Save();

        if (wuvlieKills >= 50)
            Unlock(Achievement.PlushyPurger);

        if (wuvlieKills >= 100)
            Unlock(Achievement.GunshotGuru);

        if (wuvlieKills >= 500)
            Unlock(Achievement.WuvlieWargod);
    }

    public void CompleteRound()
    {
        if (roundFinished)
            return;

        roundFinished = true;

        if (!tookDamageThisRound)
        {
            flawlessRounds++;

            PlayerPrefs.SetInt("FlawlessRounds", flawlessRounds);
            PlayerPrefs.Save();

            if (flawlessRounds >= 10)
                Unlock(Achievement.MintCondition);
        }

        if (!tookDamageThisRound && !dealtWuvlieDamageThisRound)
            Unlock(Achievement.Peacemaker);

        if (!openedBombThisRound && openedChests.Count == 3)
        {
            Unlock(Achievement.LuckyCharm);
        }
    }
}
