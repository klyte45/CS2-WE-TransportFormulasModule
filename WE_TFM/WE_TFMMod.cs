using Belzont.Interfaces;
using Belzont.Utils;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace WE_TFM
{
    public class WE_TFMMod : BasicIMod, IMod
    {
        public override void OnDispose()
        {
        }

        public override BasicModData CreateSettingsFile()
        {
            return new WE_TFMData(this);
        }

        public override void DoOnCreateWorld(UpdateSystem updateSystem)
        {
        }

        public override void DoOnLoad()
        {
            LogUtils.DoInfoLog(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                LogUtils.DoInfoLog($"Current mod asset at {asset.path}");
        }
    }
}
