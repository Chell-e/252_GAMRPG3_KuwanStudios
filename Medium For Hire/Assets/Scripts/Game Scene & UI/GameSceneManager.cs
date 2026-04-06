using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// MANAGES SCENE TRANSITIONS
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public static string previousSceneName;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LoadPreviousScene();
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
        SceneManager.LoadScene(previousSceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }


}
