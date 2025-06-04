using TMPro;
using UnityEngine;

namespace _scripts.UI
{
    public class Fps: MonoBehaviour
    {
        [SerializeField] private TMP_Text _fps;
        //[SerializeField] private TMP_Text _deltaTime;

        [SerializeField] private int _frameCount;

        private float deltaTime = 0;

        private void Start()
        {
            //Application.targetFrameRate = _frameCount;
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            int fps = Mathf.RoundToInt(1.0f / deltaTime);
            _fps.text = $"FPS: {fps}";
        
        }
    }
}