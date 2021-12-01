using UnityEngine;
using UnityEngine.UI;

public class ProgressionBar : MonoBehaviour
{
    public Transform finalPoint, player;
    public Slider bar;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        finalPoint = FindObjectOfType<FinishLine>().transform;
        bar.maxValue = Vector3.Distance(player.position, finalPoint.position);
        bar.value = 0;
        startPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        bar.value = Vector3.Distance(player.position, startPos);
    }
}
