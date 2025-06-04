using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets._scripts
{
    public class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField] private string localizationKey;
        
        private TextMeshPro _tmpText;

        private void Start()
        {
            _tmpText = GetComponent<TextMeshPro>();          
            UpdateText();
            LocalizationManager.Instance.onLanguageChange += UpdateText;
        }

        private void UpdateText()
        {
            string localizedText = LocalizationManager.Instance.GetText(localizationKey);
            _tmpText.text = localizedText;
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }

        private void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.onLanguageChange -= UpdateText;
            }
        }
    }
} 