using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // ***  singleton stuff
    public static GameStateManager Instance;
    private void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // ***  singleton stuff

    public static event Action<GameState> OnStateChanged;
    [SerializeField] public GameState currentState;

    [SerializeField] private GameState lastState;
    void Start()
    {
        //SetState(GameState.Gameplay);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        HandleTimeScale(newState);

        OnStateChanged?.Invoke(newState);

        Debug.Log($"Game State changed to: {newState}");

        return;
    }

    public void ReturnState()
    {
        currentState = lastState;

        HandleTimeScale(lastState);

        OnStateChanged?.Invoke(lastState);

        Debug.Log($"Game State REVERTED to: {lastState}");

        return;
    }

    public void HandleTimeScale(GameState state)
    {
        if (state == GameState.Gameplay)
        {
            Time.timeScale = 1f;
        }

        if (state == GameState.Cutscene
            || state == GameState.CombatDialogue)
        {
            Time.timeScale = 0f;
        }

        if (state == GameState.CombatPrompt)
        {
            Time.timeScale = 0.1f;
        }

    }

}
