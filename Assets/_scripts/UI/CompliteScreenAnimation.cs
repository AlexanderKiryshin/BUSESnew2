using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class CompliteScreenAnimation : MonoBehaviour
{
    [SerializeField] private Image _starImage1;
    [SerializeField] private Image _starImage2;
    [SerializeField] private Image _starImage3;
    [SerializeField] private float _pulsateSize = 1.2f;
    [SerializeField] private float _appearDuration = 1.2f;
    [SerializeField] private float _pulsateSpeed = 1.2f;
    [SerializeField] private float _delayBetweenStar = 1.2f;
    [SerializeField] private float _delayPulsate = 0.25f;
    [SerializeField] private GameObject _particleEffect;
    [SerializeField] private float _effectDuration = 3.0f;


    private void Awake()
    {
        Timing.RunCoroutine(_StarAnimation().CancelWith(gameObject));
        Timing.RunCoroutine(ResetSalut().CancelWith(gameObject));
    }

    private IEnumerator<float> _StarAnimation()
    {
        yield return Timing.WaitForSeconds(_delayBetweenStar);
        _starImage1.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(_delayPulsate);
        Timing.RunCoroutine(_PulsateStar(_starImage1).CancelWith(gameObject));

        yield return Timing.WaitForSeconds(_delayBetweenStar);
        _starImage2.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(_delayPulsate);
        Timing.RunCoroutine(_PulsateStar(_starImage2).CancelWith(gameObject));

        yield return Timing.WaitForSeconds(_delayBetweenStar);
        _starImage3.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(_delayPulsate);
        Timing.RunCoroutine(_PulsateStar(_starImage3).CancelWith(gameObject));
    }

    private IEnumerator<float> _PulsateStar(Image starImage)
    {
        float duration = _appearDuration;
        float elapsed = 0f;
        Vector3 originalScale = starImage.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        while (elapsed < duration)
        {
            starImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Timing.DeltaTime;
            yield return Timing.WaitForOneFrame;
        }

        starImage.transform.localScale = targetScale;


        while (true)
        {
            elapsed = 0f;
            while (elapsed < _pulsateSpeed)
            {
                starImage.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / _pulsateSpeed);
                elapsed += Timing.DeltaTime;
                yield return Timing.WaitForOneFrame;
            }

            elapsed = 0f;
            while (elapsed < _pulsateSpeed)
            {
                starImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / _pulsateSpeed);
                elapsed += Timing.DeltaTime;
                yield return Timing.WaitForOneFrame;
            }
        }
    }

    private IEnumerator<float> ResetSalut()
    {
        while (true)
        {
            _particleEffect.SetActive(false);
            _particleEffect.SetActive(true);
            float elapsed = 0f;
            while (elapsed < _effectDuration)
            {
                elapsed += Timing.DeltaTime;
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}