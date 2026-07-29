using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
//using static UnityEditorInternal.VersionControl.ListControl;

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

    [SerializeField] public Stack<GameState> stateStack = new Stack<GameState>();
    //[SerializeField] public GameState lastState;

    [SerializeField] private Texture2D defaultCursor; // optional: leave null to use OS default

    void Start()
    {
        //SetState(GameState.Gameplay);
        stateStack.Push(GameState.Gameplay);
        HandleTimeScale(stateStack.Peek());
    }

    public void SetState(GameState newState)
    {
        if (newState != stateStack.Peek())
        {
            stateStack.Push(newState);
        }

        HandleTimeScale(stateStack.Peek());

        OnStateChanged?.Invoke(stateStack.Peek());

        Debug.Log($"Game State changed to: {newState}");

        if (stateStack.Peek() != GameState.Gameplay)
        {
            Vector2 hotspotDefault = new Vector2(5, 1);
            UnityEngine.Cursor.SetCursor(defaultCursor, hotspotDefault, CursorMode.Auto);
        }

    }

    public void PreviousState()
    {
        stateStack.Pop();

        HandleTimeScale(stateStack.Peek());

        OnStateChanged?.Invoke(stateStack.Peek());

        Debug.Log($"Game State REVERTED to: {stateStack.Peek()}");
    }


    public void HandleTimeScale(GameState state)
    {
        if (state == GameState.Gameplay)
        {
            Time.timeScale = 1f;
        }

        if (state == GameState.Pause
            || state == GameState.InfoTab
            || state == GameState.Options
            || state == GameState.ShrinePanel
            || state == GameState.Dialogue
            || state == GameState.UpgradePanel
            || state == GameState.GameOver)
        {
            Time.timeScale = 0f;
        }
    }

}
