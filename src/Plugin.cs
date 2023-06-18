using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using HarmonyLib;
using QuickBrazier.Server;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Patch = QuickBrazier.Client.Patch;

namespace QuickBrazier
{
    [BepInPlugin("quick_brazier", "QuickBrazier", "0.4.1")]
    [BepInDependency("gg.deca.Bloodstone")]
    [Reloadable]
    public class Plugin : BasePlugin
    {
        private static ManualLogSource Logger { get;set; }
        public static ConfigEntry<float> Range { get; private set; }
        public static ConfigEntry<bool> AutoToggleEnabled { get;private set; }
        public static Keybinding ConfigKeybinding { get;private set; }
        private Harmony Harmony { get; set; }

        private void InitConfig()
        {
            Range = Config.Bind("Server", "Range", 5.0f,
                "Maximum distance to toggle Mist Braziers. 5 'distance' is about 1 tile.");
            AutoToggleEnabled = Config.Bind("Server", "Auto Toggle Enabled", true,
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

            Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"Plugin QuickBrazier is loaded!");
        }

        private static void RegisterKeybind()
        {
            ConfigKeybinding = KeybindManager.Register(new KeybindingDescription
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

            Harmony.UnpatchSelf();
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
            for (var i = 0; i < nativeArray.Length; i++)
            {
                Logger.LogInfo($"[{caller}:{lineNumber}]: {nativeArray[i].ToString()} ({nativeArray[i].GetType()})");
            }
        }

        public static void DebugComponentTypes(NativeArray<ComponentType> nativeArray,
            [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            for (var i = 0; i < nativeArray.Length; i++)
            {
                Logger.LogInfo($"[{caller}:{lineNumber}]: {nativeArray[i].ToString()} ({nativeArray[i].GetType()})");
            }
        }
    }
}