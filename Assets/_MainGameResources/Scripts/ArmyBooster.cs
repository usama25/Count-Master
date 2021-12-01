using UnityEngine;
using TMPro;

public class ArmyBooster : MonoBehaviour
{
    public bool multiplier;

    public int factor;

    [SerializeField] private GameObject adjacentGate;

    [SerializeField] private TextMeshPro text;
    
    [ContextMenu("Start")]
    public void Start()
    {
        text.text = (multiplier ? "x" : "+") + factor.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Soldier"))
            return;
        
        gameObject.SetActive(false);
        
        if(adjacentGate)
            adjacentGate.GetComponent<Collider>().enabled = false;
        
        if (multiplier)
        {
            int currentCount = PlayerController.Instance.spawner.army.Count;
            int increasefactor =
                currentCount * factor;
            increasefactor -= currentCount;
            PlayerController.Instance.AddSoldiers(increasefactor);
        }
        else
        {
            PlayerController.Instance.AddSoldiers(factor);
        }
        HapticManager.instance.Haptic_Medium();
    }
}
