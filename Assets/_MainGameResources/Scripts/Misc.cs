using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Misc : MonoBehaviour
{
    public GameObject finishLinePrefab,jhga;

    public int zds;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [ContextMenu("SpawnNewFinishLines")]

    void SpawnNewFinishLines()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject trigger = transform.GetChild(i).GetComponentInChildren<TriggerEvents>().gameObject;
            if (trigger.name.Contains("Finish_line"))
            {
                GameObject obj = Instantiate(finishLinePrefab, trigger.transform);
                obj.GetComponent<FinishLine>().InitFromParent();
            }
            else
            {
                print("Not spawned for " + transform.GetChild(i).name);
            }
        }
    }

    [ContextMenu("StairsPositionSetup")]
    void StairsPositionSetup()
    {
        GameObject[] levels = new GameObject[transform.childCount];

        foreach (var level in levels)
        {
            level.SetActive(true);
            if (level.transform.childCount > 4)
            {
                
            }
            else
            {
                
                for (int i = 0; i < 3; i++)
                {
                        
                }
            }
            level.SetActive(false);
        }
    }
[Button()]
     void afadf()
    {
        for (int i = 0; i < zds; i++)
        {
            Instantiate(jhga);
        }
    }
}
