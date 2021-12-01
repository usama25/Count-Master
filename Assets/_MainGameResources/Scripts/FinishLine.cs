using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public bool bigEnemy;

    public GameObject prefab;

    private void Start()
    {
        if (bigEnemy)
            transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            GetComponent<Collider>().enabled = false;
            if (bigEnemy)
            {
                PlayerController.Instance.SoldiersIdle();
                PlayerController.Instance.StopPlayer();
                UiManager.instance.spinPanel.SetActive(true);
            }
            else
            {
                CameraController.Instance.EnableFinalCam(true);
                GameManager.instance.FinishLineCrossed();
                PlayerController.Instance.spawner.TurnCountText(false);
                UiManager.instance.SetCoinsEarned();
            }
        }

        
        
    }

    public GameObject gatePrefab;
    [ContextMenu("placegate")]

    void placegate()
    {
        if (transform.childCount == 0)
        {
            GameObject obj = Instantiate(gatePrefab, transform);
            obj.transform.localPosition = gatePrefab.transform.localPosition;
            obj.transform.localEulerAngles = gatePrefab.transform.localEulerAngles;
            obj.transform.localScale = gatePrefab.transform.localScale;
        }
    }

    [ContextMenu("InitFromParent")]
    public void InitFromParent()
    {
        Transform parent = transform.parent;
        bigEnemy = parent.name.Contains("Enemy");
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.parent = parent.parent;
        transform.SetSiblingIndex(0);
        DestroyImmediate(parent.gameObject);
    }
}
