using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private bool done;
    private void Awake()
    {
        if (!done)
            done = true;
        else
            return;
        
        DontDestroyOnLoad(this);
    }
}
