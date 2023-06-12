using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using Unity.Entities;
using Bloodstone.API;

namespace QuickBrazier.Server;

public static class AutoToggle
{
    public static void OnTimeOfDayChanged(TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Night:
                TurnOffAllBonfires();
                return;
            case TimeOfDay.Day:
                TurnOnBonfiresForOnlinePlayersOnly();
                break;
            case TimeOfDay.All:
                // ???
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, null);
        }
    }

    private static void TurnOffAllBonfires()
    {
        var bonfireEntities = EntityQueries.GetBonfireEntities();
        ManualToggle.SetBurning(bonfireEntities, false);
    }

    private static void TurnOnBonfiresForOnlinePlayersOnly()
    {
        var userEntities = EntityQueries.GetUserEntities();
        var onlineTeams = new List<Team>();
        foreach (var userEntity in userEntities)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(userEntity);
            if (!user.IsConnected) continue;
            var userTeam = VWorld.Server.EntityManager.GetComponentData<Team>(userEntity);
            onlineTeams.Add(userTeam);
        }

        var onlineBraziers = GetBonfiresFromOnlineTeams(onlineTeams);
        foreach (var bonfire in onlineBraziers)
        {
            ManualToggle.SetBurning(bonfire, true);
        }
    }

    private static List<Entity> GetBonfiresFromOnlineTeams(List<Team> onlineTeams)
    {
        var serverGameManager = VWorld.Server.GetExistingSystem<ServerScriptMapper>()?._ServerGameManager;
        var entities = EntityQueries.GetBonfireEntities();
        var onlineBraziers = new List<Entity>();
        foreach (var entity in entities)
        {
            var bonfireTeam = VWorld.Server.EntityManager.GetComponentData<Team>(entity);
            if (onlineTeams.Any(onlineTeam => serverGameManager != null && serverGameManager.Value.IsAllies(bonfireTeam, onlineTeam)))
            {
                onlineBraziers.Add(entity);
            }
        }

        return onlineBraziers;
    }
}