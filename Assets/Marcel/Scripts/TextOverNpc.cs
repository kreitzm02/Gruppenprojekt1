using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextOverNpc : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main;
        if (cam != null)
        {
            text.transform.rotation = cam.transform.rotation;
            text.transform.position = target.position + offset;
        }
    }
}
