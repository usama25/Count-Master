using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using TMPro;

public class Spawning : MonoBehaviour
{
    public int StartUnits
    {
        get { return PlayerPrefs.GetInt("StartUnits", 0); }
        set { PlayerPrefs.SetInt("StartUnits", value); }
    }
    
    #region vars
    
    public int unitCount, total = 50, toAdd = 50, rearrangeAfterDeleteCount = 10, deadCount = 0, currentSoldiersCount;
    public Soldier enemyPefab;
    public Soldier [] enemyPefabs;
    public List<Soldier> army = new List<Soldier>();
    public List<Soldier> backUp = new List<Soldier>();
    public int deleteCount;
    public float animTime = 0.25f, rearrangeAnimTime = 1;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private Transform stairsContainer, spawnParent, spawnParent2;
    [SerializeField] private Vector2 horizontalRandomness, forwardRandomness;
    
    public bool canRearrange = true;

    private PlayerController playerController;
    #endregion

    #region init
    private void Start()
    {
        //Spawn();
        playerController = PlayerController.Instance;
        Init();
    }

    private bool initd;

    [ContextMenu("Init")]
    public void Init()
    {
        SelectSoldierPrefab();
        SetInitialArmy();
        ChangeSoldiersMaterial(playerController.playerMat);
        initd = true;
    }

    void SelectSoldierPrefab()
    {
        DeleteAll();
        enemyPefab = enemyPefabs[0];
        Soldier soldier = Instantiate(enemyPefab, transform);
        soldier.alreadySpawn = true;
        army.Add(soldier);
        soldier.gameObject.SetActive(true);
    }
    
    public void SetInitialArmy()
    {
        if (StartUnits > 0)
        {
            int toAdd = UpgardesManager.Instance._upgrades[0].upgradeValues[StartUnits] - army.Count;
            Spawn(true, toAdd,true);
        }
        SetCountText();
    }
    #endregion

    #region spawning-add-delete

    public void RestrictRearrangingFor(float sec)
    {
        if (canRearrange)
        {
            canRearrange = false;
            StartCoroutine(restrictRearranging(sec));
        }
    }

    IEnumerator restrictRearranging(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (!canRearrange)
        {
            canRearrange = true;
            RearrangeAll();
        }
    }

    private Tween spawnAnim;

    void Spawn(bool add = false, int addCount = 0, bool init = false)
    {
        Transform curSpawnParent = spawnParent;
        if (!init)
        {
            if (spawnAnim != null && spawnAnim.IsPlaying())
            {
                curSpawnParent = spawnParent2;
                spawnParent2.gameObject.SetActive(false);
                spawnParent2.localScale = Vector3.one;
            }
            else
            { 
                spawnParent.gameObject.SetActive(false);
                spawnParent.localScale = Vector3.one;
                // CheckSpawnAnimState();
            }
        }

        int start = 0;
        if (add)
        {
            start = army.Count - 1;
            total = army.Count + addCount - 1;
            // print(start + " " + total);
        }   
        
        for (int i = start; i < total; i++)
        {
            Vector3 spawnPos = transform.InverseTransformPoint(GetSpawnPosition(i));
            Soldier enemy;
            
            if (backUp.Count > 0)
            {
                enemy = GetSoldierFromBackup();
                enemy.transform.SetParent(curSpawnParent);
                // enemy.transform.position = enemyPefab.transform.position;
            }
            else
            {
                enemy = PoolingManager.Instance.SpawnObject("enemy",init ? transform : curSpawnParent).GetComponent<Soldier>();
                //Instantiate(enemyPefab,
                // enemyPefab.transform.position
                // , Quaternion.identity,
                //init ? transform : curSpawnParent);
            }

            enemy.transform.localPosition = spawnPos;
            if (init)
            {
                enemy.alreadySpawn = true;
            }
            enemy.gameObject.SetActive(true);
            if (init)
            {
                // enemy.transform.DOLocalMove(spawnPos, animTime);
                enemy.transform.localPosition = spawnPos;
                enemy.transform.DOScale(Vector3.zero, animTime).From();
            }

            army.Add(enemy);
        }

        if (!init)
        {
            if (curSpawnParent == spawnParent)
                SpawnAnimation();
            else
                SpawnAnimation2();
        }
    }

