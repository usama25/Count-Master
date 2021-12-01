using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    public int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("CurrentLevel", 0); }
        set { PlayerPrefs.SetInt("CurrentLevel", value); }
    }

    public GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        //if (CurrentLevel == SceneManager.sceneCountInBuildSettings)
        //    CurrentLevel = 1;
        //SceneManager.LoadScene(CurrentLevel);
        SceneManager.LoadScene(1);
    }
    public int customLevel;

    [ContextMenu("SetCustomLevel")]

    public void SetCustomLevel()
    {
        CurrentLevel = customLevel;
    }
}
