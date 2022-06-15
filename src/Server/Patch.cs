using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

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

    [HarmonyPatch(typeof(HandleGameplayEventsSystem), nameof(HandleGameplayEventsSystem.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdate(HandleGameplayEventsSystem __instance)
    {
        if (!Plugin.AutoToggleEnabled.Value) return;
        _dayNightCycleTracker.Update(__instance._DayNightCycle.GetSingleton());
    }

    
}