using UnityEngine;

public class Hurdle : MonoBehaviour
{
    public HurdleType _hurdleType;
    public bool isArrow;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            PlayerController.Instance.spawner.SoldierDied(other.gameObject, _hurdleType);

            if(isArrow)
                gameObject.SetActive(false);
		}
    }
}
public enum HurdleType
{
    Simple,
    Fall,
    Weapon
}
