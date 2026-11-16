using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference pauseAction;

    [Header("UI")]
    public GameObject pauseMenu;

    [Header("Gameplay")]
    public PlayerManager playerManager;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void OnEnable()
    {
        pauseAction.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;

        Time.timeScale = 1f;
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (playerManager.gameOver)
            return;

        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        Time.timeScale = 0f;
        playerManager.levelTheme.volume = 0.1f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        playerManager.levelTheme.volume = 0.25f;
        pauseMenu.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
