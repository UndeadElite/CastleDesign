using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Health Images")]
    public Image healthFull;
    public Image healthHalf;
    public Image healthLow;
    public Image healthEmpty;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button menuButton;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button gameOverMenuButton;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Optionally lock/hide cursor at start
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOver())
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetHealth(int current, int max)
    {
        healthFull.enabled = false;
        healthHalf.enabled = false;
        healthLow.enabled = false;
        healthEmpty.enabled = false;

        switch (current)
        {
            case 3:
                healthFull.enabled = true;
                break;
            case 2:
                healthHalf.enabled = true;
                break;
            case 1:
                healthLow.enabled = true;
                break;
            default:
                healthEmpty.enabled = true;
                break;
        }
    }

    private bool IsGameOver()
    {
        return gameOverPanel != null && gameOverPanel.activeSelf;
    }
}
