using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies
{
    public interface ISpawnable
    {
        public void Spawn(float startSpeed, Vector2 up);
    }
}
