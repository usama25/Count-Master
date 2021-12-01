using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class HapticManager : MonoBehaviour
{
    public static HapticManager instance;
    
    void Awake()
    {
        instance = this;
    }

    #region haptics

    public void Haptic_Medium()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }
    public void Hatpic_Rigid()
    {
        MMVibrationManager.Haptic(HapticTypes.RigidImpact);
    }
    public void Haptic_Heavy()
    {
        MMVibrationManager.Haptic(HapticTypes.Failure);
        // print("heavy");
    }
    public void Haptic_Success()
    {
        MMVibrationManager.Haptic(HapticTypes.Success);
    }
    public void Haptic_Light()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    public void Haptic_Fail()
    {
        MMVibrationManager.Haptic(HapticTypes.Failure);
    }


    #endregion
}