using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{

    public Scorekeeper scoreInfo;
    public ChestManager chestInfo;
    public ContinuousMoveProvider moveProvider;

    public AudioSource levelTheme;

    [Header("Player Health")]
    public int lives = 3;
    public GameObject[] hearts;

    [Header("Bonus Treats")]
    public GameObject[] treatPrefabs;
    public Transform[] treatSpawnPoints;
    public GameObject[] treatIcons;
    public Animator bonusTreatAnim;
    public RawImage treatPanel;


    [Header("Disable On Game Over")]
    public Behaviour[] movementComponents;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] grabInteractors;

    public float treatSpawnInterval = 30f;
    public float treatLifetime = 10f;

    private HashSet<int> bonusTreats = new HashSet<int>();

    public bool gameOver = false;
    private bool bonusAwarded = false;
    private bool endGameProcessed = false;
    private bool isInvincible = false;

    public GameObject summaryBackground;
    public GameObject gameSummary;
    public GameObject rayInteractor;
    public GameObject damagePanel;

    public TextMeshProUGUI wuvlieScoreText;
    public TextMeshProUGUI chestScoreText;
    public TextMeshProUGUI bonusTreatText;
    public TextMeshProUGUI totalScoreText;

    public int totalScore;
    public int mapNumber;

    public AudioSource damageSound;
    public AudioSource treatSpawnSound;
    public AudioSource treatCollectedSound;

    private void Start()
    {
        totalScore = 0;
        lives = 3;
        bonusTreats.Clear();
        gameSummary.SetActive(false);
        summaryBackground.SetActive(false);
        rayInteractor.SetActive(true);
        damagePanel.SetActive(false);

        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }

        foreach (GameObject treat in treatIcons)
        {
            treat.SetActive(false);
        }

        StartCoroutine(SpawnTreats());
    }

    public void TakePlushyDamage()
    {
        if (isInvincible || gameOver)
            return;

        damageSound.Play();
        lives--;
        AchievementManager.Instance?.PlayerTookDamage();

        if (lives >= 0 && lives < hearts.Length)
        {
            hearts[lives].SetActive(false);
        }

        Debug.Log("Player hit by plushy! Lives remaining: " + lives);

        if (lives <= 0)
        {
            EndGame();
            Debug.Log("Player defeated!");
        }
        else
        {
            StartCoroutine(DamageReaction());
        }
    }

    private IEnumerator DamageReaction()
    {
        isInvincible = true;
        damagePanel.SetActive(true);
        damagePanel.GetComponent<Animator>().Play("TakeDamage");
        yield return new WaitForSeconds(2f);
        damagePanel.SetActive(false);
        isInvincible = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Water"))
        {
            moveProvider.moveSpeed = 6f;
        }


        if (gameOver)
            return;

        if (!other.CompareTag("Treat"))
            return;

        // Find which treat prefab this instance represents.
        int treatID = -1;

        for (int i = 0; i < treatPrefabs.Length; i++)
        {
            if (other.transform.root.name ==
                treatPrefabs[i].name + "(Clone)")
            {
                treatID = i;
                break;
            }
        }

        if (treatID == -1)
            return;



        // HashSet.Add returns false for duplicates.
        if (bonusTreats.Add(treatID))
        {
            Debug.Log("Treat collected: " +
                bonusTreats.Count + "/" + treatPrefabs.Length);

            treatIcons[treatID].SetActive(true);
            StartCoroutine(UpdateTreatCollection());

            if (bonusTreats.Count == treatPrefabs.Length &&
                !bonusAwarded)
            {
                bonusAwarded = true;
                treatPanel.color = Color.yellow;
                AchievementManager.Instance?.TreatBonusWon();

                Debug.Log("All treats collected! +15 points!");

                // Add 15 points to your Scorekeeper here.
            }
        }

        Destroy(other.transform.root.gameObject);
    }

    private IEnumerator SpawnTreats()
    {
        while (!gameOver &&
               bonusTreats.Count < treatPrefabs.Length)
        {
            yield return new WaitForSeconds(treatSpawnInterval);

            if (gameOver ||
                bonusTreats.Count == treatPrefabs.Length)
                yield break;

            List<int> availableTreats = new List<int>();

            for (int i = 0; i < treatPrefabs.Length; i++)
            {
                if (!bonusTreats.Contains(i))
                    availableTreats.Add(i);
            }

            if (availableTreats.Count == 0 ||
                treatSpawnPoints.Length == 0)
                yield break;

            int treatID = availableTreats[
                Random.Range(0, availableTreats.Count)
            ];

            Transform spawnPoint = treatSpawnPoints[
                Random.Range(0, treatSpawnPoints.Length)
            ];

            Vector3 spawnPosition = spawnPoint.position;

            if (mapNumber == 1)
            {
                switch (treatID)
                {
                    case 3:
                        spawnPosition.y = 3.392f;
                        break;
                    case 4:
                        spawnPosition.y = 3.87f;
                        break;
                }
            }
            else if (mapNumber == 2)
            {
                switch (treatID)
                {
                    case 3:
                        spawnPosition.y = 4f;
                        break;
                    case 4:
                        spawnPosition.y = 4f;
                        break;
                }
            }
            else
            {
                switch (treatID)
                {
                    case 3:
                        spawnPosition.y = -7f;
                        break;
                    case 4:
                        spawnPosition.y = -7f;
                        break;
                }
            }
            

            GameObject treat = Instantiate(
                treatPrefabs[treatID],
                spawnPosition,
                spawnPoint.rotation
            );
            treatSpawnSound.Play();

            Destroy(treat, treatLifetime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            moveProvider.moveSpeed = 8f;
        }
        
    }

    private IEnumerator UpdateTreatCollection()
    {
        treatCollectedSound.Play();
        bonusTreatAnim.Play("BonusTreatAppear");
        yield return new WaitForSeconds(10f);
        bonusTreatAnim.Play("BonusTreatDisappear");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Bomb"))
        {
            Debug.Log("Player touched fire particles!");
            foreach (GameObject heart in hearts)
            {
                heart.SetActive(false);
            }
            EndGame();
        }
    }

    public void EndGame()
    {
        if (endGameProcessed)
            return;

        endGameProcessed = true;
        gameOver = true;

        levelTheme.volume = 0.1f;

        GameObject[] plushies = GameObject.FindGameObjectsWithTag("Plushy");

        for (int i = 0; i < plushies.Length; i++)
        {
            Destroy(plushies[i]);
        }

        foreach (Behaviour movement in movementComponents)
        {
            if (movement != null)
                movement.enabled = false;
        }

        // Disable object grabbing
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor in grabInteractors)
        {
            if (interactor != null)
                interactor.enabled = false;
        }

        summaryBackground.SetActive(true);

        wuvlieScoreText.text = string.Format("Wuvlie Score: " + scoreInfo.score);
        // Chest score bonus logic here

        if (bonusTreats.Count == treatPrefabs.Length)
        {
            bonusTreatText.text = string.Format("Bonus Treat Score: 15");

            if (chestInfo.chestBonusOpened)
            {
                chestScoreText.text = string.Format("Chest Score: " + chestInfo.chestBonusScore);
                totalScore = scoreInfo.score + chestInfo.chestBonusScore + 15;
                AchievementManager.Instance?.ScoreUpdated(totalScore);
            }
            else
            {
                chestScoreText.text = string.Format("Chest Score: 0");
                totalScore = scoreInfo.score + 15;
            }
            
        }
        else
        {
            bonusTreatText.text = string.Format("Bonus Treat Score: 0");
            if (chestInfo.chestBonusOpened)
            {
                chestScoreText.text = string.Format("Chest Score: " + chestInfo.chestBonusScore);
                totalScore = scoreInfo.score + chestInfo.chestBonusScore;
                AchievementManager.Instance?.ScoreUpdated(totalScore);
            }
            else
            {
                chestScoreText.text = string.Format("Chest Score: 0");
                totalScore = scoreInfo.score;
            }
        }

        totalScoreText.text = string.Format("Total Score: " + totalScore);

        if (MapProgressManager.Instance != null)
        {
            Debug.Log($"Attempting to save: Map {mapNumber}, Score {totalScore}");

            MapProgressManager.Instance.SaveScore(mapNumber, totalScore);

            Debug.Log($"High score after saving: {MapProgressManager.Instance.GetHighScore(mapNumber)}");
        }
        else
        {
            Debug.LogError("MapProgressManager.Instance is NULL!");
        }
        StartCoroutine(ShowSummary());
    }

    private IEnumerator ShowSummary()
    {
        yield return new WaitForSeconds(1f);
        gameSummary.SetActive(true);
        rayInteractor.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

