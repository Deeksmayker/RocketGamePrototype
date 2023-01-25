using UnityEngine.Events;

namespace Assets.Scripts.Model
{
    public static class SavesManager
    {
        public static UnityEvent OnCoinPickup = new();
        
        public static float Record;

        private static int _coins;
        public static int Coins
        {
            get => _coins;
            set
            {
                _coins = value;
                OnCoinPickup.Invoke();
            }
        }
    }
}