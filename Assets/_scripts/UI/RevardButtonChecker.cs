using _scripts.UI;
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
    public ButtonType _buttonType;
    public int _countOfUsage;

    private Button _button;
    private RewardedShop _rewardedShop;

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

    public void Initialize(int freeCount, RewardedShop shop)
    {
        _rewardedShop = shop;
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

    public void OnClick()
    {
        // if (!IsRewardAwailable) return;
        // IsRewardAwailable = !IsRewardAwailable;
        if (_countOfUsage == 0) return;
        _countOfUsage--;
        if (_rewardedShop != null)
            _rewardedShop.OnBosterClicked(_buttonType);
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
            _adImage.enabled = false;
            _freeText.enabled = true;
            if (_coutOfFreeUsage == null) return;
            _plusImage.enabled = false;
            _coutOfFreeUsage.enabled = true;
            _coutOfFreeUsage.text = _countOfUsage.ToString();
        }
        else
        {
            _adImage.enabled = true;
            _freeText.enabled = false;
            if (_coutOfFreeUsage == null) return;
            _plusImage.enabled = true;
            _coutOfFreeUsage.enabled = false;
        }
    }

    private void LoadData()
    {
        switch (_buttonType)
        {
            // case ButtonType.Arrange:
            //     if (MirraSDK.Data.HasKey("FirstArrange"))
            //         IsRewardAwailable = MirraSDK.Data.GetBool("FirstArrange");
            //     break;
            case ButtonType.Continue:
                if (MirraSDK.Data.HasKey("FirstContinue"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstContinue");
                else _countOfUsage = 1;
                break;
            // case ButtonType.Jumble:
            //     if (MirraSDK.Data.HasKey("FirstJumble"))
            //         IsRewardAwailable = MirraSDK.Data.GetBool("FirstJumble");
            //     break;
            case ButtonType.Turbo:
                if (MirraSDK.Data.HasKey("FirstTurbo"))
                    _countOfUsage = MirraSDK.Data.GetInt("FirstTurbo");
                else _countOfUsage = 1;
                break;
            // case ButtonType.Vip:
            //     if (MirraSDK.Data.HasKey("FirstVip"))
            //         IsRewardAwailable = MirraSDK.Data.GetBool("FirstVip");
            //     break;
            case ButtonType.CarParking:
                if (MirraSDK.Data.HasKey("CarParking"))
                    _countOfUsage = MirraSDK.Data.GetInt("CarParking");
                else _countOfUsage = 1;
                break;
        }
    }
}