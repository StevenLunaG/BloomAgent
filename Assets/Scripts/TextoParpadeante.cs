using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextoParpadeante : MonoBehaviour
{
    private TextMeshProUGUI texto;
    
    [Header("Settings")]
    public float minAlpha = 0.2f;
    public float maxAlpha = 1.0f;
    public float speed = 1.0f;

    private void Awake()
    {
        texto = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (texto != null)
        {
            float t = Mathf.PingPong(Time.time * speed, 1.0f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            
            Color color = texto.color;
            color.a = alpha;
            texto.color = color;
        }
    }
}
