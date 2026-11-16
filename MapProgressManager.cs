using UnityEngine;

public class MapProgressManager : MonoBehaviour
{
    public static MapProgressManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null)
            return;

        GameObject managerObject = new GameObject("MapProgressManager");
        managerObject.AddComponent<MapProgressManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetHighScore(int mapNumber)
    {
        return PlayerPrefs.GetInt("MapHighScore_" + mapNumber, 0);
    }

    public bool IsMapUnlocked(int mapNumber)
    {
        if (mapNumber == 1)
            return true;

        if (mapNumber < 1 || mapNumber > 3)
            return false;

        return PlayerPrefs.GetInt("MapUnlocked_" + mapNumber, 0) == 1;
    }

    public void SaveScore(int mapNumber, int finalScore)
    {
        if (mapNumber < 1 || mapNumber > 3)
        {
            Debug.LogError("Invalid map number: " + mapNumber);
            return;
        }

        int previousHighScore = GetHighScore(mapNumber);

        if (finalScore > previousHighScore)
        {
            PlayerPrefs.SetInt("MapHighScore_" + mapNumber, finalScore);

            Debug.Log("New high score saved for Map " +
                      mapNumber + ": " + finalScore);
        }

        if (mapNumber == 1 && finalScore >= 20)
        {
            PlayerPrefs.SetInt("MapUnlocked_2", 1);
        }

        if (mapNumber == 2 && finalScore >= 35)
        {
            PlayerPrefs.SetInt("MapUnlocked_3", 1);
            AchievementManager.Instance?.AllMapsUnlocked();
        }

        PlayerPrefs.Save();
    }
}
