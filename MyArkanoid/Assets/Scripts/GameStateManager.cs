using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    private Dictionary<GameState, IGameState> gameStates;
    internal static readonly object Instance;

    public IGameState currentState { get; private set; }


    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver,
        LevelCompleted,
        EndGame // New state
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
            { GameState.LevelCompleted, new LevelCompletedState() },
            { GameState.EndGame, new EndGameState() }
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
        GameplayManager.Instance.DeactivateGameplayElements();
    }

    public void UpdateState() { }

    public void ExitState() { }
}

public class GameplayState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered Gameplay State");
        GameplayManager.Instance.ActivateGameplayElements();
        
    }

    public void UpdateState()
    {
        // Handle ongoing gameplay logic if needed
    }

    public void ExitState()
    {
        // Any cleanup needed when exiting gameplay state
    }
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
        GameplayManager.Instance.DeactivateGameplayElements();
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
        GameplayManager.Instance.DeactivateGameplayElements();
    }

    public void UpdateState() { }

    public void ExitState() { }
}

public class EndGameState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entered End Game State");
        GameplayManager.Instance.DeactivateGameplayElements();
    }

    public void UpdateState() { }

    public void ExitState() { }
}