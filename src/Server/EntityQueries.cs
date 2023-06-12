using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace QuickBrazier.Server;

public static class EntityQueries
{
    private static readonly ComponentType[] BrazierComponents =
    {
        ComponentType.ReadOnly(Il2CppType.Of<Bonfire>())
    };

    public static NativeArray<Entity> GetUserEntities()
    {
        var userQuery = VWorld.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>());
        return userQuery.ToEntityArray(Allocator.Temp);
    }

    public static NativeArray<Entity> GetBonfireEntities()
    {
        var query = VWorld.Server.EntityManager.CreateEntityQuery(BrazierComponents);
        return query.ToEntityArray(Allocator.Temp);
    }
}