    [ContextMenu("AddMore")]
    public void AddMore(int n)
    {
        UiManager.instance.PlusAnimText(n);
        
        Spawn(true, n);
        
        SetCountText();
    }
    
    public Vector3 GetSpawnPosition(int index)
    {
        int circleNumber = index / unitCount;

        int n = 0;
        for (int j = 1; j < 100; j++)
        {
            n += unitCount * j;
            if (index < n)
            {
                circleNumber = j;
                break;
            }
        }
        
        int numInCircleNum = circleNumber * unitCount;
        int i = index % numInCircleNum;
        
        var radians = 2 * Mathf.PI / numInCircleNum * i;

        /* Get the vector direction */
        var vertrical = Mathf.Sin(radians);
        var horizontal = Mathf.Cos(radians);

        var spawnDir = new Vector3(horizontal, 0, vertrical);

        /* Get the spawn position */
        var spawnPos =
            transform.position + spawnDir * circleNumber; // Radius is just the distance away from the point
        spawnPos += new Vector3(Random.Range(horizontalRandomness.x, horizontalRandomness.y), 0,
            Random.Range(forwardRandomness.x, forwardRandomness.y));
        return spawnPos;
    }

    [ContextMenu("DeleteAll")]
    public void DeleteAll()
    {
        if (army.Count == 0)
            return;
        
        for (int i = 0; i < army.Count; i++)
        {
            Destroy(army[i].gameObject);
        }
        army.Clear();
    }

    public void Deleted(GameObject obj)
    {
        deleteCount++;
        if (army.Count > 20)
        
            if (deleteCount == rearrangeAfterDeleteCount)
            {
                deleteCount = 0;
                if (canRearrange)
                    RearrangeAll();
                if (army.Count < 20)
                    rearrangeAfterDeleteCount = 5;
                else if (army.Count < 5)
                    rearrangeAfterDeleteCount = 1;
            }
        
        // else
        // {
        //     RearrangeAll();
        // }

    }

    public void RearrangeAll(bool allowForFuture = true)
    {
        if (allowForFuture)
        {
            canRearrange = true;
        }

        for (int i = 0; i < army.Count; i++)
        {
            if(!army[i].inJumpState)
                army[i].transform.DOLocalMove(transform.InverseTransformPoint(GetSpawnPosition(i)), rearrangeAnimTime);
        }
    }

    public void SoldierDied(GameObject soldier, HurdleType hurdleType = HurdleType.Simple)
    {
        deadCount++;
        Soldier s = soldier.GetComponent<Soldier>();
        army.Remove(s);
        SetCountText();

        s.GotHurdled(hurdleType);
        
        Deleted(soldier);
        
        ParticlesController.Instance.SpawnParticle(0, soldier.transform);
        HapticManager.instance.Haptic_Light();
        
        if (army.Count == 0)
        {
            if(countText)
                countText.transform.parent.gameObject.SetActive(false);
            GameManager.instance.LevelFailed();
        }
    }
    #endregion

    #region spawn-anim
    void SpawnAnimation()
    {
        spawnParent.gameObject.SetActive(true);
        
        spawnParent.localScale = Vector3.zero;

        spawnAnim = spawnParent.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        spawnAnim.onComplete = SpawnAnimationComplete;
    }

    void SpawnAnimationComplete()
    {
        while (spawnParent.childCount > 0)
        {
            spawnParent.GetChild(0).SetParent(transform);
        }

    }
    
