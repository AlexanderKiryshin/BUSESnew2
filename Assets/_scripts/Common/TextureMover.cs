using UnityEngine;

public class TextureMover : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.5f, 0.5f);

    private Material material;
    private Vector2 offset;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && meshRenderer.material != null)
        {
            material = meshRenderer.material;
        }
        else
        {
            Debug.LogError("MeshRenderer или материал не найдены на объекте.");
        }
    }

    void Update()
    {
        if (material != null)
        {
            offset += speed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }
    }
}
