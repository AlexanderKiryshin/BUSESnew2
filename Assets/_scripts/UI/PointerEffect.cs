using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerEffect: MonoBehaviour, IPointerDownHandler
{
    public Color color;
    public Sprite spriteToCreate;
    public float spriteSize = 100f;
    public float scaleDuration = 2f;
    public float fadeDuration = 2f;
    public LayerMask _layerMask;

    private RectTransform canvasRectTransform;
    
    void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            StartCoroutine(SpriteCeator(localPoint));
        }
    }

    private void CreateSpriteAtPosition(Vector2 localPoint, float size)
    {
        GameObject newSpriteObject = new GameObject("CreatedSprite");
        newSpriteObject.transform.SetParent(transform);
        newSpriteObject.layer = 9;
        Image newSpriteImage = newSpriteObject.AddComponent<Image>();
        newSpriteImage.sprite = spriteToCreate;
        newSpriteImage.color = color;
        

        RectTransform newSpriteRectTransform = newSpriteObject.GetComponent<RectTransform>();
        newSpriteRectTransform.anchoredPosition = localPoint;
        newSpriteRectTransform.sizeDelta = new Vector2(spriteSize* size, spriteSize* size);

        StartCoroutine(SpriteCoroutine(newSpriteImage, newSpriteRectTransform));
    }

    private IEnumerator SpriteCeator(Vector2 localPoint)
    {
        CreateSpriteAtPosition(localPoint,  0.5f);
        yield return new WaitForSeconds(0.15f);
        CreateSpriteAtPosition(localPoint, 1f);
    }
    
    private IEnumerator SpriteCoroutine(Image spriteImage, RectTransform spriteRectTransform)
    {
        float elapsedScale = 0f;
        float elapsedFade = 0f;

        Vector2 originalSize = spriteRectTransform.sizeDelta;
        Vector2 targetSize = originalSize * 2f;

        while (elapsedScale < scaleDuration || elapsedFade < fadeDuration)
        {
            if (elapsedScale < scaleDuration)
            {
                float tScale = elapsedScale / scaleDuration;
                spriteRectTransform.sizeDelta = Vector2.Lerp(originalSize, targetSize, tScale);
                elapsedScale += Time.deltaTime;
            }

            if (elapsedFade < fadeDuration)
            {
                float tFade = elapsedFade / fadeDuration;
                spriteImage.color = Color.Lerp(color, new Color(color.r, color.g, color.b, 0), tFade);
                elapsedFade += Time.deltaTime;
            }

            yield return null;
        }

        Destroy(spriteImage.gameObject);
    }
}
    



