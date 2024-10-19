using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private Dictionary<GameState, IGameState> gameStates;
    private IGameState currentState;

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver,
        LevelCompleted
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    private void InitializeStates()
    {
        gameStates = new Dictionary<GameState, IGameState>
        {
            { GameState.MainMenu, new MainMenuState() },
            { GameState.Gameplay, new GameplayState() },
            { GameState.Paused, new PausedState() },
            { GameState.GameOver, new GameOverState() },
            { GameState.LevelCompleted, new LevelCompletedState() }
        };
    }

    public void ChangeState(GameState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = gameStates[newState];
        currentState.EnterState();

        Debug.Log($"Changed to {newState} state");
    }

    public void UpdateState()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }
}

public interface IGameState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}

public class MainMenuState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Main Menu State");
        // TODO: Activate main menu UI
    }

    public void UpdateState()
    {
        // Handle main menu interactions
    }

    public void ExitState()
    {
        // TODO: Deactivate main menu UI
    }
}

public class GameplayState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Gameplay State");
        // TODO: Start the game, activate gameplay elements
    }

    public void UpdateState()
    {
        // Handle gameplay logic
    }

    public void ExitState()
    {
        // TODO: Clean up gameplay elements
    }
}

public class PausedState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Paused State");
        Time.timeScale = 0;
        // TODO: Activate pause menu UI
    }

    public void UpdateState()
    {
        // Handle pause menu interactions
    }

    public void ExitState()
    {
        Time.timeScale = 1;
        // TODO: Deactivate pause menu UI
    }
}

public class GameOverState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Game Over State");
        // TODO: Activate game over UI
    }

    public void UpdateState()
    {
        // Handle game over screen interactions
    }

    public void ExitState()
    {
        // TODO: Clean up game over elements
    }
}

public class LevelCompletedState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Level Completed State");
        // TODO: Activate level completed UI
    }

    public void UpdateState()
    {
        // Handle level completed screen interactions
    }

    public void ExitState()
    {
        // TODO: Clean up level completed elements
    }
}