using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Assets._scripts.UI
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _position;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _background;
        [SerializeField] private RawImage targetImage;
        [SerializeField] private Color _firstPlaceColor;
        [SerializeField] private Color _secondPlaceColor;
        [SerializeField] private Color _thirdPlaceColor;
        [SerializeField] private Color _playerColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Sprite _playerImage;
        [SerializeField] private Sprite _firstPlaceImage;
        [SerializeField] private Sprite _secondPlaceImage;
        [SerializeField] private Sprite _thirdPlaceImage;
        [SerializeField] private Sprite _defaultImage;

        IEnumerator DownloadImage(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки изображения: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetImage.texture = texture;
            }
        }
        public void SetStatistic(int position, string name, int score, string url, bool isPlayer)
        {
            _position.text = position.ToString();
            _name.text = name;
            _score.text = score.ToString();
            switch (position)
            {
                case 1:
                    _background.color = _firstPlaceColor;
                    _background.sprite = _firstPlaceImage;
                    break;
                case 2:
                    _background.color = _secondPlaceColor;
                    _background.sprite = _secondPlaceImage;
                    break;
                case 3:
                    _background.color = _thirdPlaceColor;
                    _background.sprite = _thirdPlaceImage;
                    break;
                default:
                    _background.color = _defaultColor;
                    _background.sprite = _defaultImage;
                    break;
            }
            if (isPlayer)
            {
                _background.color = _playerColor;
                _background.sprite = _playerImage;
            }
            _name.text = name;
            _score.text = score.ToString();
            StartCoroutine(DownloadImage(url));
        }
    }
}
