using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Entities;
using WE_TFM.Components.Shareable;
using WE_TFM.Systems;

namespace WE_TFM.Bridge
{
    [Obsolete("Don't reference methods on this class directly. Always use reverse patch to access them, and don't use this mod DLL as hard dependency of your own mod.", true)]
    public static class WE_TFMBuildingLineCacheBridge
    {
        public unsafe static List<T> GetLines<T>(Entity selectedEntity, bool iterateToOwner) where T : unmanaged => [.. WE_TFM_BuildingLineCacheSystem.Instance.GetLines(selectedEntity, iterateToOwner).Select(x => Unsafe.As<LineDescriptor, T>(ref x))];

        public unsafe static List<T> GetFilteredLines<T>(Entity buildingEntity, string lineType, bool iterateToOwner, bool sortDescending) where T : unmanaged => [.. WE_TFM_BuildingLineCacheSystem.Instance.GetFilteredLines(buildingEntity, lineType, iterateToOwner, sortDescending).Select(x => Unsafe.As<LineDescriptor, T>(ref x))];

        public unsafe static T? GetLineByIndex<T>(Entity buildingEntity, string lineType, int index, bool iterateToOwner, bool sortDescending) where T : unmanaged
        {
            var result = WE_TFM_BuildingLineCacheSystem.Instance.GetLineByIndex(buildingEntity, lineType, index, iterateToOwner, sortDescending);
            if (result is null)
            {
                return null;
            }
            else
            {
                var x = result.Value;
                return Unsafe.As<LineDescriptor, T>(ref x);
            }

        }
    }
}
