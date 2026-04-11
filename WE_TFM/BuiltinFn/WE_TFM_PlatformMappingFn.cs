using System;

namespace WE_TFM.BultinFn
{
    public static class WE_TFM_PlatformMappingFn
    {
        public unsafe static uint GetCacheVersion() => WE_TFM_PlatformMappingSystem.CacheVersion;
    }
}
