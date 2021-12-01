using TMPro;
using UnityEngine;

public class TextCulling : MonoBehaviour
{
    public Vector2 cullDistance = new Vector2(50,150);
    
    private Color color;

    private TextMeshPro textMesh;

    private Transform cam;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        color = textMesh.color;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        color = textMesh.color;
        color.a = Mathf.Lerp(1, 0,
            Mathf.InverseLerp(cullDistance.x, cullDistance.y, Vector3.Distance(transform.position, cam.position)));
        textMesh.color = color;
    }
}
