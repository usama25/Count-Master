using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region prefs
    public int MainLevelNo
    {
        get { return PlayerPrefs.GetInt("main_levelno", 0); }
        set { PlayerPrefs.SetInt("main_levelno", value); }
    }
    public int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("CurrentLevel", 0); }
        set { PlayerPrefs.SetInt("CurrentLevel", value); }
    }
    
    public int CurrentVersion
    {
        get { return PlayerPrefs.GetInt("CurrentVersion", 0); }
        set { PlayerPrefs.SetInt("CurrentVersion", value); }
    }
    #endregion

    #region vars
    public static LevelManager instance;

    public bool isTest;
    public int levelno;
    public bool isVariationTest;
    public GameObject curLevelObject;

    public int maxLevels = 22;
    public int maxVariations = 3;
    public List<int> levelSequence = new List<int>();
    public GameObject[] levelObjects;
    public GameObject uiLevel1;

    //public MyLevel[] _levels;

    //public MyLevel currentLevel;
    private int level;

    #endregion

    #region Private Methods

     void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


        Init();
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        if (curLevelObject)
            curLevelObject.SetActive(true);

        if (isTest)
        {
            CurrentLevel = levelno;
            MainLevelNo = levelno;
        }
        else
            levelno = CurrentLevel;

        for (int i = 0; i < maxVariations; i++)
        {
            levelObjects[CurrentLevel].transform.gameObject.SetActive(false);
        }

        curLevelObject = levelObjects[CurrentLevel].transform.gameObject;
        curLevelObject.SetActive(true);
        levelObjects[CurrentLevel].SetActive(true);
        
        
        if(CurrentLevel == 0 && uiLevel1)
            uiLevel1.SetActive(true);
    }

    public int GetSceneToLoad()
    {
        if (isTest)
            return levelno;
        else
        {
            level = (PlayerPrefs.GetInt("list_levelno")) % maxLevels;
            level = levelSequence[level % maxLevels];
            return level;
        }
    }


    public void LevelComplete()
    {

        PlayerPrefs.SetInt("main_levelno", PlayerPrefs.GetInt("main_levelno") + 1);
        PlayerPrefs.SetInt("list_levelno", PlayerPrefs.GetInt("list_levelno") + 1);
        CurrentLevel++;
        Debug.Log("hellooooooo");
    }


    public int GetLevelNo()
    {
        return PlayerPrefs.GetInt("main_levelno");

    }

    public void TaskCompleted()
    {
        //print("Task Completed");
        //currentLevel.TaskCompleted();
    }


    public void StartTask()
    {
        //currentLevel.Init();
    }
    #endregion

    public InputField inputField;

    public void SetCustomLevel()
    {
        if (inputField.text.Length == 0 || inputField.text.Length > 2)
        {
            Handheld.Vibrate();
            inputField.text = "";
            return;
        }

        int sldkjh = Int32.Parse(inputField.text) - 1;
        if (sldkjh < 0 || sldkjh >= maxLevels)
        {
            Handheld.Vibrate();
            inputField.text = "";
            return;
        }
        CurrentLevel = sldkjh;
        MainLevelNo = CurrentLevel;
        UiManager.instance.LoadLevel();
    }
}
