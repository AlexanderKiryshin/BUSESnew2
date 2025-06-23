using _scripts.UI;
using Assets._scripts.UI;
using MirraGames.SDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    Arrange,
    Continue,
    Jumble,
    Turbo,
    Vip,
    CarParking
}

[RequireComponent(typeof(Button))]
public class RevardButtonChecker : MonoBehaviour
{
    [SerializeField] private Image _adImage, _plusImage;
    [SerializeField] private TMP_Text _coutOfFreeUsage;
    [SerializeField] private TMP_Text _freeText;
    [SerializeField] private GameObject _popup;
    public ButtonType _buttonType;
    public int _countOfUsage;

    private Button _button;
    private ButtonController _buttonController;

    public int CountOfUsage
    {
        get => _countOfUsage;
        set
        {
            _countOfUsage = value;
            CheckButton();
        }
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        CheckButton();
        // _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        // _button.onClick.RemoveListener(OnClick);
    }

    // private void Start()
    // {
    //     if (_buttonType == ButtonType.Turbo || _buttonType == ButtonType.Continue ||
    //         _buttonType == ButtonType.CarParking)
    //     {
    //         LoadData();
    //         CheckButton();
    //     }
    // }

    public void Initialize(int freeCount,ButtonController controller)
    {
        _buttonController = controller;
        _countOfUsage = freeCount;
        CheckButton();
    }

    public void Initialize()
    {
        if (_buttonType == ButtonType.Turbo || _buttonType == ButtonType.Continue ||
            _buttonType == ButtonType.CarParking)
        {
            LoadData();
            CheckButton();
        }
    }
    public void OnButtonClick()
    {
        if (_buttonController != null)
            _buttonController.OnBosterClicked(_buttonType);
    }
    public void OnClick()
    {
        // if (!IsRewardAwailable) return;
        // IsRewardAwailable = !IsRewardAwailable;
        if (_countOfUsage == 0)
        {
            //_popup.SetActive(true);
            return;
        }
        _countOfUsage--;
       
        CheckButton();
        SaveData();
    }

    private void SaveData()
    {
        switch (_buttonType)
        {
            // case ButtonType.Arrange:
            //     MirraSDK.Data.SetBool("FirstArrange", IsRewardAwailable);
            //     MirraSDK.Data.Save();
            //     break;

            case ButtonType.Continue:
                MirraSDK.Data.SetInt("FirstContinue", _countOfUsage);
                MirraSDK.Data.Save();
                break;

            // case ButtonType.Jumble:
            //     MirraSDK.Data.SetBool("FirstJumble", IsRewardAwailable);
            //     MirraSDK.Data.Save();
            //     break;

            case ButtonType.Turbo:
                MirraSDK.Data.SetInt("FirstTurbo", _countOfUsage);
                MirraSDK.Data.Save();
                break;

            // case ButtonType.Vip:
            //     MirraSDK.Data.SetBool("FirstVip", IsRewardAwailable);
            //     MirraSDK.Data.Save();
            //     break;

            case ButtonType.CarParking:
                MirraSDK.Data.SetInt("CarParking", _countOfUsage);
                MirraSDK.Data.Save();
                break;
        }
    }

    private void CheckButton()
    {
        if (_countOfUsage > 0)
        {
            if (_adImage != null)
                _adImage.enabled = false;
            if ((_freeText != null))
                _freeText.enabled = true;
            if (_plusImage != null)
                _plusImage.enabled = false;
            if (_coutOfFreeUsage != null)
            {
                _coutOfFreeUsage.enabled = true;
                _coutOfFreeUsage.text = _countOfUsage.ToString();
            }
        }
        else
        {
            if (_adImage !=null)
            _adImage.enabled = true;
            if ((_freeText!=null))
            _freeText.enabled = false;
            if (_coutOfFreeUsage == null) return;
            if(_plusImage != null)
            _plusImage.enabled = true;
            if (_coutOfFreeUsage != null)
            _coutOfFreeUsage.enabled = false;
        }
    }

    private void LoadData()
    {
        switch (_buttonType)
        {
             case ButtonType.Arrange:
                 if (MirraSDK.Data.HasKey("FirstArrange"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstArrange");
                else _countOfUsage = 1;
                break;
            case ButtonType.Continue:
                if (MirraSDK.Data.HasKey("FirstContinue"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstContinue");
                else _countOfUsage = 1;
                break;
             case ButtonType.Jumble:
                 if (MirraSDK.Data.HasKey("FirstJumble"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstJumble");
                else _countOfUsage = 1;
                break;
            case ButtonType.Turbo:
                if (MirraSDK.Data.HasKey("FirstTurbo"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstTurbo");
                else _countOfUsage = 1;
                break;
             case ButtonType.Vip:
                 if (MirraSDK.Data.HasKey("FirstVip"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstVip");
                else _countOfUsage = 1;
                break;
            case ButtonType.CarParking:
                if (MirraSDK.Data.HasKey("CarParking"))
                    _countOfUsage = MirraSDK.Data.GetInt("CarParking");
                else _countOfUsage = 1;
                break;
        }
    }
}