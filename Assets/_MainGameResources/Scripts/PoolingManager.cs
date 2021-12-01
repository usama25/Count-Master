//Shady

using System;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

#pragma warning disable 0414

[System.Serializable]
public class ParticlePool
{
    [FoldoutGroup("$ParticleName")]
    [Tooltip("If true Pool will be hidden in Hierarchy.")]
    public bool HideInHierarchy = true;
    [FoldoutGroup("$ParticleName")]
    [Tooltip("From this name the Particle will be called.")]
    public string ParticleName = "Name";
    [FoldoutGroup("$ParticleName")]
    [Tooltip("Size of Pool to make.")]
    public int        PoolSize       = 10;
    [FoldoutGroup("$ParticleName")]
    [Required]
    [Tooltip("Actual Prefab that will be Instantiated to make a pool.")]
    public GameObject ParticlePrefab = null;
    [FoldoutGroup("$ParticleName")]
    [Tooltip("If true the actual Parent and Queue will be avialable to see in Inspector.")]
    [SerializeField] bool Debug      = false;
    [FoldoutGroup("$ParticleName")]
    [ShowIf("Debug", true)]
    [ReadOnly]
    [Tooltip("Parent of Pool will be made at runtime.")]
    public GameObject PoolParent = null;
    [FoldoutGroup("$ParticleName")]
    [ShowIf("Debug", true)]
    [ReadOnly]
    [Tooltip("Queue will be instantiated at runtime.")]
    public Queue<GameObject> ParticleQueue = new Queue<GameObject>();
}//class end

[HideMonoScript]
public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance {get; private set;}
    [Title("PARTICLE MANAGER", "SINGLETON", titleAlignment: TitleAlignments.Centered)]
    // [HideLabel]
    [SerializeField] List<ParticlePool> Pools = new List<ParticlePool>();

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }//if end
        else
        {
            DestroyImmediate(gameObject);
            return;
        }//else end
        Startt();
    }//Awake() end

    private void Startt()
    {
        for(int i=0 ; i<Pools.Count ; i++)
            MakePool(Pools[i].ParticleName, Pools[i]);
    }//Start() end

    private void MakePool(string ParentName, ParticlePool Pool)
    {
        if(!Pool.ParticlePrefab)
            return;
        Pool.PoolParent = new GameObject(Pool.ParticleName);
        Pool.PoolParent.transform.SetParent(transform);
        if(Pool.HideInHierarchy)
            Pool.PoolParent.gameObject.hideFlags = HideFlags.HideInHierarchy;
        for(int i=0 ; i<Pool.PoolSize ; i++)
        {
            GameObject Temp = Instantiate(Pool.ParticlePrefab, Pool.PoolParent.transform);
            Temp.SetActive(false);
            Pool.ParticleQueue.Enqueue(Temp);
        }//loop end
    }//MakePool() end

   
    public GameObject SpawnObject(string enemyName, Transform Pos) => EnemySpawn(Pools.Find(pool => pool.ParticleName.Equals(enemyName)), Pos);

    
    
    private GameObject EnemySpawn(ParticlePool Pool, Transform Pos)
    {
        if (Pool.ParticleQueue.Count <= 0)
        {
            return null;
        }

        GameObject PS = Pool.ParticleQueue.Dequeue();
        
       

        PS.transform.SetParent(Pos);
        PS.gameObject.SetActive(true); 
        Pool.ParticleQueue.Enqueue(PS);


        return PS; //if end
    }
    
    public void PlayParticle(string ParticleName, Vector3 Pos, Transform parent) => Play(Pools.Find(pool => pool.ParticleName.Equals(ParticleName)), Pos , parent);

    private void Play(ParticlePool Pool, Vector3 Pos, Transform parent)
    {
        if(Pool.ParticleQueue.Count > 0)
        {
            ParticleSystem PS = Pool.ParticleQueue.Dequeue().GetComponent<ParticleSystem>();
            PS.transform.SetParent(parent);
            PS.transform.position = Pos;
            PS.transform.rotation = PS.transform.localRotation;
            PS.gameObject.SetActive(true);
            StartCoroutine(PutBackInQueue(Pool, PS.gameObject, PS.main.duration));
            PS.Play();
        }//if end
    }//PlayCollectable() end
    public void PlayParticle(string ParticleName, Transform Parent) => Play(Pools.Find(pool => pool.ParticleName.Equals(ParticleName)), Parent);
    
    private void Play(ParticlePool Pool, Transform Parent)
    {
        if(Pool.ParticleQueue.Count > 0)
        {
            ParticleSystem PS = Pool.ParticleQueue.Dequeue().GetComponent<ParticleSystem>();
            PS.transform.SetParent(Parent, true);
            PS.gameObject.SetActive(true);
            StartCoroutine(PutBackInQueue(Pool, PS.gameObject, PS.main.duration));
            PS.Play();
        }//if end
    }//PlayCollectable() end
    
    
    public void PlayParticle(string ParticleName, Vector3 pos) => Play(Pools.Find(pool => pool.ParticleName.Equals(ParticleName)), pos);
    
    private void Play(ParticlePool Pool, Vector3 pos)
    {
        if(Pool.ParticleQueue.Count > 0)
        {
            ParticleSystem PS = Pool.ParticleQueue.Dequeue().GetComponent<ParticleSystem>();
            PS.transform.position = pos;
            PS.gameObject.SetActive(true);
            StartCoroutine(PutBackInQueue(Pool, PS.gameObject, PS.main.duration));
            PS.Play();
        }//if end
    }//PlayCollectable() end

    private IEnumerator PutBackInQueue(ParticlePool Pool, GameObject Particle, float Duration)
    {
        yield return new WaitForSeconds(Duration);
        Particle.SetActive(false);
        Pool.ParticleQueue.Enqueue(Particle);
    }//Coroutine() end

}//class end