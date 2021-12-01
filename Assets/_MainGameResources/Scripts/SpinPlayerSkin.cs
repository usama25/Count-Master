using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinPlayerSkin : MonoBehaviour
{
    public int PlayerSkinPref
    {
        get { return PlayerPrefs.GetInt("PlayerSkinPref", 0); }
        set { PlayerPrefs.SetInt("PlayerSkinPref", value); }
    }
    
    [SerializeField] Transform wheelTrans;
    [SerializeField] float speed = 10, curSpeed;
    [SerializeField] Vector2 spinTime;
    [SerializeField] private Material [] skinMats;
    [SerializeField] private int sections;
    [SerializeField] private CanvasGroup skinPanel;
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        playerController.playerMat = skinMats[PlayerSkinPref];
    }

    private void Update()
    {
        wheelTrans.eulerAngles += new Vector3(0, 0, Time.deltaTime * curSpeed);
    }

    void Spin()
    {
        Spin(true);
    }
    public void Spin(bool state)
    {
        DOTween.To(() => curSpeed, x => curSpeed = x,  state ? speed : 0, .5f).onComplete =
            () => ApplyMaterial(state);
        
        if (state)
            StartCoroutine(stopSpinWithDelay());
    }

    IEnumerator stopSpinWithDelay()
    {
        skinPanel.interactable = false;
        yield return new WaitForSeconds(Random.Range(spinTime.x, spinTime.y));
        Spin(false);
    }

    void ApplyMaterial(bool state)
    {
        if(state)
            return;
        
        skinPanel.interactable = true;
        float anglePerSection = 360 / sections;
        int index = (int)Mathf.Ceil(wheelTrans.eulerAngles.z / anglePerSection);
        PlayerSkinPref = index;
        playerController.playerMat = skinMats[PlayerSkinPref];
        PlayerController.Instance.ChangeAndApplyPlayerMaterial(playerController.playerMat);
    }

}
