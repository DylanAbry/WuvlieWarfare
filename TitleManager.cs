using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public Animator transitionAnim;
    public GameObject plushPanel;
    public GameObject raycast;
    public GameObject mapSelectPanel;
    public GameObject helpPanel;
    public GameObject creditsPanel;
    public GameObject achievementsPanel;

    public GameObject controlsPanel;
    public GameObject rulesPanel;

    public GameObject titleLayout;
    public Animator titleAnim;

    public TextMeshProUGUI mapOneHighScoreText;
    public TextMeshProUGUI mapTwoHighScoreText;
    public TextMeshProUGUI mapThreeHighScoreText;

    public GameObject mapTwoCoverUp;
    public GameObject mapThreeCoverUp;

    public CanvasGroup canvasGroup;

    public AudioSource wwTitleIntro;
    public AudioSource mainTheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raycast.SetActive(false);
        helpPanel.SetActive(false);
        StartCoroutine(TitleSequence());

        RefreshHighScores();
        mapOneHighScoreText.text = "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(1);
        mapTwoHighScoreText.text = "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(2);
        mapThreeHighScoreText.text = "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(3);
        
    }

    private IEnumerator TitleSequence()
    {
        yield return new WaitForSeconds(5f);
        wwTitleIntro.Play();
        yield return new WaitForSeconds(13f);
        transitionAnim.Play("TitleFadeIn");
        yield return new WaitForSeconds(0.45f);
        plushPanel.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        transitionAnim.Play("TitleFadeOut");
        mainTheme.Play();
        yield return new WaitForSeconds(0.5f);
        raycast.SetActive(true);
    }

    public void LoadLevel(string sceneName)
    {
        int mapNumber = 0;

        switch (sceneName)
        {
            case "Map1Circus":
                mapNumber = 1;
                break;

            case "Map2ClementinePool":
                mapNumber = 2;
                break;

            case "Map3LowGPark":
                mapNumber = 3;
                break;
        }

        if (mapNumber == 0)
        {
            Debug.LogError("Unknown map scene: " + sceneName);
            return;
        }

        if (!MapProgressManager.Instance.IsMapUnlocked(mapNumber))
        {
            Debug.Log("Map " + mapNumber + " is locked!");
            return;
        }

        DisableUIInput();
        raycast.SetActive(false);
        StartCoroutine(LoadLevelSequence(sceneName));
    }

    void Update()
    {
        if (!MapProgressManager.Instance.IsMapUnlocked(2))
        {
            mapTwoCoverUp.SetActive(true);
        }
        else
        {
            mapTwoCoverUp.SetActive(false);
        }

        if (!MapProgressManager.Instance.IsMapUnlocked(3))
        {
            mapThreeCoverUp.SetActive(true);
        }
        else
        {
            mapThreeCoverUp.SetActive(false);
        }
    }

    private IEnumerator LoadLevelSequence(string sceneName)
    {
        // Play transition
        transitionAnim.Play("TitleFadeIn");

        // Wait for transition to finish
        yield return new WaitForSeconds(0.5f);

        // Load whichever scene was passed in
        SceneManager.LoadScene(sceneName);
    }

    public void PressPlay()
    {

        StartCoroutine(MapLoadSequence());
    }
    public void PressCredits()
    {

        StartCoroutine(CreditsLoadSequence());
    }

    private IEnumerator MapLoadSequence()
    {
        DisableUIInput();
        titleLayout.SetActive(false);
        titleAnim.Play("TitleRemove");
        yield return new WaitForSeconds(0.5f);
        creditsPanel.SetActive(false);
        titleAnim.Play("MapFadeIn");
        yield return new WaitForSeconds(0.5f);
        EnableUIInput();
    }
    private IEnumerator CreditsLoadSequence()
    {
        DisableUIInput();
        titleLayout.SetActive(false);
        titleAnim.Play("TitleRemove");
        yield return new WaitForSeconds(0.5f);
        titleAnim.Play("CreditsFadeIn");
        yield return new WaitForSeconds(0.5f);
        EnableUIInput();
    }

    public void BackToTitle()
    {
        StartCoroutine(TitleLoadSequence());
    }
    public void CreditBackToTitle()
    {
        StartCoroutine(CreditTitleLoadSequence());
    }

    private IEnumerator TitleLoadSequence()
    {
        DisableUIInput();
        titleAnim.Play("MapFadeOut");
        yield return new WaitForSeconds(0.5f);
        creditsPanel.SetActive(true);
        titleLayout.SetActive(true);
        titleAnim.Play("TitleEnter");
        yield return new WaitForSeconds(0.5f);
        EnableUIInput();
    }

    private IEnumerator CreditTitleLoadSequence()
    {
        DisableUIInput();
        titleAnim.Play("CreditsFadeOut");
        yield return new WaitForSeconds(0.5f);
        titleLayout.SetActive(true);
        titleAnim.Play("TitleEnter");
        yield return new WaitForSeconds(0.5f);
        EnableUIInput();
    }

    public void DisableUIInput()
    {
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void EnableUIInput()
    {

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void RefreshHighScores()
    {
        if (MapProgressManager.Instance == null)
            return;

        mapOneHighScoreText.text =
            "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(1);

        mapTwoHighScoreText.text =
            "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(2);

        mapThreeHighScoreText.text =
            "HIGH SCORE: " + MapProgressManager.Instance.GetHighScore(3);

        Debug.Log(
        "TITLE HIGH SCORES: " +
        "Map 1 = " + MapProgressManager.Instance.GetHighScore(1) +
        ", Map 2 = " + MapProgressManager.Instance.GetHighScore(2) +
        ", Map 3 = " + MapProgressManager.Instance.GetHighScore(3)
        );
    }

    public void OpenHelpPage()
    {
        mapSelectPanel.SetActive(false);
        helpPanel.SetActive(true);
    }
    public void CloseHelpPage()
    {
        mapSelectPanel.SetActive(true);
        helpPanel.SetActive(false);
    }
    public void OpenRulesPage()
    {
        controlsPanel.SetActive(false);
        rulesPanel.SetActive(true);
    }
    public void OpenControlsPage()
    {
        controlsPanel.SetActive(true);
        rulesPanel.SetActive(false);
    }

    public void OpenAchievements()
    {
        achievementsPanel.SetActive(true);
        mapSelectPanel.SetActive(false);
    }
    public void CloseAchievements()
    {
        achievementsPanel.SetActive(false);
        mapSelectPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
