using QuickBrazier;

namespace Wetstone.API;

public class Shared
{
    public static void Load()
    {
        VNetworkRegistry.RegisterServerboundStruct<ToggleBrazierMessage>((fromCharacter, msg) =>
        {
            Server.ToggleNearestBrazier(fromCharacter, msg);
        });
    }

    public static void Unload()
    {
        VNetworkRegistry.UnregisterStruct<ToggleBrazierMessage>();
    }
}