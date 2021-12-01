using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        ready,
        running,
        stop,
        levelComplete,
        levelFail,
    }

    public GameObject winCamAnim;

    public static GameManager instance;
    public GameState gameState;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Init();
    }

    private void Init()
    {
        UiManager.instance.Init();
    }

    #region functions
    public void StartGame()
    {
        UiManager.instance.StartGame();
        PlayerController.Instance.StartGame();
        // MovementController.Instance.perform = true;
    }

    public void LevelComplete()
    {
        gameState = GameState.levelComplete;
        Camera.main.transform.GetChild(0).gameObject.SetActive(true);
        UiManager.instance.LevelComplete(2);
        HapticManager.instance.Haptic_Success();
    }

    public void LevelFailed()
    {
        gameState = GameState.levelFail;
        UiManager.instance.LevelFailed(1);
        PlayerController.Instance.StopPlayer();
        if(PlayerController.Instance.curEnemyGroup)
            PlayerController.Instance.curEnemyGroup.Dance();
        if(PlayerController.Instance.enemyTower)
            PlayerController.Instance.enemyTower.StopShootingAndDance();
        HapticManager.instance.Haptic_Fail();
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    public void FinishLineCrossed()
    {
        FindObjectOfType<ProgressionBar>().gameObject.SetActive(false);
        PlayerController.Instance._movementController.HorizontalControl(false);
        PlayerController.Instance._movementController.PlaceInCenter();
        PlayerController.Instance.spawner.StairsFormation();
    }
    #endregion


}
