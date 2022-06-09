using System;
using System.Collections.Generic;
using ProjectM;
using ProjectM.Network;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Wetstone.API;

namespace QuickBrazier
{
    public static class Server
    {
        private static readonly ComponentType[] BonfireComponents =
        {
            ComponentType.ReadOnly(Il2CppType.Of<Bonfire>())
        };

        private static readonly PrefabGUID BoneGUID = new(1821405450);

        private static readonly Dictionary<Entity, DateTime> CooldownTracker = new();

        public static void ToggleNearestBrazier(FromCharacter fromCharacter, ToggleBrazierMessage msg)
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

            Entity brazier = FindClosestBrazierInRange(fromCharacter.Character);
            if (brazier != Entity.Null)
            {
                ToggleBrazier(brazier, out bool isBurning);
                if (isBurning)
                {
                    AddBonesIfEmpty(fromCharacter, brazier);    
                }
            }
        }

        public static void AddBonesIfEmpty(FromCharacter fromCharacter, Entity brazier)
        {
            InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, fromCharacter.Character, out Entity playerInventory);
            InventoryUtilities.TryGetInventoryEntity(VWorld.Server.EntityManager, brazier, out Entity brazierInventory);
            var gameDataSystem = VWorld.Server.GetExistingSystem<GameDataSystem>();

            int playerBoneCount = InventoryUtilities.ItemCount(VWorld.Server.EntityManager, playerInventory, BoneGUID);
            int brazierBoneCount = InventoryUtilities.ItemCount(VWorld.Server.EntityManager, brazierInventory, BoneGUID);
            if (playerBoneCount > 0 && brazierBoneCount == 0)
            {
                InventoryUtilities.TryGetItemSlot(VWorld.Server.EntityManager, playerInventory, BoneGUID, out int slotId);
                InventoryUtilitiesServer.SplitItemStacks(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId);
                InventoryUtilitiesServer.TryMoveItem(VWorld.Server.EntityManager, gameDataSystem.ItemHashLookupMap,
                    playerInventory, slotId, brazierInventory, out bool moved);
            }
        }

        private static Entity FindClosestBrazierInRange(Entity interactor)
        {
            var entities = GetBonfireEntities(VWorld.Server.EntityManager);
            Entity closest = Entity.Null;
            float closestDistance = Mathf.Infinity;
            foreach (var entity in entities)
            {
                float distance = DistanceFromInteractor(interactor, entity, VWorld.Server.EntityManager);
                if (distance <= Plugin.range.Value && distance < closestDistance)
                {
                    closest = entity;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private static NativeArray<Entity> GetBonfireEntities(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(BonfireComponents);
            return query.ToEntityArray(Allocator.Temp);
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

        private static void ToggleBrazier(Entity brazier, out bool isBurning)
        {
            var brazierComponentData = VWorld.Server.EntityManager.GetComponentData<BurnContainer>(brazier);
            brazierComponentData.Enabled = !brazierComponentData.Enabled;
            VWorld.Server.EntityManager.SetComponentData(brazier, brazierComponentData);
            isBurning = brazierComponentData.Enabled;
        }
    }
}