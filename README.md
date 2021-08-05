# Memoria FF PR
This is a small modification that makes gameplay of [Final Fantasy I (PR)](https://store.steampowered.com/app/1173770/FINAL_FANTASY/), [Final Fantasy II (PR)](https://store.steampowered.com/app/1173780/FINAL_FANTASY_II/) and [Final Fantasy III (PR)](https://store.steampowered.com/app/1173790/FINAL_FANTASY_III/) more enjoyable. 

## Installation:
- Unpack [BepInEx Loader x64 6.0.0-be.401](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2021.08.02/Loader_v2021.08.01.zip) into the game folder.
- Unpack one of the archives into the game folder:
    - [FF1](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2021.08.05/FF1_v2021.08.05.zip)
    - [FF2](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2021.08.05/FF2_v2021.08.05.zip)
    - [FF3](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2021.08.05/FF3_v2021.08.05.zip)

If you are already using BepInEx to load other mods, use the most recent version of the loader.

## Features:

- You can increase game speed (Default Key: F1).
- You can disable encounters (Default Key: F2).
- You can export, edit and import text resources (like characteristics of enemies) (Export disabled by default)

## Configuration:

- Edit file `$GameFolder$\BepInEx\config\Memoria.ffpr.cfg`

## Export:

@Eatitup86:
Just providing these in case you want to use them. I posted this on a FF Modding Discord that has been collaborating a lot with the new pixel remasters. Will need updated just a bit if you resolve that Export always on issue. :)

1. Download BepInEx Loader + Mod v2021.08.02 from: https://github.com/Albeoris/Memoria.FF1
2. Extract to the root of your game install.
3. Run your game + close it.
4. Navigate to InstallDir\BepInEx\config and open Memoria.ffpr.cfg
5. Set ExportEnabled = true
6. Run your game again, wait a bit for extraction to complete then close it.
7. Navigate to: InstallDir\FINAL FANTASY_Data\StreamingAssets\Assets\GameAssets\Serial\Data to explore the files within and make any modifications you would like to try.
8. Launch the game again and the changes will be applied automatically.


![screen](https://i.imgur.com/1IrVylI.png)
