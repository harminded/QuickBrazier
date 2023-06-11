using System;
using Bloodstone.API;
using UnityEngine;

namespace QuickBrazier.Client;

public class Toggler
{
    private DateTime _lastToggle = DateTime.Now;

    public void Update()
    {
        if ((Input.GetKeyInt(Plugin.ConfigKeybinding.Primary) || Input.GetKeyInt(Plugin.ConfigKeybinding.Secondary)) &&
            DateTime.Now - _lastToggle > TimeSpan.FromSeconds(0.25))
        {
            _lastToggle = DateTime.Now;
            ToggleBrazier();
        }
    }

    private void ToggleBrazier()
    {
        VNetwork.SendToServerStruct<ToggleBrazierMessage>(new()
        {
        });
    }
}