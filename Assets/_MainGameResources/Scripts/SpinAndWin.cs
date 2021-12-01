using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SpinAndWin : MonoBehaviour
{
    private Tween arrowTween;
    public Transform arrowTrans;
    public float spinAnimTime = .75f;

    public UnityEvent OnSelectedEvenets;
    
    // Start is called before the first frame update
    void Start()
    {
        arrowTween = arrowTrans.DOLocalRotate(new Vector3(0, 0, 110), spinAnimTime).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo).SetRelative(true);
    }
    
    [ContextMenu("PrintLocalZ")]
    public void StopSpinning()
    {
        arrowTween.Pause();
        float zVal = arrowTrans.localEulerAngles.z;
        int n = 0;

        if (zVal < 203)
            n = 40;
        else if (zVal < 225)
            n = 80;
        else if (zVal < 247)
            n = 40;
        else
            n = 10;
        
        AssignReward(n);
        gameObject.SetActive(false);
        if (OnSelectedEvenets != null)
            OnSelectedEvenets.Invoke();
    }

    void PrintLocalZ()
    {
        print(arrowTrans.localEulerAngles.z);
    }

    void AssignReward(int n)
    {
        PlayerController.Instance.spawner.AddMore(n);
    }
}
