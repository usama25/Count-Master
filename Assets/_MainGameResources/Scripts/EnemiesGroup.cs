using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class EnemiesGroup : MonoBehaviour
{
    #region vars

    public int total;
    [Foldout("spawning ints")]
    public int unitCount, deadCount, rearrangeAfterDeleteCount = 5, deleteCount = 0;

    public Soldier enemyPefab;
    
    [Foldout("speeds & times")]
    [SerializeField] float towardsTargetSpeed = 0.1f, rearrangeAnimTime = 1;
    
    [Foldout("bools")]
    public bool moveTowardsTarget, pullPlayer = true, archers;
    
    bool movingTowardsTarget;
    
    [Foldout("transforms")]
    [SerializeField] Transform target, ringAnim;

    [SerializeField] private TextMeshPro countText;

    [SerializeField] private List<Soldier> army = new List<Soldier>();
    
    private bool pulledPlayer;
    
    [Foldout("Archery")]
    public float shootAfter = 1;
    private ParticlesController _particlesController;
    private PlayerController playerController;
    #endregion

    #region unity-methods

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        
        SetCountText();

        if (archers)
        {
            ArchersIdle();
            _particlesController = ParticlesController.Instance;
        }

        playerController = PlayerController.Instance;
    }

    private void FixedUpdate()
    {
        if(movingTowardsTarget)
          HandleTowardsTargetMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            if (pullPlayer && !pulledPlayer)
            {
                playerController.PullPlayer(transform);
                pulledPlayer = true;
                SetPlayerTarget();
                playerController._movementController.SetTowardsTargetSpeed(true);
                // playerController.spawner.RestrictRearrangingFor(3000);

                if(archers)
                    StopCoroutine("waitAndShootArrows");
            }
            
            GetComponent<Collider>().enabled = false;

            if (moveTowardsTarget)
            {
                target = playerController.transform;
                movingTowardsTarget = true;
                Run();
            }
        }
    }
    #endregion

    #region spawning
    public void Spawn()
    {
        for (int i = 0; i < total; i++)
        {            
            Vector3 spawnPos = GetSpawnPosition(i);
            var enemy = Instantiate(enemyPefab, spawnPos, enemyPefab.transform.rotation, transform);
            army.Add(enemy);
            enemy.gameObject.SetActive(true);
            if (archers)
                enemy.isArcher = archers;
        }
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
        return spawnPos;
    }
    #endregion

    #region animations
    void Run()
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i]._animator.SetTrigger("Run");
        }
    }
    
    public void Dance()
    {
        if(archers)
            StopCoroutine("waitAndShootArrows");
        
        movingTowardsTarget = false;
        for (int i = 0; i < army.Count; i++)
        {
            army[i]._animator.SetTrigger("Dance");
        }
    }
    #endregion

    #region other-methods
    [ContextMenu("Find Ring")]
    void FindRing()
    {
        ringAnim = transform.GetChild(transform.childCount - 1).GetChild(0);
    }
    
    public void SetPlayerTarget()
    {
        playerController.SetCurrentEnemyGroup(this);
    }
    
    void HandleTowardsTargetMovement()
    {
        transform.position = Vector3.MoveTowards( transform.position, target.position, towardsTargetSpeed);
    }

    void SetCountText()
    {
        if(countText)
            countText.text = (total - deadCount).ToString();
    }
    #endregion

    #region deletion & rearrangin

    /// <summary>
    /// solider from enemy group died
    /// </summary>
    public void SoldierDied(GameObject soldier)
    {
        deadCount++;
        SetCountText();
        army.Remove(soldier.GetComponent<Soldier>());
        soldier.SetActive(false);
        Deleted();
        
        ParticlesController.Instance.SpawnParticle(1, soldier.transform);

        if (deadCount == total + 1)
        {
            movingTowardsTarget = false;
            enabled = false;
            if(countText)
                countText.transform.parent.gameObject.SetActive(false);
            // gameObject.SetActive(false);
            playerController.spawner.RearrangeAll();
            
            if (pullPlayer)
                playerController.NormalControls(.25f);
            
            playerController.SetCurrentEnemyGroup();
            ringAnim.gameObject.SetActive(true);
            playerController._movementController.SetTowardsTargetSpeed(true);
        }
    }

    public void RearrangeAll()
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i].transform.DOLocalMove(  transform.InverseTransformPoint(GetSpawnPosition(i)), rearrangeAnimTime);
        }
    }

    void Deleted()
    {
        deleteCount++;
        if (deleteCount == rearrangeAfterDeleteCount)
        {
            deleteCount = 0;
            if (army.Count > 30)
            {
                //RearrangeAll();
            }
            else
            {
                rearrangeAfterDeleteCount = 5;
                TargetPlayerSoldiers();
            }
        }
        // if (army.Count == 20)
        // {
        //     TargetPlayerSoldiers();
        //     playerController.spawner.RearrangeAll(false);
        // }
        //
        // if (army.Count == 10)
        //     TargetPlayerSoldiers();
        // if (army.Count == 5)
        //     TargetPlayerSoldiers();

    }

    public void TargetPlayerSoldiers()
    {
        for (int i = 0; i < playerController.spawner.army.Count; i++)
        {
            try
            {
                if(i == army.Count)
                    return;
                Vector3 t = playerController.spawner.army[i].transform.position;
                army[i].transform.DOMove(t,
                    Vector3.Distance(army[i].transform.position, t)/5).SetEase(Ease.Linear);
            }
            catch
            {
            }
        }
        
    }
    #endregion

    #region archery
    void ArchersIdle()
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i]._animator.SetTrigger("Archer");
        }
    }

    public void StartShootingArrows()
    {
        StartCoroutine( "waitAndShootArrows");
    }

    IEnumerator waitAndShootArrows()
    {
        ShootArrows();
        yield return new WaitForSeconds(shootAfter);
        StartCoroutine("waitAndShootArrows");
    }

    void ShootArrows()
    {
        for (int i = 0; i < army.Count; i++)
        {
            _particlesController.GetSpawnedParticle(2, army[i].transform);
        }
    }
    #endregion
}
