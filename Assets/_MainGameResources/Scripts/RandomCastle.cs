using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCastle : MonoBehaviour
{
    [SerializeField] private GameObject[] _objects;
    // Start is called before the first frame update
    void Start()
    {
        _objects[Random.Range(0, _objects.Length)].SetActive(true);
    }
}
