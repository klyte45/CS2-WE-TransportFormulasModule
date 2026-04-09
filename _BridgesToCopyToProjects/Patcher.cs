public static class Patcher
{
    internal bool DoPatches()
    {
        return ConnectBridge("WE_TFM", [
                (typeof(WE_TFMBuildingLineCacheBridge), "WE_TFMBuildingLineCacheBridge"),
             (typeof(WE_TFMComponentGetterBridge), "WE_TFMComponentGetterBridge"),
             (typeof(WE_TFMIncomingVehicleBridge), "WE_TFMIncomingVehicleBridge"),
             (typeof(WE_TFMLineStatusBridge), "WE_TFMLineStatusBridge"),
             (typeof(WE_TFMPlatformMappingBridge), "WE_TFMPlatformMappingBridge"),
            ]);
    }
    private static bool ConnectBridge(string dllName, List<(Type, string)> typesMapping)
    {
        try
        {
            if (AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == dllName) is Assembly weAssembly)
            {
                var exportedTypes = weAssembly.ExportedTypes;
                foreach (var (type, sourceClassName) in typesMapping)
                {
                    var targetType = exportedTypes.First(x => x.Name == sourceClassName);
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        MethodInfo srcMethod;
                        if (method.TryGetAttribute<PatchGenericMethod>(out var attribute))
                        {
                            var targetMethodName = attribute.OriginalMethodName ?? method.Name;
                            MethodInfo[] methods = targetType.GetMethods(allFlags);
                            srcMethod = methods.FirstOrDefault(x => x.Name == targetMethodName && x.IsGenericMethod && x.GetGenericArguments().Length == attribute.Types.Length);
                            if (srcMethod == null)
                            {
                                log.Warn($"Method not found while patching {dllName}: {targetType.FullName} {targetMethodName}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))}) with generic arguments [{string.Join(", ", attribute.Types.Select(x => x.FullName))}] - Searched for {targetMethodName}");
                                continue;
                            }
                            if (!srcMethod.IsGenericMethod || srcMethod.GetGenericArguments().Length != attribute.Types.Length)
                            {
                                log.Warn($"Method not found while patching {dllName}: {targetType.FullName} {srcMethod.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))}) with generic arguments [{string.Join(", ", [.. attribute.Types.Select(x => x.FullName)])}] - Incompatible types (found: [{string.Join(", ", [.. srcMethod.GetGenericArguments().Select(x => x.FullName)])}])");
                                continue;
                            }
                            srcMethod = srcMethod.MakeGenericMethod(attribute.Types);
                            if (!srcMethod.GetParameters().Types().SequenceEqual(method.GetParameters().Types()) || srcMethod.ReturnType != method.ReturnType)
                            {
                                log.Warn($"Method not found while patching {dllName}: {targetType.FullName} {srcMethod.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))}) with generic arguments [{string.Join(", ", attribute.Types.Select(x => x.FullName))}] - Parameter or return type mismatch (found: ({string.Join(", ", srcMethod.GetParameters().Select(x => $"{x.ParameterType}"))}) => {srcMethod.ReturnType.FullName})");
                                continue;
                            }
                        }
                        else
                        {
                            srcMethod = targetType.GetMethod(method.Name, allFlags, null, [.. method.GetParameters().Select(x => x.ParameterType)], null);
                            if (srcMethod == null)
                            {
                                log.Warn($"Method not found while patching {dllName}: {targetType.FullName} {method.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))})");
                                continue;
                            }
                            if (srcMethod.IsGenericMethod)
                            {
                                log.Warn($"Method {srcMethod} is generic but doesn't have {nameof(PatchGenericMethod)} attribute while patching {dllName}: {targetType.FullName} {srcMethod.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))})");
                                continue;
                            }
                        }
                        if (srcMethod != null)
                        {
                            Harmony.ReversePatch(srcMethod, new HarmonyMethod(method));
                            log.Info($"Reverse Patched: {srcMethod} => {method}");
                        }
                        else
                        {
                            log.Warn($"Method not found while patching {dllName}: {targetType.FullName} {srcMethod.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType}"))})");
                        }
                    }
                }
                return true;
            }
            else
            {
                log.Warn($"{dllName}.dll file required for using this mod! Check if it's enabled.");
                return false;
            }
        }
        catch (Exception e)
        {
            log.Warn($"{dllName}.dll file required for using this mod! Check if it's enabled. Error loading.\n{e}");
            return false;
        }
    }
}