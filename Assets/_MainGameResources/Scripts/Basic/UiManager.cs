using System.Collections;
using DG.Tweening;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using TMPro;

public class UiManager : MonoBehaviour
{
    #region Prefs
    public int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("CurrentLevel", 0); }
        set { PlayerPrefs.SetInt("CurrentLevel", value); }
    }

    public int MainLevelNo
    {
        get { return PlayerPrefs.GetInt("main_levelno", 0); }
        set { PlayerPrefs.SetInt("main_levelno", value); }
    }
    
    public int Coins
    {
        get { return PlayerPrefs.GetInt("Coins", 0); }
        set { PlayerPrefs.SetInt("Coins", value); }
    }
    #endregion

    #region Public Fields
    public static UiManager instance;

    [Header("Transform")]
    public Transform ui_MainMenu;
    public Transform ui_Gameplay, coinsParent;
    public GameObject ui_tutorial, ui_gameplayHeader, releaseWaterBtn, ui_touchpad, spinPanel, coinPrefab;
    public Transform ui_LevelComplete, ui_LevelFailed;

    public Transform loading;
    
    [Header("Images")]
    public Image loadingBar;


    [Header("Text")] public Text winCoins;
    public TextMeshProUGUI levelNo, plusAnimText, coinsText;

    public float coinsRadius;
    
    public bool isLevelFail;

    public UpgardesManager _upgardesManager;
    #endregion

    #region Public Methods

    public void Init()
    {
        ui_MainMenu.gameObject.SetActive(true);
        ui_Gameplay.gameObject.SetActive(false);
        ui_LevelComplete.gameObject.SetActive(false);
        UpdateTextUI();
    }

    public void StartGame()
    {
        ui_MainMenu.gameObject.SetActive(false);
        ui_Gameplay.gameObject.SetActive(true);
    }

    [ContextMenu("SkipLevel")]
    void SkipLevel()
    {
        LevelComplete(0);
        loading.gameObject.SetActive(true);
        SceneManager.LoadScene(CurrentLevel);
    }

    public void LoadLevel()
    {
        // loading.gameObject.SetActive(true);
        SceneManager.LoadScene(0);
    }

    public void LevelComplete(float delay)
    {
        if (won || failed)
            return;

        won = true;
        ui_Gameplay.gameObject.SetActive(false);
        StartCoroutine(LevelCompleteDelay(delay));
        CurrentLevel++;
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (CurrentLevel == levelManager.maxLevels)
            CurrentLevel = 0;
        
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level_Complete", "Level_No_"+CurrentLevel.ToString());

        MainLevelNo++;
    }

    public void CoinsOnWin()
    {
        // Coins += coinsEarned + _upgardesManager.GetCurrentUpgradeValue(1);
        // winCoins.text = "+" + (coinsEarned + _upgardesManager.GetCurrentUpgradeValue(1)).ToString();
        // SetCoinsText();
        // CoinsAnim();
    }

    // [HideInInspector]
    public int coinsEarned;
    public void SetCoinsEarned()
    {
        coinsEarned = PlayerController.Instance.spawner.army.Count;
    }
    
    private IEnumerator LevelCompleteDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        //var lc_dialogue = ui_LevelComplete.Find("CompleteDialogue").gameObject;
        //var lf_dialogue = ui_LevelComplete.Find("FailDialogue").gameObject;

        //lc_dialogue.SetActive(false);
        //lf_dialogue.SetActive(false);
        
       
        //if (!isLevelFail)
        //{
        //    SoundManager.instance.PlayGameSound(SoundManager.NameOfSounds.levelFail);
        //    lc_dialogue.SetActive(true);
        //}
        //else
        //{
        //    SoundManager.instance.PlayGameSound(SoundManager.NameOfSounds.levelComplete);
        //    LevelManager.instance.LevelComplete();
        //    lf_dialogue.SetActive(true);
        //}
        ui_LevelComplete.gameObject.SetActive(true);
        CoinsOnWin();
    }
    bool won, failed;
    public void LevelFailed(float delay)
    {
        if (won || failed)
            return;

        failed = true;
        ui_Gameplay.gameObject.SetActive(false);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level_Failed", "Level_No_"+CurrentLevel.ToString());
        StartCoroutine(LevelFailedDelay(delay));
        //AnalyticsController.instance.LevelFailedAnalytics();
    }

    private IEnumerator LevelFailedDelay(float delay)
    {

        yield return new WaitForSeconds(delay);


        //var lc_dialogue = ui_LevelComplete.Find("CompleteDialogue").gameObject;
        //var lf_dialogue = ui_LevelComplete.Find("FailDialogue").gameObject;

        //lc_dialogue.SetActive(false);
        //lf_dialogue.SetActive(false);


        //if (!isLevelFail)
        //{
        //    SoundManager.instance.PlayGameSound(SoundManager.NameOfSounds.levelFail);
        //    lc_dialogue.SetActive(true);
        //}
        //else
        //{
        //    SoundManager.instance.PlayGameSound(SoundManager.NameOfSounds.levelComplete);
        //    LevelManager.instance.LevelComplete();
        //    lf_dialogue.SetActive(true);
        //}

        ui_LevelFailed.gameObject.SetActive(true);
    }

    #endregion

    #region Private methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        UpdateTextUI();

    }

    private void Start()
    {
        //AnalyticsController.instance.OnPlayAnalytics();
    }

    #endregion

    #region buttons methods

    public void GameStart()
    {
      
    }

    [Button("coins anim")]
    public void CoinsAnim()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 positionToSpawn = Random.insideUnitCircle * coinsRadius;
            GameObject objInstantiated = Instantiate(coinPrefab, coinsParent);
            objInstantiated.transform.localPosition = positionToSpawn;
            objInstantiated.transform.DOMove(coinsText.transform.position, 1).SetDelay(.75f).onComplete = () => objInstantiated.SetActive(false);
            objInstantiated.transform.DOScale(1, .55f);
            // prefabsReference.Add(objInstantiated);
        }
    }

    void SetCoinsText()
    {
        coinsText.text = Coins.ToString();
    }

    public void PlayButtonMethod()
    {
        ui_MainMenu.gameObject.SetActive(true); 
        Loading(false);
        StartCoroutine(LoadScene());
    }


    private void Loading(bool value)
    {
        loading.gameObject.SetActive(value);
       
       
    }


    private IEnumerator LoadScene()
    {
        Loading(true);
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelManager.GetSceneToLoad());
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;
            yield return null;
        }
        Loading(false);
        //CheckSetting();
        // this will create a gameobject in everyscene with SettingChecker component to enable/disable setting values
    }


    //public void _OnRestart(bool isComplete)
    //{

    //    PlayButtonHaptic();
    //    CheckSetting();// check if SettingChecker again on restart
    //    if (isComplete)
    //    {

    //        UpdateTextUI();


    //    }
    //    else
    //    {
    //        GameManager.instance.gameState = GameManager.GameStates.Ready;

    //        SceneManager.LoadSceneAsync(0);
    //    }

    //}


    public void UpdateTextUI()
    {
        if (levelNo)
            levelNo.text = (MainLevelNo + 1).ToString();
    }

    public void PlusAnimText(int n)
    {
        if (plusAnimText!=null)
        {
            plusAnimText.text = "+" + n.ToString();
            plusAnimText.gameObject.SetActive(true);
        }
    }
    //public void CheckSetting()
    //{
    //    SoundManager.instance.CheckSound();
      
    //    SettingChecker settingChecker = FindObjectOfType<SettingChecker>();
    //    if (settingChecker)
    //        Destroy(settingChecker.gameObject);

    //    GameObject go = new GameObject("SettingChecker");
    //    go.AddComponent<SettingChecker>();
    //}


    //public void PlayButtonHaptic()
    //{
    //    AL_HapticFeedBack.Generate(HapticTypes.LightImpact);
    //    if (SoundManager.instance)
    //        SoundManager.instance.PlayGameSound(SoundManager.NameOfSounds.click);
    //}
    #endregion
}