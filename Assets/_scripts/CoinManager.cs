using System;
using MirraGames.SDK;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private BusRemover _busRemover;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private int _coinsOnComplete = 50;
    [SerializeField] private LevelLoader _levelLoader;
    private int _coins;

    private int _coinsInPreviosLevel;
    private int  _oneCoinCost;
    private bool _isNeedCoinAnimation = false;
    public event Action<int> OnCoinsChanged;
    public static CoinManager instance;

    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        LoadDataOrInit();
        
    }

    private void Start()
    {

        _uiManager.OnCompleteBtnClicked += AddCOinsOnComplete;
        _uiManager.OnDoubleRewardBtnAdClicked += AddCoinsOnDoubleReward;
        CoinSpawner.Instance.CoinReachedTarget += AddCoins;
        SetCost();
        if (_coins != 0 && _coinsInPreviosLevel != _coins && !_isNeedCoinAnimation)
        {
            CoinSpawner.Instance.SpawnCoins();
            int level = MirraSDK.Data.GetInt("Level");
            var isCoinAnimation = MirraSDK.Data.GetBool("CoinLevelAnimation");
            isCoinAnimation = true;
            MirraSDK.Data.SetBool("CoinLevelAnimation", isCoinAnimation);
            MirraSDK.Data.Save();
            _coins = _coinsInPreviosLevel;
        }
        OnCoinsChanged?.Invoke(_coins);
    }

    private void OnDisable()
    {
        CoinSpawner.Instance.CoinReachedTarget -= AddCoins;
        _uiManager.OnCompleteBtnClicked -= AddCOinsOnComplete;
        _uiManager.OnDoubleRewardBtnAdClicked -= AddCoinsOnDoubleReward;
    }

    private void AddCoinsOnDoubleReward()
    {
        _coinsInPreviosLevel = _coins;
        _coins += _coinsOnComplete * 2;
        Analytic.GoldChanged(MirraSDK.Data.GetInt("Level"), _coins);
        SaveCoins();
        SoundManager.instance.PlayAddCoinsSound();
        OnCoinsChanged?.Invoke(_coins);
    }
  
    private void AddCoins()
    {
        _coins += _oneCoinCost;
        SoundManager.instance.PlayAddCoinsSound();
        OnCoinsChanged?.Invoke(_coins);
    }

    public void AddCoinsOnBuy(int amount)
    {
        _coins += amount;
        SoundManager.instance.PlayAddCoinsSound();
        OnCoinsChanged?.Invoke(_coins);
        MirraSDK.Data.SetInt("CoinsInPreviosLevel", _coinsInPreviosLevel);
        MirraSDK.Data.SetInt("Coins", _coins);
    }

    private void AddCOinsOnComplete()
    {
        _coinsInPreviosLevel = _coins;
        _coins += _coinsOnComplete;
        Analytic.GoldChanged(MirraSDK.Data.GetInt("Level"), _coins);
        SaveCoins();
        SoundManager.instance.PlayAddCoinsSound();
        OnCoinsChanged?.Invoke(_coins);
    }
    
    public bool TryToSpendCoins(int value)
    {
        if (value <= _coins)
        {
            _coins -= value;
            Analytic.GoldChanged(MirraSDK.Data.GetInt("Level"), _coins);
            SaveCoins();
            OnCoinsChanged?.Invoke(_coins);
            return true;
        }
        SoundManager.instance.PlayErrorSound();
        return false;
    }

    private void LoadDataOrInit()
    {
        if (MirraSDK.Data.HasKey("Coins"))
        {
            _coins = MirraSDK.Data.GetInt("Coins");
            _coinsInPreviosLevel = MirraSDK.Data.GetInt("CoinsInPreviosLevel");
            OnCoinsChanged?.Invoke(_coins);
        }
        else
        {
            _coins = 0;
            _coinsInPreviosLevel = 0;
            SaveCoins();
            OnCoinsChanged?.Invoke(_coins);
        }

        if (MirraSDK.Data.HasKey("CoinLevelAnimation"))
        {
            _isNeedCoinAnimation = MirraSDK.Data.GetBool("CoinLevelAnimation");
        }
        else
        {
            
            MirraSDK.Data.SetBool("CoinLevelAnimation", false);
            MirraSDK.Data.Save();
        }
            
    }
    
    private void SaveCoins()
    {
        MirraSDK.Data.SetInt("CoinsInPreviosLevel", _coinsInPreviosLevel);
        MirraSDK.Data.SetInt("Coins", _coins);
        MirraSDK.Data.Save();
    }

    private void SetCost()
    {
        _oneCoinCost = (_coins - _coinsInPreviosLevel) / CoinSpawner.Instance.coinCount;
    }
}