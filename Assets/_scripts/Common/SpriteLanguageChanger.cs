using System;
using Assets._scripts;
using MirraGames.SDK.Common;
using UnityEngine;
using UnityEngine.UI;

public class SpriteLanguageChanger : MonoBehaviour
{
    [SerializeField] private Sprite _engSprite;
    [SerializeField] private Sprite _rusSprite;
    [SerializeField] private Sprite _turkSprite;

    private Image _currentSprite;

    private void Start()
    {
        _currentSprite = GetComponent<Image>();
        LocalizationManager.Instance.onLanguageChange += UpdateSprite;
        UpdateSprite();
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance)
            LocalizationManager.Instance.onLanguageChange -= UpdateSprite;
    }

    private void UpdateSprite()
    {
        switch (LocalizationManager.Instance.GetCurrentLanguage())
        {
            case LanguageType.English:
                _currentSprite.sprite = _engSprite;
                break;
            case LanguageType.Russian:
                _currentSprite.sprite = _rusSprite;
                break;
            case LanguageType.Turkish:
                _currentSprite.sprite = _turkSprite;
                break;
            default:
                _currentSprite.sprite = _engSprite;
                break;
        }
    }
}