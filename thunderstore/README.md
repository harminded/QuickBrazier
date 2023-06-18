# QuickBrazier

Original mod by [Harminded](https://v-rising.thunderstore.io/package/Harminded/). Ported to Gloomrot by [p1xel8ted](https://v-rising.thunderstore.io/package/p1xel8ted/).

## Installation (Manual)

1. Install [BepInExPack V Rising](https://v-rising.thunderstore.io/package/BepInEx/BepInExPack_V_Rising/)
2. Install [Bloodstone API](https://v-rising.thunderstore.io/package/deca/Bloodstone/)
3. Extract quick_brazier.dll into `(VRising/BepInEx/plugins`
4. Extract quick_brazier.dll into `(VRising/VRising_Server/BepInEx/plugins`

* [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) recommended for in-game hosted
  games
* (Optional) If not using ServerLaunchFix, extract quick_brazier.dll into (VRising server folder)/BepInEx/plugins

## How to use

* Toggle nearest brazier in range with L (bindable)
* Braziers will automatically turn on when day starts, and turn off when night starts. Only for online players/clans.

## Configuration

Config file is located at `(VRising client/server folder)/VRising/BepInEx/config/quick_brazier.cfg`

```
## Settings file was created by plugin quick_brazier v0.4.1
## Plugin GUID: quick_brazier

[Server]

## Maximum distance to toggle Mist Braziers. 5 'distance' is about 1 tile.
# Setting type: Single
# Default value: 5
range = 5

## Turn braziers on when day starts, and off during the night starts, for online players/clans only.
# Setting type: Boolean
# Default value: true
autoToggleEnabled = true
```