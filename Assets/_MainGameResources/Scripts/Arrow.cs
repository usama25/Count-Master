using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;

    public float speed;

    private void OnEnable()
    {
        target = PlayerController.Instance.spawner.GetRandomSoldier();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
        // transform.Translate(target.position - transform.position);
        transform.LookAt(target);
    }
}
