using MirraGames.SDK;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._scripts.UI
{
    public class ButtonController : MonoBehaviour
    {
        public int VipBooster;
        public int ArrangeBooster;
        public int ChangeColorBooster;
        public int TimeBooster;

        [SerializeField] TextMeshProUGUI _vipBoosterText;
        [SerializeField] TextMeshProUGUI _arrangeBoosterText;
        [SerializeField] TextMeshProUGUI _changeColorBoosterText;
        [SerializeField] TextMeshProUGUI _timeBoosterText;
        [SerializeField] Image _vipBoosterImage;
        [SerializeField] Image _arrangeBoosterImage;
        [SerializeField] Image _changeColorBoosterImage;
        [SerializeField] Image _timeBoosterImage;
        [SerializeField] Sprite _plus;
        [SerializeField] Sprite _background;
        [SerializeField] RevardButtonChecker _vip;
        [SerializeField] RevardButtonChecker _arrange;
        [SerializeField] RevardButtonChecker _changeColor;
        [SerializeField] RevardButtonChecker _time;
        [SerializeField] UiManager _uiManager;
        [SerializeField] GameObject _vipPopup;
        [SerializeField] GameObject _arrangePopup;
        [SerializeField] GameObject _changeColorPopup;
        [SerializeField] GameObject _timePopup;
        public void Start()
        {
            UiManager.OnArrangeUsed+= OnArrange;
            UiManager.OnVipUsed += OnVip;
            UiManager.OnJungleUsed += OnJumble;
            UiManager.OnTurboUsed += OnTurbo;
            StartCoroutine(InitializeButtonsCoroutine());
        }

        private IEnumerator InitializeButtonsCoroutine()
        {
            yield return new WaitUntil(() => MirraSDK.IsInitialized);
            InitializeButtons();
           
        }

        private void InitializeButtons()
        {
            if (!MirraSDK.Data.HasKey("level"))
            {
                MirraSDK.Data.SetInt("level", 0);
            }
            if (MirraSDK.Data.HasKey("VipBooster"))
                VipBooster = MirraSDK.Data.GetInt("VipBooster");
            else
            {
                VipBooster = 1;
                MirraSDK.Data.SetInt("VipBooster", VipBooster);
            }

            if (MirraSDK.Data.HasKey("ArrangeBooster"))
                ArrangeBooster = MirraSDK.Data.GetInt("ArrangeBooster");
            else
            {
                ArrangeBooster = 1;
                MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
            }

            if (MirraSDK.Data.HasKey("ChangeColorBooster"))
                ChangeColorBooster = MirraSDK.Data.GetInt("ChangeColorBooster");
            else
            {
                ChangeColorBooster = 1;
                MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
            }

            if (MirraSDK.Data.HasKey("TimeBooster"))
                TimeBooster = MirraSDK.Data.GetInt("TimeBooster");
            else
            {
                TimeBooster = 1;
                MirraSDK.Data.SetInt("TimeBooster", TimeBooster);
            }
            MirraSDK.Data.Save();
            if (VipBooster > 0)
            {
                _vipBoosterText.text = VipBooster.ToString();
                _vipBoosterImage.sprite = _background;
                _vipBoosterText.gameObject.SetActive(true);
            }
            else
            {
                _vipBoosterImage.sprite = _plus;
                _vipBoosterText.gameObject.SetActive(false);
            }
            _vip.Initialize(VipBooster, this);
            if (ArrangeBooster > 0)
            {
                _arrangeBoosterText.text = ArrangeBooster.ToString();
                _arrangeBoosterImage.sprite = _background;
                _arrangeBoosterText.gameObject.SetActive(true);
            }
            else
            {
                _arrangeBoosterImage.sprite = _plus;
                _arrangeBoosterText.gameObject.SetActive(false);
            }
            _arrange.Initialize(ArrangeBooster, this);
            if (ChangeColorBooster > 0)
            {
                _changeColorBoosterText.text = ChangeColorBooster.ToString();
                _changeColorBoosterImage.sprite = _background;
                _changeColorBoosterText.gameObject.SetActive(true);
            }
            else
            {
                _changeColorBoosterImage.sprite = _plus;
                _changeColorBoosterText.gameObject.SetActive(false);
            }
            _changeColor.Initialize(ChangeColorBooster, this);
            if (TimeBooster > 0)
            {
                _timeBoosterText.gameObject.SetActive(true);
                _timeBoosterText.text = TimeBooster.ToString();
                _timeBoosterImage.sprite = _background;
            }
            else
            {
                _timeBoosterImage.sprite = _plus;
                _timeBoosterText.gameObject.SetActive(false);
            }
            _time.Initialize(TimeBooster, this);
        }
        public void OnArrange()
        {
            ArrangeBooster--;
            MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
            MirraSDK.Data.Save();
            InitializeButtons();
        }

        public void OnVip()
        {
            VipBooster--;
            MirraSDK.Data.SetInt("VipBooster", VipBooster);
            MirraSDK.Data.Save();
            InitializeButtons();
        }

        public void OnJumble()
        {
            ChangeColorBooster--;
            MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
            MirraSDK.Data.Save();
            InitializeButtons();
        }

        public void OnTurbo()
        {
            TimeBooster--;
            MirraSDK.Data.SetInt("TimeBooster", TimeBooster);
            MirraSDK.Data.Save();
            InitializeButtons();
        }
        public void OnBosterClicked(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Arrange:
                    if (ArrangeBooster > 0)
                    {
                        _uiManager.ArrangeRewardedWithCheck();
                    }      
                    else
                    {
                        _arrangePopup.SetActive(true);
                    }
                    break;

                case ButtonType.Vip:
                    if (VipBooster > 0)
                    {
                        _uiManager.VipRewardedWithCheck();
                    }
                    else
                    {
                        _vipPopup.SetActive(true);
                    }
                    break;

                case ButtonType.Jumble:
                    if (ChangeColorBooster > 0)
                    {
                        _uiManager.JumbleRewarded();
                    }
                    else
                    {
                        _changeColorPopup.SetActive(true);
                    }
                    break;
                case ButtonType.Turbo:
                    if (TimeBooster > 0)
                    {
                       _uiManager.TurboRewardedWithCheck();
                    }
                    else
                    {
                        _timePopup.SetActive(true);
                    }
                    break;
            }
          
        }
        private void OnDestroy()
        {
            UiManager.OnArrangeUsed -= OnArrange;
            UiManager.OnVipUsed -= OnVip;
            UiManager.OnJungleUsed -= OnJumble;
            UiManager.OnTurboUsed -= OnTurbo;
        }
    }
}
