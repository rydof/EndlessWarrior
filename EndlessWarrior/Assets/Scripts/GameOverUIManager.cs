using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text highestRoundText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "TitleMenu";

    private void OnEnable()
    {
        if (waveManager != null)
            waveManager.OnPlayerDeath += ShowGameOverUI;
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnDisable()
    {
        if (waveManager != null)
            waveManager.OnPlayerDeath -= ShowGameOverUI;
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.RemoveListener(ReturnToMenu);
    }

    private void ShowGameOverUI(int currentWave)
    {
        UpdateHighestRound(currentWave);
        gameObject.SetActive(true);
    }

    private void UpdateHighestRound(int currentWave)
    {
        if (highestRoundText != null)
            highestRoundText.text = $"Runda: {currentWave}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
