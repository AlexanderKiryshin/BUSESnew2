using Assets._scripts;
using MirraGames.SDK;
using MirraGames.SDK.Common;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts.UI
{
    public class RewardedShop : MonoBehaviour
    {
        [SerializeField] private int _BeginnerKitBonus = 5;
        [SerializeField] private int _BeginnerKitBonusGold = 4000;
        [SerializeField] private int _PremiumKitBonus = 2;
        [SerializeField] private int _PremiumKitBonusGold = 1000;
        [SerializeField] private int _gold_300 = 300;
        [SerializeField] private int _gold_700 = 700;
        [SerializeField] private int _gold_1200 = 1200;
        [SerializeField] private CoinManager _coinManager;
        [SerializeField] private RevardButtonChecker _vipChecker, _arrangeChecker, _changeColorChecker;
        [SerializeField] private RevardButtonChecker _turboChecker, _continueChecker, _carParkingChecker;

        [SerializeField] private TextMeshProUGUI _beginerKit_150PriceText,
            _premiumKit_75PriceText,
            _gold_300PriceText,
            _gold_700PriceText,
            _gold_1200PriceText,
            _removeAd_100PriceText;

        public static Action OnNoAdsBuy;

        [SerializeField] private Button _beginerKitButton_150,
            _premiumKitButton_75,
            _goldButton_300,
            _goldButton_700,
            _goldButton_1200,
            _removeAdButton_100;

        public bool IsAdActive = true;
        public int VipBooster;
        public int ArrangeBooster;
        public int ChangeColorBooster;

        //Теги заполняются на площадке
        private const string beginerKit_150 = "beginerKit_150";
        private const string premiumKit_75 = "premium_kit_75";
        private const string gold_300 = "gold_300";
        private const string gold_700 = "gold_700";
        private const string gold_1200 = "gold_1200";
        private const string removeAd = "removeAd";

        private Dictionary<string, int> _purchaseCounts = new Dictionary<string, int>();

        public void OpenShop()
        {
            AdManager.instance.OnWindowEnabled(true);
        }

        public void CloseShop()
        {
            AdManager.instance.OnWindowEnabled(false);
        }

        private void Awake()
        {
            LoadDataOrInit();
            _beginerKitButton_150.onClick.AddListener(BuyBeginerKit);
            _premiumKitButton_75.onClick.AddListener(BuyPremiumKit);
            _goldButton_300.onClick.AddListener(BuyGold10);
            _goldButton_700.onClick.AddListener(BuyGold25);
            _goldButton_1200.onClick.AddListener(BuyGold35);
            _removeAdButton_100.onClick.AddListener(BuyRemoveAd);
            if (MirraSDK.Data.HasKey("no_ads"))
            {
                MirraSDK.Ads.EnableNoAds();
            }
        }

        private void Start()
        {
            UpdateButtons();
            _turboChecker.Initialize();
            _continueChecker.Initialize();
            _carParkingChecker.Initialize();
            SetCurrencies();
            LocalizationManager.Instance.onLanguageChange += SetCurrencies;
            //LoadPurchases();
            //CheckAndApplyPurchases();
            FetcAllPurchases();
        }
        
        private void FetcAllPurchases()
        {
            MirraSDK.Payments.RestorePurchases((fetchData) =>
            {
                foreach (var key in new[] { beginerKit_150, premiumKit_75, gold_300, gold_700, gold_1200, removeAd })
                {
                    fetchData.RestoreProduct(
                            productTag: key,
                            onProductRestore: () =>
                        {
                            //OnPurchaseCompleted(key);
                            ApplyPurchase(key);
                        }
                     );
                }
            });
            /*MirraSDK.Payments.Fetch(onFetchSuccess: (fetchData) =>
            {
                foreach (var key in new[] { beginerKit_150, premiumKit_75, gold_300, gold_700, gold_1200, removeAd })
                {
                    fetchData.ConsumeProduct(
                        productTag: key,
                        onProductConsume: () =>
                        {
                            //OnPurchaseCompleted(key);
                            ApplyPurchase(key);
                        }
                    );
                }
            });*/
        }
        
       /* private void FetchPurchases(string key)
        {
            MirraSDK.Payments.Fetch(onFetchSuccess: (fetchData) =>
            {
                fetchData.ConsumeProduct(
                    productTag: key,
                    onProductConsume: () =>
                    {
                        //OnPurchaseCompleted(key);
                        ApplyPurchase(key);
                    }
                );
            });
        }*/

        private void SetCurrencies()
        {
            ProductData productData = MirraSDK.Payments.GetProductData(beginerKit_150);
            _beginerKit_150PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
            productData = MirraSDK.Payments.GetProductData(premiumKit_75);
            _premiumKit_75PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
            productData = MirraSDK.Payments.GetProductData(gold_300);
            _gold_300PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
            productData = MirraSDK.Payments.GetProductData(gold_700);
            _gold_700PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
            productData = MirraSDK.Payments.GetProductData(gold_1200);
            _gold_1200PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
            productData = MirraSDK.Payments.GetProductData(removeAd);
            _removeAd_100PriceText.text = productData.GetFullPriceFloat() + productData.Currency;
        }

        private void OnDestroy()
        {
            _beginerKitButton_150.onClick.RemoveListener(BuyBeginerKit);
            _premiumKitButton_75.onClick.RemoveListener(BuyPremiumKit);
            _goldButton_300.onClick.RemoveListener(BuyGold10);
            _goldButton_700.onClick.RemoveListener(BuyGold25);
            _goldButton_1200.onClick.RemoveListener(BuyGold35);
            _removeAdButton_100.onClick.RemoveListener(BuyRemoveAd);
            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.onLanguageChange -= SetCurrencies;
        }

        private void UpdateButtons()
        {
           // _vipChecker.Initialize(VipBooster, this);
          //  _arrangeChecker.Initialize(ArrangeBooster, this);
          //  _changeColorChecker.Initialize(ChangeColorBooster, this);
        }

        private void LoadDataOrInit()
        {
            IsAdActive = !MirraSDK.Payments.IsAlreadyPurchased(productTag: removeAd);

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

            MirraSDK.Data.Save();
        }

        private void BuyBeginerKit()
        {
            MirraSDK.Payments.Purchase(
                productTag: beginerKit_150,
                onSuccess: () => { ApplyPurchase(beginerKit_150); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        private void BuyPremiumKit()
        {
            MirraSDK.Payments.Purchase(
                productTag: premiumKit_75,
                onSuccess: () => { ApplyPurchase(premiumKit_75); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        private void BuyGold10()
        {
            MirraSDK.Payments.Purchase(
                productTag: gold_300,
                onSuccess: () => { ApplyPurchase(gold_300); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        private void BuyGold25()
        {
            MirraSDK.Payments.Purchase(
                productTag: gold_700,
                onSuccess: () => { ApplyPurchase(gold_700); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        private void BuyGold35()
        {
            MirraSDK.Payments.Purchase(
                productTag: gold_1200,
                onSuccess: () => { ApplyPurchase(gold_1200); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        private void BuyRemoveAd()
        {
            MirraSDK.Payments.Purchase(
                productTag: removeAd,
                onSuccess: () => { ApplyPurchase(removeAd); },
                onError: () => { Debug.LogWarning("Failed to purchase"); }
            );
        }

        public void OnBosterClicked(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Arrange:
                    ArrangeBooster--;
                    MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
                    MirraSDK.Data.Save();
                    break;

                case ButtonType.Vip:
                    VipBooster--;
                    MirraSDK.Data.SetInt("VipBooster", VipBooster);
                    MirraSDK.Data.Save();
                    break;

                case ButtonType.Jumble:
                    ChangeColorBooster--;
                    MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
                    MirraSDK.Data.Save();
                    break;
            }
        }

        private void CheckAndApplyPurchases()
        {
            foreach (var key in new[] { beginerKit_150, premiumKit_75, gold_300, gold_700, gold_1200, removeAd })
            {
                    ApplyPurchase(key);
                // if (IsPurchaseNotApplied(key))
                // {
                // }
            }
        }

        private void OnPurchaseCompleted(string purchaseKey)
        {
            if (purchaseKey == removeAd)
            {
                _purchaseCounts[purchaseKey] = 1;
                SavePurchase(purchaseKey);
            }
            else
            {
                if (_purchaseCounts.ContainsKey(purchaseKey))
                {
                    _purchaseCounts[purchaseKey]++;
                }
                else
                {
                    _purchaseCounts[purchaseKey] = 1;
                }

                SavePurchase(purchaseKey);
            }
        }

        private void LoadPurchases()
        {
            _purchaseCounts = new Dictionary<string, int>
            {
                { beginerKit_150, 0 },
                { premiumKit_75, 0 },
                { gold_300, 0 },
                { gold_700, 0 },
                { gold_1200, 0 }
            };
            if (MirraSDK.Data.HasKey(removeAd))
            {
                _purchaseCounts[removeAd] = MirraSDK.Data.GetBool(removeAd) ? 1 : 0;
            }

            foreach (var key in new[] { beginerKit_150, premiumKit_75, gold_300, gold_700, gold_1200 })
            {
                if (MirraSDK.Data.HasKey(key))
                    _purchaseCounts[key] = MirraSDK.Data.GetInt(key);
                else
                    _purchaseCounts[key] = 0;
            }

            MirraSDK.Data.Save();
        }

        private void SavePurchase(string purchaseKey)
        {
            if (purchaseKey == removeAd)
            {
                MirraSDK.Data.SetBool(removeAd, true);
            }
            else
            {
                MirraSDK.Data.SetInt(purchaseKey, _purchaseCounts[purchaseKey]);
            }

            MirraSDK.Data.Save();
        }

        private bool IsPurchaseNotApplied(string purchaseKey)
        {
            Debug.Log($"Checking purchase: {purchaseKey}");
            Debug.Log($"Data HasKey: {MirraSDK.Data.HasKey(purchaseKey)}");
            Debug.Log($"Purchase counts contains key: {_purchaseCounts.ContainsKey(purchaseKey)}");
            Debug.Log($"Purchase count: {(_purchaseCounts.ContainsKey(purchaseKey) ? _purchaseCounts[purchaseKey]: -1)}");
            
            return !MirraSDK.Data.HasKey(purchaseKey) && _purchaseCounts.ContainsKey(purchaseKey) &&
                   _purchaseCounts[purchaseKey] > 0;
        }

        private void ApplyPurchase(string purchaseKey)
        {
            ApplyPurchaseLogic(purchaseKey);
            //SavePurchase(purchaseKey);
        }

        private void ApplyPurchaseLogic(string purchaseKey)
        {
            switch (purchaseKey)
            {
                case beginerKit_150:
                    VipBooster += _BeginnerKitBonus;
                    ArrangeBooster += _BeginnerKitBonus;
                    ChangeColorBooster += _BeginnerKitBonus;
                    UpdateButtons();
                    _coinManager.AddCoinsOnBuy(_BeginnerKitBonusGold);
                    MirraSDK.Data.SetInt("VipBooster", VipBooster);
                    MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
                    MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
                    MirraSDK.Data.Save();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), beginerKit_150);
                    break;

                case premiumKit_75:
                    VipBooster += _PremiumKitBonus;
                    ArrangeBooster += _PremiumKitBonus;
                    ChangeColorBooster += _PremiumKitBonus;
                    UpdateButtons();
                    _coinManager.AddCoinsOnBuy(_PremiumKitBonusGold);
                    MirraSDK.Data.SetInt("VipBooster", VipBooster);
                    MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
                    MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
                    MirraSDK.Data.Save();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), premiumKit_75);
                    break;

                case gold_300:
                    _coinManager.AddCoinsOnBuy(_gold_300);
                    MirraSDK.Data.Save();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), gold_300);
                    break;

                case gold_700:
                    _coinManager.AddCoinsOnBuy(_gold_700);
                    MirraSDK.Data.Save();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), gold_700);
                    break;

                case gold_1200:
                    _coinManager.AddCoinsOnBuy(_gold_1200);
                    MirraSDK.Data.Save();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), gold_1200);
                    break;

                case removeAd:
                    IsAdActive = false;
                    MirraSDK.Data.SetBool("no_ads", true);
                    MirraSDK.Ads.EnableNoAds();
                    VipBooster += _BeginnerKitBonus;
                    ArrangeBooster += _BeginnerKitBonus;
                    ChangeColorBooster += _BeginnerKitBonus;
                    UpdateButtons();
                    _coinManager.AddCoinsOnBuy(_BeginnerKitBonusGold);
                    MirraSDK.Data.SetInt("VipBooster", VipBooster);
                    MirraSDK.Data.SetInt("ArrangeBooster", ArrangeBooster);
                    MirraSDK.Data.SetInt("ChangeColorBooster", ChangeColorBooster);
                    MirraSDK.Data.Save();
                    OnNoAdsBuy?.Invoke();
                    Analytic.BuyInApp(MirraSDK.Data.GetInt("Level"), removeAd);
                    break;

                default:
                    Debug.LogWarning($"Unknown purchase key: {purchaseKey}");
                    break;
            }
        }
    }
}