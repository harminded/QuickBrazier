using HarmonyLib;
using ProjectM;

namespace QuickBrazier.Client;

[HarmonyPatch]
public class Patch
{
    private static Toggler _toggler;

    public static void Load()
    {
        _toggler = new Toggler();
    }

    [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
    [HarmonyPostfix]
    public static void HandleInput(GameplayInputSystem __instance, InputState inputState)
    {
        _toggler.Update();
    }
}