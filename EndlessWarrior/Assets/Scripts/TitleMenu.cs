using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject titleScreenUI;
    [SerializeField] private GameObject rulesUI;
    [SerializeField] private Button playButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;

    private const string GAME_SCENE = "WaveFighter";

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
        if (rulesButton != null)
            rulesButton.onClick.AddListener(ShowRulesUI);
        if (backButton != null)
            backButton.onClick.AddListener(ShowTitleScreen);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        ShowTitleScreen();
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(StartGame);
        if (rulesButton != null)
            rulesButton.onClick.RemoveListener(ShowRulesUI);
        if (backButton != null)
            backButton.onClick.RemoveListener(ShowTitleScreen);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }

    public void ShowTitleScreen()
    {
        titleScreenUI?.SetActive(true);
        rulesUI?.SetActive(false);
        Time.timeScale = 0f;
    }

    private void ShowRulesUI()
    {
        titleScreenUI?.SetActive(false);
        rulesUI?.SetActive(true);
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GAME_SCENE);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