    void SpawnAnimation2()
    {
        spawnParent2.gameObject.SetActive(true);
        
        spawnParent2.localScale = Vector3.zero;

        spawnParent2.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack).onComplete = SpawnAnimationComplete2;
        
    }

    void SpawnAnimationComplete2()
    {
        while (spawnParent2.childCount > 0)
        {
            spawnParent2.GetChild(0).SetParent(transform);
        }
    }
    

    #endregion
    
    #region stairs-Formation

    [Header("stairs-Formation")] 
    public float width = 1;
    
    public float height = 2;
    [ContextMenu("stairsFormation")]
    public void StairsFormation()
    {
        StartCoroutine("stairsFormation");
    }

    IEnumerator stairsFormation()
    {
        bool keepRunning = true, firstOfIdentical = true;
        yield return null;
        int layerNo = 1;

        while (keepRunning)
        {
            yield return new WaitForSeconds(.05f);
            
            // float xPosStart = layerNo / 2;
             keepRunning = MakeLayer(layerNo, transform.position.x + (-.5f * ((float)layerNo - 1)), width, height);
            if (!firstOfIdentical)
            {
                layerNo++;
            }

            firstOfIdentical = !firstOfIdentical;
        }
    }
    
    bool MakeLayer(int noInLayer, float xPosStart, float width, float height)
    {
        if (army.Count >= noInLayer)
        {
            // second layer
            stairsContainer.position += new Vector3(0, height, 0);
            int i ;
            for (i = 0; i < noInLayer; i++)
            {
                // print("ii");
                Soldier obj = army[0];
                army.RemoveAt(0);
                obj.transform.SetParent(stairsContainer);
                Vector3 pos = obj.transform.position;
                obj.transform.position = new Vector3(xPosStart + (i * width), pos.y 
                    // * (noInLayer > 2 ? 1 : 0)
                    ,
                    transform.position.z);
            }

            // if (army.Count >= noInLayer)
            return true;
        }
        else
        {
            // int toSpawnCount = noInLayer - army.Count;

            while (true)
            {
                // Soldier obj = army[0];
                try
                {
                    army[0].gameObject.SetActive(false);
                    army.RemoveAt(0);
                }
                catch
                {
                }


                if (army.Count == 0)
                    break;
            }

            return false;
        }
    }
    #endregion

    #region other-methods
    
    public void ChangeSoldiersMaterial(Material mat)
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i].skin.material = mat;
        }
        enemyPefab.skin.material = playerController.playerMat;
    }
    public void AllSoldiersPlay(string str)
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i]._animator.SetTrigger(str);
        }
    }
    
    void SetCountText()
    {
        // currentSoldiersCount = total - deadCount;
        if(countText)
            countText.text = army.Count.ToString();
    }

    public void TurnCountText(bool state)
    {
        countText.transform.parent.gameObject.SetActive(state);
    }

    /// <summary>
    /// checks if all soldiers are done on stairs
    /// </summary>
    public void CheckStairsState()
    {
        if (stairsContainer.childCount == 0)
        {
            PlayerController.Instance.StopPlayer();
            AllSoldiersPlayIdle();
            GameManager.instance.LevelComplete();
        }
    }

    void AllSoldiersPlayIdle()
    {
        Soldier[] soldiers = FindObjectsOfType<Soldier>();
        foreach (var soldier in soldiers)
        {
            soldier._animator.SetTrigger("Idle");
        }
    }

    public Transform GetRandomSoldier()
    {
        Transform t = transform;
        try
        {
            t =  army[Random.Range(0, army.Count)].transform;
        }
        catch
        {
        }

        return t;
    }   
    #endregion

    #region back-up-soldiers

    public void AddSoldierToBackup(Soldier soldier)
    {
        backUp.Add(soldier);
    }

    public Soldier GetSoldierFromBackup()
    {
        Soldier soldier = backUp[0];
        backUp.Remove(soldier);
        return soldier;
    }
    #endregion
}
