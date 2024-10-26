using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Lives { get; private set; }
    public int CurrentLevel { get; private set; }
    public bool IsAutoPlayEnabled { get; private set; }
    public int MaxLevel = 3; // New field to define the maximum level


    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnLevelChanged;
    public event Action<bool> OnAutoPlayToggled;
    public event Action<GameStateManager.GameState> OnGameStateChanged;

    private PaddleController paddleController;
    private BallController ballController;
    private GameStateManager gameStateManager;
    private BrickManager brickManager;


    private void Awake()
    {
        FindGameObjects();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create GameStateManager as a child of GameManager
            GameObject gameStateManagerObject = new GameObject("GameStateManager");
            gameStateManagerObject.transform.SetParent(this.transform);
            gameStateManager = gameStateManagerObject.AddComponent<GameStateManager>();

            brickManager = FindObjectOfType<BrickManager>();
            if (brickManager == null)
            {
                Debug.LogError("BrickManager not found in the scene!");
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupEventListeners();
        LoadHighScore();
        gameStateManager.ChangeState(GameStateManager.GameState.MainMenu);

        //FindGameObjects();
        //InitializeGame();
    }

    private void Update()
    {

        if (gameStateManager != null)
        {
            gameStateManager.UpdateState();
        }

        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            ToggleAutoPlay();
            OnAutoPlayToggled?.Invoke(IsAutoPlayEnabled);

        }

        if(IsAutoPlayEnabled == true)
        {
            UpdateAutoPlay(ballController.transform.position);


        }
        LoadHighScore();

    }

    private void FindGameObjects()
    {
        paddleController = FindObjectOfType<PaddleController>();
        ballController = FindObjectOfType<BallController>();

        if (paddleController == null)
        {
            Debug.LogError("PaddleController not found in the scene.");
        }

        if (ballController == null)
        {
            Debug.LogError("BallController not found in the scene.");
        }

    }

    public void InitializeGame()
    {
        Score = 0;
        Lives = 3;
        CurrentLevel = 1;
        IsAutoPlayEnabled = false;
        UpdateUI();
        Debug.Log("Game initialized with default values");
    }

    private void SetupEventListeners()
    {
        OnScoreChanged += (score) => UIManager.Instance.UpdateScore(score);
        OnLivesChanged += (lives) => UIManager.Instance.UpdateLives(lives);
        OnLevelChanged += (level) => UIManager.Instance.UpdateLevel(level);
    }

    private void UpdateUI()
    {
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
        OnLevelChanged?.Invoke(CurrentLevel);
        OnAutoPlayToggled?.Invoke(IsAutoPlayEnabled);
    }

    public void GoToMainMenu()
    {

        InitializeGame(); // Reset game state when returning to main menu
        gameStateManager.ChangeState(GameStateManager.GameState.MainMenu);
        SaveManager.Instance.LoadGameData();
    }
    public void StartGame()
    {
        SaveManager.Instance.LoadGameData();
        InitializeGame();
        ResetGameElements(true);
        ChangeGameState(GameStateManager.GameState.Gameplay);
        Debug.Log("Starting new game");
    }

    // Modified to handle both new games and continued games
    private void ResetGameElements(bool resetBricks)
    {
        // Reset ball
        BallController ballController = FindObjectOfType<BallController>();
        if (ballController != null)
        {
            ballController.ResetBall();
        }
        else
        {
            Debug.LogWarning("BallController not found when resetting game elements");
        }

        // Reset paddle
        PaddleController paddleController = FindObjectOfType<PaddleController>();
        if (paddleController != null)
        {
            paddleController.OnBallReset();
        }
        else
        {
            Debug.LogWarning("PaddleController not found when resetting game elements");
        }

        // Reset bricks if needed (usually true for both new game and level continue)
        if (resetBricks && brickManager != null)
        {
            brickManager.ResetBricks(CurrentLevel);
        }
        else if (resetBricks)
        {
            Debug.LogWarning("BrickManager not found when resetting bricks");
        }

        // Ensure we're in the correct state
        IsAutoPlayEnabled = false;

        Debug.Log($"Game elements reset. Level: {CurrentLevel}, Reset Bricks: {resetBricks}");
    }

    public void ContinueGame(int level, int score, int lives)
    {
        // Set the game state from saved data
        Score = score;
        Lives = lives;
        CurrentLevel = level;

        // Update UI
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
        OnLevelChanged?.Invoke(CurrentLevel);

        // Reset the game elements for the current level
        ResetGameElements(true);

        // Change to gameplay state
        ChangeGameState(GameStateManager.GameState.Gameplay);

        Debug.Log($"Continuing game from Level: {level}, Score: {score}, Lives: {lives}");
    }

    public bool CanContinueGame()
    {
        return SaveManager.Instance.HasSavedGame();
    }


    public void PauseGame()
    {
        gameStateManager.ChangeState(GameStateManager.GameState.Paused);
    }

    public void ResumeGame()
    {
        gameStateManager.ChangeState(GameStateManager.GameState.Gameplay);
    }

    public void GameOver()
    {
        SaveManager.Instance.SaveGameData();
        gameStateManager.ChangeState(GameStateManager.GameState.GameOver);
    }

    public void LevelCompleted()
    {
        SaveManager.Instance.SaveGameData();
        Debug.Log($"Level {CurrentLevel} completed.");
        if (CurrentLevel >= MaxLevel)
        {
            EndGame();
        }
        else
        {
            gameStateManager.ChangeState(GameStateManager.GameState.LevelCompleted);
        }


        AudioManager.instance.PlaySoundFXClip(AudioManager.instance.complete, transform, 0.5f);

    }

    public void StartNextLevel()
    {
        AdvanceLevel();
        Debug.Log($"Starting level {CurrentLevel}");
        ResetGameElements(true);
        ChangeGameState(GameStateManager.GameState.Gameplay);
        ballController.ResetBall();
    }

    private void EndGame()
    {
        ChangeGameState(GameStateManager.GameState.EndGame);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void ChangeGameState(GameStateManager.GameState newState)
    {
        gameStateManager.ChangeState(newState);
        OnGameStateChanged?.Invoke(newState);

    }

    private void TogglePause()
    {
        if (gameStateManager.currentState is GameplayState)
        {
            PauseGame();
        }
        else if (gameStateManager.currentState is PausedState)
        {
            ResumeGame();
        }

        AudioManager.instance.PlaySoundFXClip(AudioManager.instance.select, transform, 0.5f);

    }

    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);

        if (Score > HighScore)
        {
            HighScore = Score;
            //SaveHighScore();
        }
    }

    public void LoseLife()
    {
        Lives--;
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            GameOver();
        }
        else
        {
            ResetGameElements(false);
        }
    }

    public void AdvanceLevel()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    private void LoadHighScore()
    {
        var gameData = SaveManager.Instance.GetGameData();
        HighScore = gameData.highScore;
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }

    public void ToggleAutoPlay()
    {
        IsAutoPlayEnabled = !IsAutoPlayEnabled;
        OnAutoPlayToggled?.Invoke(IsAutoPlayEnabled);
    }

    public void UpdateAutoPlay(Vector2 ballPosition)
    {
        if (IsAutoPlayEnabled)
        {
            PaddleController paddleController = FindObjectOfType<PaddleController>();
            if (paddleController != null)
            {
                paddleController.AutoMove(ballPosition.x);
            }
        }
    }
}