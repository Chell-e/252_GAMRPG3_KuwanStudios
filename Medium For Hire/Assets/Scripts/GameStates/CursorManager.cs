using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D aimCursor;
    [SerializeField] private Texture2D inspectCursor;

    [Header("Hotspot Settings")] // the clickable point of the cursor
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private void OnEnable()
    {
        // Subscribe to the GameState change event
        GameStateManager.OnStateChanged += HandleCursorChange;
    }

    private void OnDisable()
    {
        // unubscribe to avoid memory leaks
        GameStateManager.OnStateChanged -= HandleCursorChange;
    }

    private void HandleCursorChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                break;
            case GameState.Tabbed:
                break;
            case GameState.SelectionPrompt:
                break;
            case GameState.Cutscene:
                break;

        }

        return;
    }
}
