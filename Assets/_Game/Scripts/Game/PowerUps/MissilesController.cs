using LightItUp.Singletons;

namespace LightItUp
{
    public class MissilesController : SingletonCreate<MissilesController>
    {
        public void LaunchMissile()
        {
            // todo: check if missile is on cooldown
        }
    }
}