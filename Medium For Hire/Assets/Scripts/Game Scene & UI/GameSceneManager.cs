using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // if there's an instance already, destroy this
            Destroy(this.gameObject);
        }
    }

    // can either use scene name or build index
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
