using UnityEngine;
using  Cinemachine;
using  DG.Tweening;
using  NaughtyAttributes;

public class CameraController : Singleton<CameraController>
{ 
    [SerializeField] CinemachineVirtualCamera cam;

    private CinemachineTransposer _transposer;
    
    private Vector3 followOffset;

    public float speed = 10;
    public float height = 10;
     
    private bool animate, obtainedOffset;

    public void EnableFinalCam(bool anim = false)
    {
        cam.enabled = true;
        if(anim)
            FinalCamAnim();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam.enabled = true;
        _transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        cam.enabled = false;
        followOffset = _transposer.m_FollowOffset;
    }

    private void Update()
    {
        if(animate)
            _transposer.m_FollowOffset = Vector3.Lerp(_transposer.m_FollowOffset, followOffset, Time.deltaTime * speed);
    }

    public void StepUp()
    {
        if (!obtainedOffset)
        {
            obtainedOffset = true;
        }
        
        animate = true;
        // DOTween.To(() => _transposer.m_FollowOffset, x => _transposer.m_FollowOffset = x,
        //     _transposer.m_FollowOffset + new Vector3(0, height, 0), 0.25f);
        followOffset += new Vector3(0, height, 0);
    }

    [Button()]
    void FinalCamAnim()
    {
        DOTween.To(() => cam.m_Lens.FieldOfView, x => cam.m_Lens.FieldOfView = x, 5, 1.5f).SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo).SetRelative(true);
        CinemachineTransposer transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        DOTween.To(() => transposer.m_FollowOffset.y, x => transposer.m_FollowOffset.y = x, 5, 1.5f).SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo).SetRelative(true);
    }
}
