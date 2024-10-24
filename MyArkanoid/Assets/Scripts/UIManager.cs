using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletedPanel;
    public GameObject endGamePanel; // New panel for EndGame screen

    [Header("Gameplay UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI highScoreText;



    [Header("Buttons")]
    public Button startGameButton;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;
    public Button goBackToMainMenu;
    public Button nextLevelButton;
    public Button endGameMainMenuButton; // New button for EndGame screen
    public Button endGameQuitButton; // New button for EndGame screen
    public Button continueGameButton;
    public Button deleteSaveDataButton;
    public Button autoPlayToggleButton;


    [Header("Controlls")]
    public Slider paddleSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupButtonListeners();
    }

    private void Update()
    {
        UpdateMainMenuUI();
    }

    private void SetupButtonListeners()
    {
        startGameButton.onClick.AddListener(GameManager.Instance.StartGame);
        resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);
        restartButton.onClick.AddListener(GameManager.Instance.StartGame);
        quitButton.onClick.AddListener(Application.Quit);
        goBackToMainMenu.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        nextLevelButton.onClick.AddListener(GameManager.Instance.StartNextLevel);
        endGameMainMenuButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        endGameQuitButton.onClick.AddListener(GameManager.Instance.QuitGame);
        continueGameButton?.onClick.AddListener(ContinueGame);
        deleteSaveDataButton?.onClick.AddListener(DeleteSaveData);
        autoPlayToggleButton?.onClick.AddListener(ToggleAutoPlay);

    }

    public void ShowPanel(GameStateManager.GameState state)
    {
        HideAllPanels();
        switch (state)
        {
            case GameStateManager.GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case GameStateManager.GameState.Gameplay:
                gameplayPanel.SetActive(true);
                break;
            case GameStateManager.GameState.Paused:
                pausePanel.SetActive(true);
                break;
            case GameStateManager.GameState.GameOver:
                gameOverPanel.SetActive(true);
                break;
            case GameStateManager.GameState.LevelCompleted:
                levelCompletedPanel.SetActive(true);
                break;
            case GameStateManager.GameState.EndGame:
                endGamePanel.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        levelCompletedPanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
    }

    private void ShowGameplay()
    {
        gameplayPanel.SetActive(true);
        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        paddleSlider.gameObject.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
        // Keep gameplay UI visible in pause state
        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        // Optionally show final score
        scoreText.gameObject.SetActive(true);
    }

    private void ShowLevelCompleted()
    {
        levelCompletedPanel.SetActive(true);
        // Show score and level
        scoreText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level: {level}";
    }

    public void UpdateMainMenuUI()
    {
        if (continueGameButton != null)
        {
            continueGameButton.interactable = SaveManager.Instance.HasSavedGame();
        }

        if (highScoreText != null)
        {
            var gameData = SaveManager.Instance.GetGameData();
            highScoreText.text = $"High Score: {gameData.highScore}";
        }

        if(autoPlayToggleButton != null)
        {
            autoPlayToggleButton.interactable = true;
        }
    }

    private void ContinueGame()
    {
        var gameData = SaveManager.Instance.GetGameData();
        GameManager.Instance.ContinueGame(gameData.currentLevel, gameData.currentScore, gameData.remainingLives);
    }

    private void DeleteSaveData()
    {
        SaveManager.Instance.DeleteAllData(); // or DeleteGameData() if you want to keep settings
        UpdateMainMenuUI();
    }

    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    private void ToggleAutoPlay()
    {

        GameManager.Instance.ToggleAutoPlay();
    }
}