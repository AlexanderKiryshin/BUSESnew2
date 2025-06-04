using Assets._scripts;
using MirraGames.SDK;
using UnityEngine;
using MirraGames.SDK.Common;
namespace _Scripts.Common
{
    public class MaterialLanguageChanger: MonoBehaviour
    {
        [SerializeField] private Material _engMaterial;
        [SerializeField] private Material _rusMaterial;
        [SerializeField] private Material _turkMaterial;
    
        private MeshRenderer _currentMaterial;
    
        private void Start()
        {
            _currentMaterial = GetComponent<MeshRenderer>();          
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
                    _currentMaterial.material = _engMaterial;
                    break;
                case LanguageType.Russian:
                    _currentMaterial.material = _rusMaterial;
                    break;
                case LanguageType.Turkish:
                    _currentMaterial.material = _turkMaterial;
                    break;
                default:
                    _currentMaterial.material = _engMaterial;
                    break;
            }
        }
    }
}