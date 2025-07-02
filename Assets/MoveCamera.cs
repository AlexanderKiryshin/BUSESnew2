using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour
{
    [Header("Границы движения (world space)")]
    public Rect bounds = new Rect(-10, -10, 20, 20);   // x,y = левый‑нижний угол

    public Vector2 zBounds;

    [Header("Чувствительность перетаскивания")]
    public float dragSensitivity = 1f;                 // чем больше, тем быстрее камера

    private Camera cam;
    private Vector3 lastMousePos;
    [SerializeField] int minZoom = 18;
    [SerializeField] int maxZoom = 32;
    [SerializeField] int zoomStep = 2;
    [SerializeField] Button zoomIn;
    [SerializeField] Button zoomOut;

    void Awake() => cam = GetComponent<Camera>();

    void Update()
    {
        // запоминаем точку, где начали тащить
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        // двигаем, пока ЛКМ зажата
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            // переводим пиксельный сдвиг в мировое пространство
            Vector3 worldDelta =
                cam.ScreenToWorldPoint(new Vector3(delta.x, delta.y, cam.nearClipPlane)) -
                cam.ScreenToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));

            // инвертируем, чтобы камера «следовала» за курсором
            transform.position -= worldDelta * dragSensitivity;

            // жёсткое ограничение в пределах bounds
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, bounds.xMin, bounds.xMax);
            pos.y = Mathf.Clamp(pos.y, bounds.yMin, bounds.yMax);
            pos.z = Mathf.Clamp(pos.z, zBounds.x, zBounds.y);
            transform.position = pos;
        }
    }

    public void ZoomIn()
    {
        zoomOut.interactable = true;
        if (cam.orthographicSize>minZoom)
        {
            cam.orthographicSize -= zoomStep;
            if (cam.orthographicSize < minZoom)
            {
                cam.orthographicSize = minZoom;
                zoomIn.interactable = false;
            }           
        }       
    }

    public void ZoomOut()
    {
        zoomIn.interactable = true;
        if (cam.orthographicSize < maxZoom)
        {
            cam.orthographicSize += zoomStep;
            if (cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize = maxZoom;
                zoomOut.interactable = false;
            }
        }
    }

#if UNITY_EDITOR
    // визуализация прямоугольника в режиме Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, new Vector3(bounds.width, bounds.height, 0f));
    }
#endif
}
