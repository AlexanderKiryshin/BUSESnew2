using UnityEngine;

public class Pulsate : MonoBehaviour
{

    public float amplitude = 0.1f;  
    public float frequency = 3f;    

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localScale = initialScale * scale;
    }
}
