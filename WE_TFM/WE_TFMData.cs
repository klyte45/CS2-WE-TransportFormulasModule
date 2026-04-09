using Belzont.Interfaces;
using Game.Modding;
using Game.Settings;
using Unity.Entities;

namespace WE_TFM
{
    public class WE_TFMData(IMod mod) : BasicModData(mod)
    {
        private readonly WE_TFMMod mod = mod as WE_TFMMod;

        public override void OnSetDefaults()
        {
        }

        [SettingsUIButton]
        public bool ResetConnections
        {
            set
            {
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<WE_TFM_PlatformMappingSystem>().MarkToResetWaypointsDestinations();
            }
        }
    }
}
