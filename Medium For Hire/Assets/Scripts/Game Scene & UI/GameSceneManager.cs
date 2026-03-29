using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene" || SceneManager.GetActiveScene().name == "BestiaryScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LoadScene("MainMenu");
            }
        }
    }

    // can either use scene name or build index
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
