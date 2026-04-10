using Belzont.Interfaces;
using Belzont.Utils;
using BridgeWE;
using Colossal.Core;
using Game;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WE_TFM.BridgeWE;
using WE_TFM.WEBridge;

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

            MainThreadDispatcher.RegisterUpdater(() => { DoPatches(); });
        }
        private bool DoPatches()
        {
            try
            {
                if (AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "BelzontWE") is Assembly weAssembly)
                {
                    var exportedTypes = weAssembly.ExportedTypes;
                    foreach (var (type, sourceClassName) in new List<(Type, string)>() {
                    (typeof(WEFontManagementBridge), "FontManagementBridge"),
                    (typeof(WEImageManagementBridge), "ImageManagementBridge"),
                    (typeof(WETemplatesManagementBridge), "TemplatesManagementBridge"),
                    (typeof(WERouteFn), "WERouteFn"),
                    (typeof(WELocalizationBridge), "LocalizationBridge"),
                    (typeof(WEModuleOptionsBridge), "ModuleOptionsBridge")
                })
                    {
                        var targetType = exportedTypes.First(x => x.Name == sourceClassName);
                        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            var srcMethod = targetType.GetMethod(method.Name, RedirectorUtils.allFlags, null, method.GetParameters().Select(x => x.ParameterType).ToArray(), null);
                            if (srcMethod != null)
                            {
                                Harmony.ReversePatch(srcMethod, new HarmonyMethod(method));
                            }
                            else
                            {
                                LogUtils.DoWarnLog($"Method not found while patching WE: {targetType.FullName} {srcMethod.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))})");
                            }
                        }
                    }
                }
                else
                {
                    LogUtils.DoWarnLog("Write Everywhere dll file required for using this mod! Check if it's enabled.");
                    return false;
                }
            }
            catch
            {
                LogUtils.DoWarnLog("Write Everywhere dll file required for using this mod! Check if it's enabled.");
                return false;
            }
            return true;
        }
    }
}
