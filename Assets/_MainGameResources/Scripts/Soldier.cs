using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Soldier : MonoBehaviour
{
    #region vars

    [SerializeField] private bool isEnemy;
    public  bool isArcher, alreadySpawn;
    bool targetedPlayer;

    public float jumpHeight = 1, jumpTime = 1;

    private PlayerController _playerController;

    public Animator _animator;

    public bool inJumpState;

    public SkinnedMeshRenderer skin;

    private Collider _collider;
    #endregion

    #region unity-methods

    private void OnEnable()
    {
        if (!isEnemy)
            if (!alreadySpawn)
                _animator.SetTrigger("Run");
    }

    [ContextMenu("FindSkin")]
    void FindSkin()
    {
        if (!skin)
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _playerController = PlayerController.Instance;
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jump"))
        {
            if (inJumpState)
                return;

            inJumpState = true;

            // _playerController.spawner.RestrictRearrangingFor(1);
            transform.DOLocalMoveY(jumpHeight, jumpTime).SetRelative(true).SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.Linear).onComplete = () => inJumpState = false;
        }
        else if (other.CompareTag("Soldier"))
        {
            if (isEnemy)
            {
                if (!targetedPlayer)
                {
                    targetedPlayer = true;
                    GetComponentInParent<EnemiesGroup>().TargetPlayerSoldiers();
                }
                _playerController.SoldierDied(gameObject, other.gameObject, isArcher);
                _playerController._movementController.SetTowardsTargetSpeed(false );
            }
        }
        else if (other.CompareTag("Stair"))
        {
            transform.SetParent(null);
            PlayerController.Instance.spawner.CheckStairsState();
        }
    }

    private void OnDisable()
    {
        if (!isArcher)
        {
            try
            {
                if(UiManager.instance.ui_MainMenu.gameObject.activeSelf)
                    return;
                if(_playerController.CurrentCharacter != 0)
                    return;
            }
            catch
            {
                return;
            }
            
            
            _playerController.spawner.AddSoldierToBackup(this);
            alreadySpawn = false;
            ResetForPlayer();
        }
    }

    #endregion

    #region others
    void ResetForPlayer()
    {
        isEnemy = false;
        skin.material = _playerController.playerMat;
        transform.rotation = Quaternion.identity;
        _animator.Play("Idle", 0, 0);
        gameObject.layer = 12;
        _collider.enabled = true;
        hitted = false;
    }

    public void GotWeaponHit()
    {
        transform.DOMoveY(Random.Range(4, 8), .5f).SetRelative(true).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        transform.DOMoveX(Random.Range(4, 15), 1f).SetRelative(true).SetEase(Ease.Linear);
        _animator.SetTrigger("Hit");
    }
    #endregion

    #region hits response
    private bool hitted;

    public void GotHurdled(HurdleType hurdleType)
    {
        if (hitted)
            return;

        hitted = true;

        _collider.enabled = false;
        
        switch (hurdleType)
        {
            case HurdleType.Simple:
                gameObject.SetActive(false);
                break;
            case HurdleType.Fall:
                transform.DOMoveY(-15, 5);
                _animator.SetTrigger("Die");
                Invoke("soldierDieWithDelay", 1);

                break;
            case HurdleType.Weapon:
                GotWeaponHit();
                Invoke("soldierDieWithDelay", 1);
                break;
        }
    }


    void soldierDieWithDelay( )
    {
        gameObject.SetActive(false);
    }
    #endregion

}
