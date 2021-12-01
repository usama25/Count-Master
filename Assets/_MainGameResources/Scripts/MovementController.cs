using UnityEngine;
using DG.Tweening;

public class MovementController : MonoBehaviour
{
    #region vars

    

    [SerializeField] bool moveForward, horizontalControl, goTowardsTarget;
    
    [SerializeField] SwerveInputs input;

    [SerializeField] float sideSpeed = 5, forwardSpeed = 0.25f, towardsTargetSpeed = 0.1f;
    [SerializeField] private Vector2 towardsTargetSpeeds;
    float swerveVal = 5;
    [SerializeField] Vector2 sideRange;

    [SerializeField] Transform target;
    #endregion

    #region unity

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 0.3f, transform.localPosition.z);
        
        if (horizontalControl)
            HandleSideMovement();

        if (moveForward)
            HandleForwardMovement();

        if (goTowardsTarget)
            HandleTowardsTargetMovement();
    }
    #endregion
    #region others

    void HandleSideMovement()
    {
        swerveVal = sideSpeed * Time.deltaTime * input.moveX;
        transform.Translate(swerveVal, 0, 0);
        Vector3 pos = transform.position;
        pos = new Vector3(Mathf.Clamp(pos.x, sideRange.x, sideRange.y), pos.y, pos.z);
        transform.position = pos;
    }

    void HandleForwardMovement()
    {
        transform.Translate(Vector3.forward * forwardSpeed);
    }
    
    void HandleTowardsTargetMovement()
    {
        transform.position = Vector3.MoveTowards( transform.position, target.position, towardsTargetSpeed);
    }

    public void MoveTowardsTarget(Transform t)
    {
        moveForward = false;
        target = t;
        horizontalControl = false;
        goTowardsTarget = true;
    }

    public void MoveTowardsTarget(bool state)
    {
        goTowardsTarget = state;
    }
    
    public void NormalControls()
    {
        moveForward = true;
        horizontalControl = true;
        goTowardsTarget = false;
    }

    public void StopEverything()
    {
        moveForward = false;
        horizontalControl = false;
        goTowardsTarget = false;
    }
    
    public void HorizontalControl(bool state)
    {
        horizontalControl = state;
    }
    
    public void PlaceInCenter()
    {
        transform.DOLocalMoveX(0, 1).SetEase(Ease.Linear);
    }

    public void SetTowardsTargetSpeed(bool max)
    {
        towardsTargetSpeed = !max ? towardsTargetSpeeds.x : towardsTargetSpeeds.y;
    }
    #endregion

}
