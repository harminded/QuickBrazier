﻿using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using QuickBrazier.Server;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Wetstone.API;
using Patch = QuickBrazier.Client.Patch;

namespace QuickBrazier
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [Reloadable]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static ConfigEntry<float> Range;
        public static ConfigEntry<bool> AutoToggleEnabled;
        public static Keybinding ConfigKeybinding;
        private Harmony _hooks;

        private void InitConfig()
        {
            Range = Config.Bind("Server", "range", 5.0f,
                "Maximum distance to toggle Mist Braziers. 5 'distance' is about 1 tile.");
            AutoToggleEnabled = Config.Bind("Server", "autoToggleEnabled", true,
                "Turn braziers on when day starts, and off during the night starts, for online players/clans only.");
        }

        public override void Load()
        {
            Logger = Log;
            InitConfig();
            RegisterMessage();
            
            if (VWorld.IsClient)
            {
                Patch.Load();
                RegisterKeybind();
            }

            if (VWorld.IsServer)
            {
                Server.Patch.Load();
            }

            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void RegisterKeybind()
        {
            ConfigKeybinding = KeybindManager.Register(new()
            {
                Id = "harminded.quickbrazier.toggle",
                Category = "QuickBrazier",
                Name = "Toggle",
                DefaultKeybinding = KeyCode.L,
            });
        }

        private static void RegisterMessage()
        {
            VNetworkRegistry.RegisterServerboundStruct<ToggleBrazierMessage>(Listener.OnToggleBrazierMessage);
        }

        public override bool Unload()
        {
            Config.Clear();
            UnregisterMessage();
            if (VWorld.IsClient)
            {
                UnregisterKeybind();
            }

            _hooks.UnpatchSelf();
            return true;
        }

        private static void UnregisterMessage()
        {
            VNetworkRegistry.UnregisterStruct<ToggleBrazierMessage>();
        }

        private static void UnregisterKeybind()
        {
            KeybindManager.Unregister(ConfigKeybinding);
        }

        public static void Debug(object message, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            Logger.LogInfo($"[{caller}:{lineNumber}]: {message.ToString()}");
        }

        public static void DebugNativeArray(NativeArray<Entity> nativeArray, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            for (int i = 0; i < nativeArray.Length; i++)
            {
                Logger.LogInfo($"[{caller}:{lineNumber}]: {nativeArray[i].ToString()} ({nativeArray[i].GetType()})");
            }
        }

        public static void DebugComponentTypes(NativeArray<ComponentType> nativeArray,
            [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            for (int i = 0; i < nativeArray.Length; i++)
            {
                Logger.LogInfo($"[{caller}:{lineNumber}]: {nativeArray[i].ToString()} ({nativeArray[i].GetType()})");
            }
        }
    }
}