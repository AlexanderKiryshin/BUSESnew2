using UnityEngine;
using UnityEngine.UI;

namespace _scripts.UI
{
    public class ButtonPlayClickSound: MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PlayClickSound);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(PlayClickSound);
        }

        private void PlayClickSound() => SoundManager.instance.PlayButtonSound();
    }
}