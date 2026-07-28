using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// MANAGES SCENE TRANSITIONS
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public static string previousSceneName;
    public GameObject settingsPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene"
            || SceneManager.GetActiveScene().name == "AnitoBook")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LoadPreviousScene();
            }
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                settingsPanel.SetActive(false);
            }
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (StageManager.Instance.isGameOver) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {

                if (GameStateManager.Instance.stateStack.Peek() != GameState.Options)
                {
                    UIManager.Instance.TogglePauseScreen();

                }
                else if (GameStateManager.Instance.stateStack.Peek() == GameState.Options)
                {
                    UIManager.Instance.ToggleOptionsPanel();

                }
            }
        }
    }

    // can either use scene name or build index
    public void LoadScene(string sceneName)
    {
        // store current scene as a previous scene 
        // this is for the ESC button on some scenes (Bestiary, Shop)
        previousSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);

    }

    public void LoadPreviousScene()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene")
        {
            Debug.Log("SHOP SCENE DETECTED");
            SceneManager.LoadScene("MainMenu");
            return;
        }
        SceneManager.LoadScene(previousSceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }


}
