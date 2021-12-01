using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public static ParticlesController Instance;

    public CustomParticle[] _customParticles;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _customParticles.Length; i++)
        {
            CustomParticle cp = _customParticles[i];
            if(cp.hasPool)
            {
                cp.poolSystem.pool = new List<GameObject>();
                for (int j = 0; j < cp.poolSystem.startSize; j++)
                {
                    GameObject particle = Instantiate(cp._particle, cp.particlesContainer);
                    cp.poolSystem.pool.Add(particle);
                    particle.SetActive(false);
                    
                    PooledParticle pooledParticle;

                    if (!particle.GetComponent<PooledParticle>())
                        pooledParticle = particle.AddComponent<PooledParticle>();
                    else
                        pooledParticle = particle.GetComponent<PooledParticle>();
                    
                    pooledParticle.Init(i, cp.autoReturn, cp.particleBehavior);

                }
            }
        }
    }

    #region Other-Methods
    public void SpawnParticle(int i, Transform t)
    {
        CustomParticle customParticle = _customParticles[i];

        if (customParticle.currentCount == 0 || customParticle.currentCount == customParticle.spawnAfter)
        {
            if (customParticle.currentCount != 0)
                customParticle.currentCount = 0;

            GameObject particle;

            if (customParticle.hasPool)
            {
                particle = GetFromPool(i);
                PositionParticle(t, particle, i);
            }
            else
                particle = Instantiate(customParticle._particle, t.position + customParticle.positionOffset, Quaternion.identity, customParticle.particlesContainer);

            particle.gameObject.SetActive(true);
        }
        if(customParticle.spawnAfter > 0)
            customParticle.currentCount++;
    }
    
    public GameObject GetSpawnedParticle(int i, Transform t)
    {
        GameObject particle = gameObject;

        CustomParticle customParticle = _customParticles[i];

        if (customParticle.currentCount == 0 || customParticle.currentCount == customParticle.spawnAfter)
        {
            if (customParticle.currentCount != 0)
                customParticle.currentCount = 0;


            if (customParticle.hasPool)
            {
                particle = GetFromPool(i);
                PositionParticle(t, particle, i);
            }
            else
                particle = Instantiate(customParticle._particle, t.position + customParticle.positionOffset, Quaternion.identity, customParticle.particlesContainer);

            particle.gameObject.SetActive(true);
        }
        if(customParticle.spawnAfter > 0)
            customParticle.currentCount++;
        return particle;
    }

    void PositionParticle(Transform t, GameObject particle, int i)
    {
        particle.transform.position = t.position + _customParticles[i].positionOffset;
    }

    #region Pooling

    /// <summary>
    /// if we require to spawn particles in specific occassions
    /// </summary>
    /// <param name="i">index of custom particle</param>
    public void GenerateParticlesPool(int i)
    {
        _customParticles[i].hasPool = true;

        _customParticles[i].poolSystem.pool = new List<GameObject>();
        for (int j = 0; j < _customParticles[i].poolSystem.startSize; j++)
        {
            GameObject particle = Instantiate(_customParticles[i]._particle, _customParticles[i].particlesContainer);
            _customParticles[i].poolSystem.pool.Add(particle);
            particle.SetActive(false);

            PooledParticle pooledParticle;

            if (!particle.GetComponent<PooledParticle>())
                pooledParticle = particle.AddComponent<PooledParticle>();
            else
                pooledParticle = particle.GetComponent<PooledParticle>();

            pooledParticle.customParticleIndex = i;

        }
    }

    public void ReturnToPool(int customParticleIndex, GameObject particle)
    {
        CustomParticle customParticle = _customParticles[customParticleIndex];
        customParticle.poolSystem.pool.Add(particle);
    }

    public GameObject GetFromPool(int customParticleIndex)
    {
    // print(customParticleIndex);
        // if(_customParticles[WeaponcustomParticleIndex]
        //     .poolSystem.pool[0] == null)
        // {
        //     Debug.LogError(_customParticles[customParticleIndex].name + " pool is empty.");
        //     return null;
        // }

        GameObject particle = _customParticles[customParticleIndex].poolSystem.pool[0];
        _customParticles[customParticleIndex].poolSystem.pool.RemoveAt(0);
        return particle;
    }
    #endregion

    #endregion

}
#region Custom-Classes
[System.Serializable]
public class CustomParticle
{
    public string name;
    public GameObject _particle;
    public int spawnAfter = 5;
    public int currentCount = 0;
    public Vector3 positionOffset;
    public Transform particlesContainer;
    public bool hasPool, particleBehavior = true, autoReturn = true;
    public ParticlesPool poolSystem;
}
[System.Serializable]
public class ParticlesPool
{
    public List<GameObject> pool;
    public int startSize;
}
#endregion