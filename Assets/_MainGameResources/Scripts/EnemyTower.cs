using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class EnemyTower : MonoBehaviour
{
    #region vars

    [SerializeField] private TextMeshPro countText;

    int health;
    public int totalHealth;
    private bool pulledPlayer;
    public Transform towerTrans;
    public Vector2 yPosLimits;

    [SerializeField] List<Soldier> army;

    public float shootAfter = 1;

    private ParticlesController _particlesController;
    #endregion
    
    #region unity-methods

    // Start is called before the first frame update
    void Start()
    {
        health = totalHealth;
        if (!towerTrans)
            towerTrans = transform;
        SetCountText();
        _particlesController = ParticlesController.Instance;
        if(army.Count > 0)
            ArchersIdle();
        ArrowsForEachTurn(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            if (!pulledPlayer)
            {
                PlayerController.Instance.PullPlayer(transform);
                PlayerController.Instance.enemyTower = this;
                GetComponent<SphereCollider>().enabled = false;
                pulledPlayer = true;
                StartShootingArrows();
                return;
            }
            
            health--;
            if (health == 0)
            {
                gameObject.SetActive(false);
                PlayerController.Instance.NormalControls();
                return;
            }
            else
            {
                float yPos = Mathf.Lerp(yPosLimits.x, yPosLimits.y, Mathf.InverseLerp(totalHealth, 0, health));
                towerTrans.DOLocalMoveY(yPos, 0.5f);
            }

            SetCountText();
            PlayerController.Instance.spawner.SoldierDied(other.gameObject);
        }
    }

    private void OnDisable()
    {
        ArrowsForEachTurn(false);
        if(PlayerController.Instance)
            PlayerController.Instance.enemyTower = this;
    }
#endregion

    #region archery

    /// <summary>
    /// one arrow is shot on each 5th call,
    /// this function will get arrow on each call as there are less soldiers here
    /// </summary>
    /// <param name="state"></param>
    public void ArrowsForEachTurn(bool state)
    {
        _particlesController._customParticles[2].spawnAfter = state ? 0 : 5;
    }
    
    void SetCountText()
    {
        if(countText)
            countText.text = health.ToString();
    }
    
    
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
        yield return new WaitForSeconds(shootAfter);
        ShootArrows();
        if(enabled)
            StartCoroutine("waitAndShootArrows");
    }

    void ShootArrows()
    {
        for (int i = 0; i < army.Count; i++)
        {
            _particlesController.GetSpawnedParticle(2, army[i].transform);
        }
    }
    
    public void StopShootingAndDance()
    {
        StopCoroutine("waitAndShootArrows");
        for (int i = 0; i < army.Count; i++)
        {
            army[i]._animator.SetTrigger("Dance");
        }
    }
    #endregion
}
