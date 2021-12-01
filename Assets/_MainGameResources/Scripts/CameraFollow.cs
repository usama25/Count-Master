using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	#region var
	[SerializeField] Vector3 offset;
    [SerializeField] Transform player;
    #endregion

    #region unity 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag( "Player" ).transform;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

	private void LateUpdate()
	{
        transform.position = player.position + offset;
	}
	#endregion
}
