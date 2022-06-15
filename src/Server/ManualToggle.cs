using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Wetstone.API;

namespace QuickBrazier.Server;

public class ManualToggle
{
    private static readonly PrefabGUID BoneGuid = new(1821405450);
    
    public static void ToggleNearestBrazierInRange(FromCharacter fromCharacter)
    {
        Entity brazier = FindClosestBrazierInRange(fromCharacter.Character);
        if (brazier != Entity.Null)
        {
            Toggle(brazier, out bool isBurning);
            if (isBurning)
            {
                AddBonesIfEmpty(fromCharacter, brazier);
            }
        }
    }
    
    private static Entity FindClosestBrazierInRange(Entity interactorEntity)
    {
        var bonfireEntities = EntityQueries.GetBonfireEntities();
        Entity closestBonfire = Entity.Null;
        float closestDistance = Mathf.Infinity;
        foreach (var bonfireEntity in bonfireEntities)
        {
            float distance = DistanceFromInteractor(interactorEntity, bonfireEntity, VWorld.Server.EntityManager);
            if (distance <= Plugin.Range.Value && distance < closestDistance)
            {
                closestBonfire = bonfireEntity;
                closestDistance = distance;
            }
        }
        return closestBonfire;
    }
    
    private static void Toggle(Entity brazier, out bool isBurning)
    {
        var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
        brazierComponentData.Enabled = !brazierComponentData.Enabled;
        VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
        isBurning = brazierComponentData.Enabled;
    }
    
    public static void SetBurning(Entity brazier, bool isBurning)
    {
        var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
        brazierComponentData.Enabled = isBurning;
        VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
    }
    
    public static void SetBurning(NativeArray<Entity> braziers, bool isBurning)
    {
        foreach (var brazier in braziers)
        {
            var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = isBurning;
            VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
        }
    }
    
    private static float DistanceFromInteractor(Entity entity, Entity interactor, EntityManager entityManager)
    {
        var interactorPosition = ToVector3(entityManager.GetComponentData<LocalToWorld>(interactor));
        var entityPosition = ToVector3(entityManager.GetComponentData<LocalToWorld>(entity));

        return Vector3.Distance(interactorPosition, entityPosition);
    }
    
    private static Vector3 ToVector3(LocalToWorld localToWorld)
    {
        return new Vector3(localToWorld.Position.x, localToWorld.Position.y, localToWorld.Position.z);
    }
    
    public static void AddBonesIfEmpty(FromCharacter fromCharacter, Entity brazier)
    {
        InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, fromCharacter.Character, out Entity playerInventory);
        InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, brazier, out Entity brazierInventory);
        var gameDataSystem = VWorld.Server.GetExistingSystem<GameDataSystem>();

        int playerBoneCount = InventoryUtilities.ItemCount(VWorld.Server.EntityManager, playerInventory, BoneGuid);
        int brazierBoneCount = InventoryUtilities.ItemCount(VWorld.Server.EntityManager, brazierInventory, BoneGuid);
        if (playerBoneCount > 0 && brazierBoneCount == 0)
        {
            InventoryUtilities.TryGetItemSlot(VWorld.Server.EntityManager, playerInventory, BoneGuid, out int slotId);
            InventoryUtilitiesServer.SplitItemStacks(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                playerInventory, slotId);
            InventoryUtilitiesServer.TryMoveItem(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                playerInventory, slotId, brazierInventory, out bool moved);
        }
    }
}