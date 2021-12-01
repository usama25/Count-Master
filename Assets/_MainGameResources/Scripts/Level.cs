using UnityEngine;
using NaughtyAttributes;
public class Level : MonoBehaviour
{
    public bool hasStairs = true, hasArchers;
    public Transform stairsPosition, pathParent;
    private void Start()
    {
        GameObject stairs = GameObject.Find("StairsPrefab");
        stairsPosition = FindObjectOfType<FinishLine>().transform.GetChild(1); 
        GameObject castle = FindObjectOfType<RandomCastle>().gameObject;
        if (hasStairs)
        {
            stairs.transform.position = stairsPosition.position;
            if(castle)
              castle.SetActive( false );
        }
        else
        {
            if(castle) 
                castle.transform.position = stairsPosition.position;
            stairs.gameObject.SetActive(false);
        }

        if (hasArchers)
        {
            ParticlesController.Instance.GenerateParticlesPool(2);
        }
    }

    [ContextMenu("FindPathParent")]
    void FindPathParent()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Path"))
                pathParent = transform.GetChild(i);
        }
    }
    
    [Button()]
    void AssignGateNumbers()
    {
        ArmyBooster[] armyBoosters = GetComponentsInChildren<ArmyBooster>();
        for (int i = 0; i < armyBoosters.Length; i++)
        {
            armyBoosters[i].Start();
        }
    }
}
