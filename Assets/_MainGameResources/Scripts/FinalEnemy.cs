using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalEnemy : MonoBehaviour
{
    public int Current_BE_Costume
    {
        get { return PlayerPrefs.GetInt("Current_BE_Costume", 0); }
        set { PlayerPrefs.SetInt("Current_BE_Costume", value); }
    }

    #region vars

    public Slider healthSlider;
    public int totalHealth, health, stopPlayerOnHealthDown, currentHealthDown;

    private bool pulledPlayer;

    private Animator animator;

    public float towardsTargetSpeed, restrictPlayerTime = 5;

    private bool moveTowardsTarget;

    private Transform target;
    [SerializeField] Transform targetForPlayer;

    [SerializeField] private BigEnemyCostume[] bigEnemyCostumes;

    private PlayerController playerController;

    private Collider swordColldier;
    #endregion

    #region unity
    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerController.Instance;
        health = totalHealth;
        if (healthSlider)
        {
            healthSlider.value = totalHealth;
            healthSlider.maxValue = totalHealth;
        }
        UpdateSlider();
        animator = GetComponentInChildren<Animator>();
        target = playerController.transform;
        ApplyCostume();
    }

    private void FixedUpdate()
    {
        if (moveTowardsTarget)
            HandleTowardsTargetMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            if (!pulledPlayer)
            {
                playerController.PullPlayer(targetForPlayer);
                GetComponent<SphereCollider>().enabled = false;
                pulledPlayer = true;
                moveTowardsTarget = true;
                playerController.spawner.RestrictRearrangingFor(restrictPlayerTime);
                CameraController.Instance.EnableFinalCam();
                SetAnimatorState("Run");
                if (healthSlider)
                    healthSlider.gameObject.SetActive(true);
                return;
            }

            moveTowardsTarget = false;
            SetAnimatorState("Attack");
            //
            // if (currentHealthDown == stopPlayerOnHealthDown)
            // {
            //     currentHealthDown = 0;
            //     playerController._movementController.MoveTowardsTarget(false);
            // }
            // else
            // {
            //     currentHealthDown++;
            //     playerController.spawner.RestrictRearrangingFor(restrictPlayerTime);
            // }
            
            health--;
            if (health == 0)
            {
                SetAnimatorState("Die");
                GetComponent<CapsuleCollider>().enabled = false;
                playerController.SoldiersDance();
                UiManager.instance.SetCoinsEarned();
                GameManager.instance.LevelComplete();
                if(healthSlider)
                    healthSlider.gameObject.SetActive(false);
                
                Current_BE_Costume++;
                if (Current_BE_Costume == bigEnemyCostumes.Length)
                    Current_BE_Costume = 0;
                
                return;
            }

            UpdateSlider();
            playerController.spawner.SoldierDied(other.gameObject);
            
        }
    }
    #endregion

    #region others
    [ContextMenu("AttackAnim")]
    void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }
    
    void SetAnimatorState(string str)
    {
        animator.SetTrigger(str);
    }
    
    private void UpdateSlider()
    {
        if(healthSlider)
            healthSlider.value = health;
    }
    
    void HandleTowardsTargetMovement()
    {
        transform.position = Vector3.MoveTowards( transform.position, target.position, towardsTargetSpeed);
    }

    public void PlayerComeColser()
    {
        playerController.TurnTargetFollowing(true);
        StartCoroutine(RestrictPlayer());
    }

    IEnumerator RestrictPlayer()
    {
        yield return new WaitForSeconds(.5f);
        playerController.TurnTargetFollowing(false);

    }

    public void ApplyCostume()
    {
        if(bigEnemyCostumes[Current_BE_Costume].helmet)
            bigEnemyCostumes[Current_BE_Costume].helmet.SetActive(true);
        
        bigEnemyCostumes[Current_BE_Costume].weapon.SetActive(true);
        swordColldier = bigEnemyCostumes[Current_BE_Costume].weapon.GetComponent<Collider>();
        SwordCollider(0);
    }

    public void SwordCollider(int i)
    {
        swordColldier.enabled = i == 1;
    }
    #endregion

    
}
[System.Serializable]
public class BigEnemyCostume
{
    public GameObject weapon, helmet;
}
