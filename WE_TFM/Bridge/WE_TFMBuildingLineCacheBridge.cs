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
    }
}
