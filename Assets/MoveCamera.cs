using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour
{
    public Vector2 xBounds;
    public Vector2 yBounds;
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

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam.orthographicSize <= minZoom)
        {
            zoomIn.interactable = false;
        }
        if (cam.orthographicSize >= maxZoom)
        {
            zoomOut.interactable = false;
        }
    }


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
           // transform.position -= worldDelta * dragSensitivity;

            // жёсткое ограничение в пределах bounds
            Vector3 pos = transform.position - worldDelta * dragSensitivity;
            pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
            pos.y = Mathf.Clamp(pos.y,yBounds.x, yBounds.y);
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
            yBounds.y += zoomStep;
            if (cam.orthographicSize <= minZoom)
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
            yBounds.y -= zoomStep;
            if (cam.orthographicSize >= maxZoom)
            {
                cam.orthographicSize = maxZoom;
                zoomOut.interactable = false;
            }
        }
    }
}
