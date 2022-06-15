using System;
using System.Collections.Generic;
using HarmonyLib;
using ProjectM.Network;
using Unity.Entities;

namespace QuickBrazier.Server
{
    [HarmonyPatch]
    public class Listener
    {
        private static readonly Dictionary<Entity, DateTime> CooldownTracker = new();

        public static void OnToggleBrazierMessage(FromCharacter fromCharacter, ToggleBrazierMessage msg)
        {
            if (fromCharacter.Character == Entity.Null)
            {
                return;
            }

            if (CooldownTracker.ContainsKey(fromCharacter.Character) &&
                DateTime.Now - CooldownTracker[fromCharacter.Character] < TimeSpan.FromSeconds(0.25))
            {
                return;
            }
            
            CooldownTracker[fromCharacter.Character] = DateTime.Now;
            ManualToggle.ToggleNearestBrazierInRange(fromCharacter);
        }
    }
}