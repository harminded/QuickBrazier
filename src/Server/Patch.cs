using HarmonyLib;
using ProjectM.Gameplay.Systems;

namespace QuickBrazier.Server;

[HarmonyPatch]
public class Patch
{
    private static DayNightCycleTracker _dayNightCycleTracker;

    public static void Load()
    {
        _dayNightCycleTracker = new DayNightCycleTracker();
        _dayNightCycleTracker.OnTimeOfDayChanged += AutoToggle.OnTimeOfDayChanged;
    }

    [HarmonyPatch(typeof(InteractSystemClient), nameof(InteractSystemClient.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdate(InteractSystemClient __instance)
    {
       
        if (!Plugin.AutoToggleEnabled.Value) return;
        _dayNightCycleTracker.Update(__instance._DayNightCycle.GetSingleton());
    }

    
}