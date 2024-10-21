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
    }

    public void GoToMainMenu()
    {
        gameStateManager.ChangeState(GameStateManager.GameState.MainMenu);
    }
    public void StartGame()
    {
        InitializeGame();
        ResetGameElements(true);
        ChangeGameState(GameStateManager.GameState.Gameplay);
    }

    private void ResetGameElements(bool resetBricks)
    {
        BallController ballController = FindObjectOfType<BallController>();
        if (ballController != null)
        {
            ballController.ResetBall();
        }

        PaddleController paddleController = FindObjectOfType<PaddleController>();
        if (paddleController != null)
        {
            paddleController.OnBallReset();
        }

        if (resetBricks && brickManager != null)
        {
            brickManager.ResetBricks(CurrentLevel);
        }
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
        gameStateManager.ChangeState(GameStateManager.GameState.GameOver);
    }

    public void LevelCompleted()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke(CurrentLevel);
        gameStateManager.ChangeState(GameStateManager.GameState.LevelCompleted);
    }

    public void StartNextLevel()
    {
        ResetGameElements(true);
        ChangeGameState(GameStateManager.GameState.Gameplay);
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
    }

    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);

        if (Score > HighScore)
        {
            HighScore = Score;
            SaveHighScore();
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
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
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