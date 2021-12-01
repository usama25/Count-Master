using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    private PlayerController _playerController;

    private Transform target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (target)
        {
            transform.Translate(target.position - transform.position);
        }
    }

    IEnumerator FindTarget()
    {
        yield return new WaitForSeconds(1);
        target = _playerController.spawner.GetRandomSoldier();
    }
}
