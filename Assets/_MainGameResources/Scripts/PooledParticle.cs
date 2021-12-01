using UnityEngine;
using System.Collections;

public class PooledParticle : MonoBehaviour
{
    public int customParticleIndex;

    public float deactiveAfter = -1;
    public bool autoReturn, particleBehavior;
    
    private void Start()
    {
        if(particleBehavior)
            deactiveAfter = GetComponent<ParticleSystem>().main.duration;
    }

    public void Init(int cpi, bool ar, bool pb)
    {
        customParticleIndex = cpi;
        autoReturn = ar;
        particleBehavior = pb;
    }
    
    void ReturnToPool()
    {
        ParticlesController.Instance.ReturnToPool(customParticleIndex, gameObject);
    }

    private void OnEnable()
    {
        if(particleBehavior)
            GetComponent<ParticleSystem>().Play(true);
        
        if(autoReturn)
           StartCoroutine(deactive());
    }

    private void OnDisable()
    {
        ReturnToPool();
    }

    IEnumerator deactive()
    {
        if (deactiveAfter < 0)
            Start();

        yield return new WaitForSeconds(deactiveAfter);
        gameObject.SetActive(false);
    }
}