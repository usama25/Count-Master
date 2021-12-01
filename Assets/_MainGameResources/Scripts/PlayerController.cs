using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    #region prefs
    
    public int CurrentCharacter
    {
        get { return PlayerPrefs.GetInt("CurrentCharacter", 0); }
        set { PlayerPrefs.SetInt("CurrentCharacter", value); }
    }
    #endregion
    
    #region vars
    public EnemiesGroup curEnemyGroup;
    public EnemyTower enemyTower;

    [HideInInspector]
    public Spawning spawner;

    [HideInInspector]
    public MovementController _movementController;
    public Material playerMat;
    public GameObject playerRingAnim;
    #endregion

    #region unity-methods

    private void OnEnable()
    {
        // if (LevelManager.instance)
        // {
        //     print("called from PlayerController");
        //     LevelManager.instance.Init();
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponent<Spawning>();
        _movementController = GetComponent<MovementController>();
    }
    #endregion

    #region other-methdos
    public void SetCurrentEnemyGroup(EnemiesGroup grp = null)
    {
        curEnemyGroup = grp;
    }

    public void SoldierDied(GameObject enemySoldier, GameObject playerSoldier, bool isArcher = false)
    {
        if(curEnemyGroup)
            curEnemyGroup.SoldierDied(enemySoldier);
        
        // archers can ony kill player with arrows
        if(!isArcher)
           spawner.SoldierDied(playerSoldier);
    }

    public void AddSoldiers(int n)
    {
        spawner.AddMore(n);
    }

    public void PullPlayer(Transform target)
    {
        _movementController.MoveTowardsTarget(target);
    }

    public void NormalControls(float delay)
    {
        StartCoroutine(normalControlsDelay(delay));
    }

    IEnumerator normalControlsDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NormalControls();
    }
    public void NormalControls()
    {
        _movementController.NormalControls();
    }

    public void SoldiersRun()
    {
        spawner.AllSoldiersPlay("Run");
    }
    
    public void SoldiersIdle()
    {
        spawner.AllSoldiersPlay("Idle");
    }
    
    public void SoldiersDance()
    {
        spawner.AllSoldiersPlay("Dance");
    }

    public void StartGame()
    {
        NormalControls();
        SoldiersRun();
    }

    public void StopPlayer()
    {
        _movementController.StopEverything();
    }
    
    public void HorizontalControl(bool state)
    {
        _movementController.HorizontalControl(state);
    }

    [ContextMenu("AssignArrowsTarget")]
    public void AssignArrowsTarget()
    {
        ParticlesController particlesController = ParticlesController.Instance;
        for (int i = 0; i < particlesController._customParticles[2].poolSystem.pool.Count; i++)
        {
            particlesController._customParticles[2].poolSystem.pool[i].GetComponent<Arrow>().target = transform;
        }
    }

    public void TurnTargetFollowing(bool state)
    {
        _movementController.MoveTowardsTarget(state);
    }

    public void ChangeAndApplyPlayerMaterial(Material mat)
    {
        playerMat = mat;
        playerRingAnim.SetActive(true);
        spawner.ChangeSoldiersMaterial(mat);
        AnimatePlayerColour();
    }

    void AnimatePlayerColour()
    {
        playerMat.DOColor(new Color(229, 229, 229, 184), .25f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
    }
    #endregion

}
