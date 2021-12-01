using UnityEngine;

public class Stair : MonoBehaviour
{
    private bool done;

    private void OnTriggerEnter(Collider other)
    {
        if(done)
            return;
        
        if (other.CompareTag("Soldier"))
        {
            done = true;
            CameraController.Instance.StepUp();
            HapticManager.instance.Haptic_Medium();
         }
    }
}
