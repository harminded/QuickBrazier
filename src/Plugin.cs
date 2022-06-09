using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

namespace QuickBrazier
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [Reloadable]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static ConfigEntry<float> range;
        private Harmony _hooks;

        private void InitConfig()
        {
            range = Config.Bind("Server", "range", 5.0f,
                "Maximum distance to toggle Mist Braziers. 5 'distance' is about 1 tile.");
        }

        public override void Load()
        {
            Logger = Log;
            InitConfig();
            Shared.Load();
            if (VWorld.IsClient)
            {
                Client.Load();
            }

            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override bool Unload()
        {
            Config.Clear();
            Shared.Unload();
            if (VWorld.IsClient)
            {
                Client.Unload();
            }

            _hooks.UnpatchSelf();
            return true;
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