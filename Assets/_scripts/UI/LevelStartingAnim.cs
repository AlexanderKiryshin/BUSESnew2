using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartingAnim : MonoBehaviour
{
    public RectTransform uiObject;
    public Image[] colorChangingObjects;
    public Image[] colorChangingObjects2;
    public Color[] colors; 
    public float moveDuration = 2f;
    public float colorChangeInterval = 1f;
    public float moveBackDuration = 2f;
    public float changingColorTime = 2f;

    private Vector2 originalPosition;
    private Vector2 offScreenPosition;

    void Start()
    {
        uiObject.gameObject.SetActive(true);
        originalPosition = uiObject.anchoredPosition;
        offScreenPosition = new Vector2(Screen.width, uiObject.anchoredPosition.y);
        uiObject.anchoredPosition = offScreenPosition;

        StartCoroutine(MoveUIObjectToOriginalPosition());
    }

    private IEnumerator MoveUIObjectToOriginalPosition()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = offScreenPosition;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            uiObject.anchoredPosition = Vector2.Lerp(startPosition, originalPosition, elapsedTime / moveDuration);
            yield return null;
        }

        uiObject.anchoredPosition = originalPosition;
        StartCoroutine(ChangeColors());
        StartCoroutine(ChangeColors2());
        yield return new WaitForSeconds(changingColorTime);
        StartCoroutine(MoveUIObjectBackOffScreen());
    }

    private IEnumerator ChangeColors()
    {
        int colorIndex = 0;

        while (true)
        {
            foreach (var obj in colorChangingObjects)
            {
                obj.color = colors[colorIndex];
                colorIndex = (colorIndex + 1) % colors.Length;
                yield return new WaitForSeconds(colorChangeInterval);
            }
        }
    }private IEnumerator ChangeColors2()
    {
        int colorIndex = 0;

        while (true)
        {
            foreach (var obj in colorChangingObjects2)
            {
                obj.color = colors[colorIndex];
                colorIndex = (colorIndex + 1) % colors.Length;
                yield return new WaitForSeconds(colorChangeInterval);
            }
        }
    }

    private IEnumerator MoveUIObjectBackOffScreen()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = originalPosition;

        while (elapsedTime < moveBackDuration)
        {
            elapsedTime += Time.deltaTime;
            uiObject.anchoredPosition = Vector2.Lerp(startPosition, offScreenPosition, elapsedTime / moveBackDuration);
            yield return null;
        }

        uiObject.anchoredPosition = offScreenPosition;
        uiObject.gameObject.SetActive(false);
    }
    
}
