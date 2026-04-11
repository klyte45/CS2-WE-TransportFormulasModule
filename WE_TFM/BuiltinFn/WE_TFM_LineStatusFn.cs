using Game.Prefabs;
using System;
using System.Collections.Generic;
using Unity.Entities;
using WE_TFM.Systems;

namespace WE_TFM.BultinFn
{
    public static class WE_TFM_LineStatusFn
    {
        public unsafe static List<Entity> GetCityLines(TransportType tt, bool acceptCargo, bool acceptPassenger) => WE_TFM_LineStatusSystem.Instance.GetCityLines(tt, acceptCargo, acceptPassenger);
    }
}
