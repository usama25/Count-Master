using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reload_Scene()
    {
        //UiManager.instance.loading.gameObject.SetActive(true);
        SceneManager.LoadScene(0);
    }
}
