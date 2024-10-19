using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    private Dictionary<GameState, IGameState> gameStates;
    public IGameState currentState;

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
        InitializeStates();
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

        UIManager.Instance.ShowPanel(newState);

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

    }

    public void UpdateState() { }

    public void ExitState() { }
}

public class GameplayState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Gameplay State");
        GameManager.Instance.InitializeGame();
    }

    public void UpdateState()
    {
        // Handle gameplay logic
    }

    public void ExitState() { }
}

public class PausedState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Paused State");
        Time.timeScale = 0;
    }

    public void UpdateState() { }

    public void ExitState()
    {
        Time.timeScale = 1;
    }
}

public class GameOverState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Game Over State");
    }

    public void UpdateState() { }

    public void ExitState() { }
}

public class LevelCompletedState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Level Completed State");
        GameManager.Instance.AdvanceLevel();
    }

    public void UpdateState() { }

    public void ExitState() { }
}