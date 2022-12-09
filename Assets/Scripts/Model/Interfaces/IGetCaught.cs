namespace Assets.Scripts.Model.Interfaces
{
    public interface IGetCaught
    {
        public bool CanGetCaught();
        public void GetCaught();
        public void ReleaseCaught();
        public void TakeDamageOnRelease();
    }
}
