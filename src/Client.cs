using System;
using HarmonyLib;
using ProjectM;
using UnityEngine;
using Wetstone.API;

namespace QuickBrazier
{
    [HarmonyPatch]
    public class Client
    {
        private static DateTime _lastInventoryTransfer = DateTime.Now;
        private static Keybinding configKeybinding;

        public static void Load()
        {
            configKeybinding = KeybindManager.Register(new()
            {
                Id = "harminded.quickbrazier.toggle",
                Category = "QuickBrazier",
                Name = "Toggle",
                DefaultKeybinding = KeyCode.L,
            });
        }

        public static void Unload()
        {
            KeybindManager.Unregister(configKeybinding);
        }

        [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
        [HarmonyPostfix]
        public static void HandleInput(GameplayInputSystem __instance, InputState inputState)
        {
            if (!VWorld.IsClient)
            {
                return;
            }

            if ((Input.GetKeyInt(configKeybinding.Primary) ||
                 Input.GetKeyInt(configKeybinding.Secondary)) &&
                DateTime.Now - _lastInventoryTransfer > TimeSpan.FromSeconds(0.25))
            {
                Plugin.Debug("BRAZIER");
                _lastInventoryTransfer = DateTime.Now;
                ToggleBrazier();
            }
        }

        private static void ToggleBrazier()
        {
            VNetwork.SendToServerStruct<ToggleBrazierMessage>(new()
            {
            });
        }
    }
}