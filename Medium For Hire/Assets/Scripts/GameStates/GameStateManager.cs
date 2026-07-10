using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
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

    public static event Action<GameState> OnStateChanged;
    [SerializeField] public GameState currentState { get; private set; }
    void Start()
    {
        SetState(GameState.Gameplay);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        HandleTimeScale(newState);

        OnStateChanged?.Invoke(newState);

        Debug.Log($"Game State changed to: {newState}");

        return;
    }

    public void HandleTimeScale(GameState state)
    {
        Time.timeScale = (state == GameState.Gameplay) ? 1f : 0f;
    }

}
