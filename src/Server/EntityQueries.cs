using ProjectM;
using ProjectM.Network;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

namespace QuickBrazier.Server;

public class EntityQueries
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