using MirraGames.SDK;
using MirraGames.SDK.Common;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._scripts
{
    public class LocalizationManager:MonoBehaviour
    {
        private Dictionary<LanguageType, Dictionary<string, string>> _localizations;
        private LanguageType _currentLanguage;
        public Action onLanguageChange;
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalizationManager>();
                }
                return _instance;
            }
        }
        private void Awake()
        {
            InitializeLocalizations();
        }

        private void Start()
        {
            StartCoroutine(SetLanguageRoutine());
        }

        private IEnumerator SetLanguageRoutine()
        {
            yield return new WaitUntil(() => MirraSDK.IsInitialized);
            _currentLanguage = LanguageType.English;

            switch (_currentLanguage)
            {
                case LanguageType.Russian:
                case LanguageType.English:
                case LanguageType.Turkish:
                case LanguageType.German:
                    break;
                default: _currentLanguage = LanguageType.English; break;
            }
            SetLanguage(_currentLanguage);
        }
        private void InitializeLocalizations()
        {
            _localizations = new Dictionary<LanguageType, Dictionary<string, string>>
            {
                [LanguageType.English] = new Dictionary<string, string>
                {
                    ["level"] = "Level",
                    ["continue"] = "Continue?",
                    ["restart"] = "Restart",
                    ["complete"] = "Complete",
                    ["failed"] = "Failed!",
                    ["arrange"] = "Arrange",
                    ["jumble"] = "Jumble",
                    ["parking_spot"] = "parking spot",
                    ["ads_not_available"] = "Ads not available",
                    ["coins"] = "Coins: {0}",
                    ["buy_for"] = "Buy for {0}",
                    ["in_queue"] = "In Queue",
                    ["unlock_parking_slot"] = "Unlock parking space to resolve traffic!",
                    ["arrange_info"] = "Fill the Passengers based onvehicle colors.",
                    ["jumble_info"] = "Reshuffle the vehicle colors in the parking slot",
                    ["parking_spot_info"] = "Unlock 1 parking space",
                    ["free"] = "Free",
                    ["first_phrase"] = "Caught in a traffic jam!",
                    ["receive"] = "receive",
                    ["time"] = "time x2",
                    ["turbo"] = "turbo",
                    ["turbo_info"] = "Turbocharge your game\r\nspeed (X2)",
                    ["helicopter"] = "Select a car to the vip slot",
                    ["passangers_are_coming"] = "Passengers are coming!",
                    ["left"] = "left",
                    ["vip_info"] = "Move ANY CAR to\r\nVIP Space",
                    ["vip"] = "VIP SLOT",
                    ["slot_not_empty"] = "The VIP slot is occupied",
                    ["no_buses"] = "There are no buses in the parking lot",
                    ["no_money"] = "Not enough coins",
                    ["shop"] = "Shop",
                    ["beginners_kit"] = "Beginner's Kit",
                    ["premium_set"] = "Premium set",
                    ["remove_ads_bundle"] = "Remove ads bundle",
                    ["replay"]="Replay",
                    ["reamoveAdDescription"]= "Disables ads during gameplay",
                    ["ads"]= "COFFEE BREAK IN ",
                    ["score"]="Score: ",
                    ["leaderboard"] = "Leaderboard",
                },
                [LanguageType.Russian] = new Dictionary<string, string>
                {
                    ["level"] = "Уровень",
                    ["continue"] = "Продолжить?",
                    ["restart"] = "Заново",
                    ["complete"] = "Пройден", 
                    ["failed"] = "Провал!",
                    ["arrange"] = "Упорядочить",
                    ["jumble"] = "Перемешать",
                    ["parking_spot"] = "Парковочное место",
                    ["ads_not_available"] = "Реклама недоступна",
                    ["coins"] = "Монеты: {0}",
                    ["buy_for"] = "Купить за {0}",
                    ["in_queue"] = "В очереди",
                    ["unlock_parking_slot"] = "Разблокируйте парковочное место, чтобы разрешить дорожное движение",
                    ["arrange_info"] = "Заполните пассажиров в соответствии с цветами транспортного средства",
                    ["jumble_info"] = "Измените цвет автомобиля на парковочном месте",
                    ["parking_spot_info"] = "Разблокируйте 1 парковочное место",
                    ["free"] = "Бесплатно",
                    ["first_phrase"] = "застрял в дорожной пробке!",
                    ["receive"] = "Получить",
                    ["time"] = "Время x2",
                    ["turbo"] = "Турбо",
                    ["turbo_info"] = "Увеличьте скорость своей игры (X2)",
                    ["helicopter"] = "Выберите автомобиль для вип-слота",
                    ["passangers_are_coming"] = "Пассажиры приближаются!",
                    ["left"] = "осталось",
                    ["vip_info"] = "Переместить ЛЮБОЙ АВТОМОБИЛЬ\r\n на ВИП место",
                    ["vip"] = "ВИП СЛОТ",
                    ["slot_not_empty"] = "ВИП слот занят",
                    ["no_buses"] = "нет автобусов на парковке",
                    ["no_money"] = "недостаточно монет",
                    ["shop"] = "Магазин",
                    ["beginners_kit"] = "Набор для начинающих",
                    ["premium_set"] = "Премиальный набор",
                    ["remove_ads_bundle"] = "Отключить рекламу",
                    ["replay"]="Переиграть",
                    ["reamoveAdDescription"]="Отключает рекламу во время игрового процесса",
                    ["ads"] = "КОФЕ БРЕЙК ЧЕРЕЗ ",
                    ["score"] = "Счет: ",
                    ["leaderboard"] = "Таблица лидеров",
                },
                [LanguageType.Turkish] = new Dictionary<string, string>
                {
                    ["level"] = "Seviye",
                    ["continue"] = "Devam?",
                    ["restart"] = "Yeniden Başlat", 
                    ["complete"] = "Tamamlandı",
                    ["failed"] = "Başarısız!",
                    ["arrange"] = "Düzenle",
                    ["jumble"] = "karmakarışık",
                    ["parking_spot"] = "Park Yeri",
                    ["ads_not_available"] = "Reklam mevcut değil",
                    ["coins"] = "Para: {0}",
                    ["buy_for"] = "Satın al {0}",
                    ["in_queue"] = "Sırada",
                    ["unlock_parking_slot"] = "Trafiği çözmek için park yerinin kilidini açın!",
                    ["arrange_info"] = "Yolcuları araç renklerine göre doldurun",
                    ["jumble_info"] = "Park yerindeki araç renklerini değiştirin",
                    ["parking_spot_info"] = "1 Park yerinin kilidini açın",
                    ["free"] = "ücretsiz",
                    ["first_phrase"] = "Trafik sıkışıklığına yakalandı!",
                    ["receive"] = "almak",
                    ["time"] = "zaman x2",
                    ["turbo"] = "hızlanma",
                    ["turbo_info"] = "Oyun hızınızı\r\nartırın (X2)",
                    ["helicopter"] = "Vıp yuvasına bir araba seçin",
                    ["passangers_are_coming"] = "Yolcular geliyor!",
                    ["left"] = "duruyordu",
                    ["vip_info"] = "HERHANGİ bir ARACI\r\nVİP Alanına Taşı",
                    ["vip"] = "VIP YUVASI",
                    ["slot_not_empty"] = "VIP yuvası dolu",
                    ["no_buses"] = "Otoparkta otobüs yok",
                    ["no_money"] = "yeterli para yok",
                    ["shop"] = "dükkan",
                    ["beginners_kit"] = "Başlangıç Seti",
                    ["premium_set"] = "Prim seti",
                    ["remove_ads_bundle"] = "Reklam paketini kaldır",
                    ["replay"]="Yeniden oyna",
                    ["reamoveAdDescription"]= "Oyun sırasında reklamları devre dışı bırakır",
                    ["ads"] = "Kahve molası ",
                    ["score"] = "Puan: ",
                    ["leaderboard"] = "Lider Tablosu",
                },
                [LanguageType.German] = new Dictionary<string, string>
                {
                    ["level"] = "Stufe",
                    ["continue"] = "Fortfahren?",
                    ["restart"] = "Neustart",
                    ["complete"] = "Abgeschlossen",
                    ["failed"] = "Gescheitert!",
                    ["arrange"] = "Anordnen",
                    ["jumble"] = "Verwirren",
                    ["parking_spot"] = "Parkplatz",
                    ["ads_not_available"] = "Werbung nicht verfügbar",
                    ["coins"] = "Münzen: {0}",
                    ["buy_for"] = "Kaufen für {0}",
                    ["in_queue"] = "In Warteschlange",
                    ["unlock_parking_slot"] = "Entsperr den Parkplatz, um das Verkehrsstau zu lösen!",
                    ["arrange_info"] = "Füllen Sie die Fahrzeuge nach Farbe der Passagiere ein",
                    ["jumble_info"] = "Ändern Sie die Farben der Fahrzeuge auf dem Parkplatz",
                    ["parking_spot_info"] = "1 Parkplatz entsperren",
                    ["free"] = "kostenlos",
                    ["first_phrase"] = "In einen Verkehrsstau geraten!",
                    ["receive"] = "erhalten",
                    ["time"] = "Zeit x2",
                    ["turbo"] = "Turbo",
                    ["turbo_info"] = "Erhöhen Sie die Spielgeschwindigkeit\r\n(X2)",
                    ["helicopter"] = "Wählen Sie ein Auto für das VIP-Zeuggplatz",
                    ["passangers_are_coming"] = "Passagiere kommen!",
                    ["left"] = "blieb",
                    ["vip_info"] = "JEDIGES AUTO\r\nINS VIP-BEREICH BRINGEN",
                    ["vip"] = "VIP-BEREICH",
                    ["slot_not_empty"] = "VIP-Zeuggplatz belegt",
                    ["no_buses"] = "Keine Busse auf dem Parkplatz",
                    ["no_money"] = "Nicht genug Münzen",
                    ["shop"] = "Geschäft",
                    ["beginners_kit"] = "Anfänger-Set",
                    ["premium_set"] = "Premium-Set",
                    ["remove_ads_bundle"] = "Werbungspaket entfernen",
                    ["replay"] = "Nochmal spielen",
                    ["reamoveAdDescription"] = "Schaltet Werbung während des Spiels aus",
                    ["ads"] = "Kaffeepause ",
                    ["score"] = "Punktestand: ",
                    ["leaderboard"] = "Rangliste",
                }
            };
        }


        public string GetText(string key)
        {
            if (_localizations.ContainsKey(_currentLanguage) &&
                _localizations[_currentLanguage].ContainsKey(key))
            {
                return _localizations[_currentLanguage][key];
            }
            return $"Missing_{key}";
        }

        public string GetText(string key, params object[] args)
        {
            string text = GetText(key);
            return string.Format(text, args);
        }

        public void SetLanguage(LanguageType language)
        {
            
            if (_localizations.ContainsKey(language))
            {
                _currentLanguage = language;
            }
            else
            {
                _currentLanguage = LanguageType.English;
            }
            onLanguageChange?.Invoke();
        }

        public LanguageType GetCurrentLanguage()
        {
            return _currentLanguage;
        }
    }
}
