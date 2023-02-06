using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Model
{
    public static class SavesManager
    {
        public static readonly string[] BaseSkinCosts = new[] {"0", "100", "200", "200", "300", "300", "500", "1500", "3000", "5000", "500", "1000", "2000", "5000", "10000", "100000", "50000", "50000", "7000", "100000", "50000"};
        
        public static readonly string PlatformName = "PC";

        public static UnityEvent OnCoinValueChanged = new();

        public static float Record;

        private static int _coins = 0;
        public static int Coins
        {
            get => _coins;
            set
            {
                _coins = value;
                OnCoinValueChanged.Invoke();
            }
        }

        public static Sprite Skin;
        public static string[] SkinCosts;

        public static int SlowTimeUpgradeLevel = 1;
        public static int BounceUpgradeLevel = 1;
        public static int RadiusUpgradeLevel = 1;
        public static int StartTimeUpgradeLevel = 1;

        public static float GetSlowTimeDuration() => SlowTimeUpgradeLevel + 5 - 1;
        public static float GetBounceDuration() => BounceUpgradeLevel + 5 - 1;
        public static float GetRadiusValue() => RadiusUpgradeLevel * 0.5f + 2 - 0.5f;
        public static float GetStartTimeValue() => StartTimeUpgradeLevel * 10 - 10;

        public static int GetUpgradeCost(int level)
        {
            return GetFibonacciNumber(level + 3) * 100;
        }
                                         
        public static int GetFibonacciNumber(int len)  
        {  
            int a = 0, b = 1, c = 0;
            for (int i = 2; i < len; i++)  
            {  
                c= a + b;
                a= b;  
                b= c;  
            }

            return c;
        } 

        /*public void SaveStats()
        {
            switch (PlatformName)
            {
                case "PC":
                    break;
                case "Yandex":
                    break;
            }
        }*/
    }
}