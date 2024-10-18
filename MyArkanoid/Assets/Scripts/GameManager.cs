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

    private PaddleController paddleController;
    private BallController ballController;

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
        InitializeGame();
        paddleController = FindObjectOfType<PaddleController>();
        ballController = FindObjectOfType<BallController>();
    }

    private void InitializeGame()
    {
        Score = 0;
        Lives = 3;
        CurrentLevel = 1;
        IsAutoPlayEnabled = false;
        LoadHighScore();
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
        Debug.Log("Score: " + Score);
    }

    public void LoseLife()
    {
        Lives--;
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            GameOver();
        }
        Debug.Log("Lives: " + Lives);
    }

    public void AdvanceLevel()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke(CurrentLevel);
        Debug.Log("Level: " + CurrentLevel);
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        // TODO: Implement game over logic
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
        if (IsAutoPlayEnabled && paddleController != null)
        {
            paddleController.AutoMove(ballPosition.x);
        }
    }

    // TODO: Add methods for saving and loading game state
}