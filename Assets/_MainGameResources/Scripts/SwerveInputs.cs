using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwerveInputs : MonoBehaviour
{
    #region Var
    private float lastFingerPositionX;
    private float moveDeltaX;
    public float moveX => moveDeltaX;
    #endregion

    #region Unity 

    // Start is called before the first frame update
    void Start()
    {
        Input.multiTouchEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SwerveInput();
    }

    #endregion

    #region PublicMethods

    public void SwerveInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            lastFingerPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            moveDeltaX = Input.mousePosition.x - lastFingerPositionX;
            lastFingerPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveDeltaX = 0;
        }
    }

    #endregion
}
