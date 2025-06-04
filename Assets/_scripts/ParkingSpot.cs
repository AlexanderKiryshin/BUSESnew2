using System.Collections;
using _scripts.UI;
using UnityEngine;

public class ParkingSpot : MonoBehaviour, IRaycastTarget
{
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private GameObject _adIcon;
    [SerializeField] private ParkingManager _parkingManager;
    [SerializeField] private GameObject _adsNotAvailible;
    CanvasGroup _canvasGroup;
    public bool _isCanOppenedByAd = false;
    private bool _isBlocked;
    private RectTransform _rectTransform;

    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _canvasGroup = _adsNotAvailible.GetComponent<CanvasGroup>();
        _rectTransform = _adsNotAvailible.GetComponent<RectTransform>();
        CheckSpot();
    }

    public void CheckSpot()
    {
        if (!_isCanOppenedByAd)
        {
            _adIcon.SetActive(false);
            _collider.enabled = false;
        }
    }

    // private void OnMouseDown()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //     {
    //         return;
    //     }
    //     if (_isBlocked)
    //     {
    //         return;
    //     }
    //     if (!_isCanOppenedByAd)
    //         return;
    //     else
    //     {
    //         AdManager.instance.StartRewarded("parking_spot", OnRewarded, OnNonReady);
    //     }
    // }

    public void OnRewarded()
    {
        _isCanOppenedByAd = false;
        CheckSpot();
        _parkingManager.OpenSpotByClickSpot(this);
    }

    private void OnNonReady()
    {
        StartCoroutine(AlphaCoroutine());
    }

    private IEnumerator AlphaCoroutine()
    {
        _adsNotAvailible.SetActive(true);
        _isBlocked = true;
        float time = 1;
        while (time > 0)
        {
            _canvasGroup.alpha = time;
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x,
                Vector2.Lerp(new Vector2(-450, 300), new Vector2(-450, 50), time).y);
            time -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _adsNotAvailible.SetActive(false);
        _isBlocked = false;
    }

    public void Iterract()
    {
        if (_isBlocked)
        {
            return;
        }

        if (!_isCanOppenedByAd)
            return;
        else
        {
            _uiManager.SelectSpot(this);
            _uiManager.OpenCarParkingWindow();
        }
    }

}