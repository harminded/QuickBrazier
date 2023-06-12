using System;
using HarmonyLib;
using ProjectM.Gameplay.Systems;

namespace QuickBrazier.Server;

[HarmonyPatch]
public static class Patch
{
    private static DayNightCycleTracker _dayNightCycleTracker;

    public static void Load()
    {
        _dayNightCycleTracker = new DayNightCycleTracker();
        _dayNightCycleTracker.OnTimeOfDayChanged += AutoToggle.OnTimeOfDayChanged;
    }

    [HarmonyPatch(typeof(HandleGameplayEventsBase), nameof(HandleGameplayEventsBase.OnUpdate))]
    [HarmonyPostfix]
    public static void HandleGameplayEventsBase_Patch(ref HandleGameplayEventsBase __instance)
    {
        if (!Plugin.AutoToggleEnabled.Value) return;
        if(__instance._ScriptMapper == null) return; 
        try
        {
            _dayNightCycleTracker.Update(__instance._ScriptMapper._DayNightCycleAccessor.GetSingleton());
        }
        catch (Exception)
        {
            //fail silently
        }
    }
}