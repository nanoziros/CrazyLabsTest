using System;
using LightItUp.Singletons;

namespace LightItUp
{
    public class PowerUpService : SingletonCreate<PowerUpService>
    {
        [Serializable]
        public enum BoosterType
        {
        	Missile = 0
        }

        public void TriggerPowerUp(BoosterType type)
        {
            switch (type)
            {
                case BoosterType.Missile:
                    MissilesController.Instance.LaunchMissiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}