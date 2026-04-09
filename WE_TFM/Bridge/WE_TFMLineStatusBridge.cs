using Game.Prefabs;
using System;
using System.Collections.Generic;
using Unity.Entities;
using WE_TFM.Systems;

namespace WE_TFM.Bridge
{
    [Obsolete("Don't reference methods on this class directly. Always use reverse patch to access them, and don't use this mod DLL as hard dependency of your own mod.", true)]
    public static class WE_TFMLineStatusBridge
    {
        public unsafe static List<Entity> GetCityLines(TransportType tt, bool acceptCargo, bool acceptPassenger) => WE_TFM_LineStatusSystem.Instance.GetCityLines(tt, acceptCargo, acceptPassenger);
    }
}
