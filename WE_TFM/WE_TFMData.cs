using Belzont.Interfaces;
using Game.Modding;

namespace WE_TFM
{
    public class WE_TFMData(IMod mod) : BasicModData(mod)
    {
        private readonly WE_TFMMod mod = mod as WE_TFMMod;

        public override void OnSetDefaults()
        {
        }
    }
}
