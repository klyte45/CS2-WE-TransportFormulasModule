using System;

namespace WE_TFM.Bridge
{
    [Obsolete("Don't reference methods on this class directly. Always use reverse patch to access them, and don't use this mod DLL as hard dependency of your own mod.", true)]
    public static class WE_TFMPlatformMappingBridge
    {
        public unsafe static uint GetCacheVersion() => WE_TFM_PlatformMappingSystem.CacheVersion;
    }
}
