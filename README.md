# Memoria FF PR
This is a small modification that makes gameplay of [Final Fantasy I (PR)](https://store.steampowered.com/app/1173770/FINAL_FANTASY/), [Final Fantasy II (PR)](https://store.steampowered.com/app/1173780/FINAL_FANTASY_II/), [Final Fantasy III (PR)](https://store.steampowered.com/app/1173790/FINAL_FANTASY_III/), [Final Fantasy IV (PR)](https://store.steampowered.com/app/1173800/FINAL_FANTASY_IV/), [Final Fantasy V (PR)](https://store.steampowered.com/app/1173810/FINAL_FANTASY_V/) and [Final Fantasy VI (PR)](https://store.steampowered.com/app/1173820/FINAL_FANTASY_VI/) more enjoyable. 

## Installation:
- Unpack [BepInEx_UnityIL2CPP_x64_1504d80_6.0.0-be.546](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/BepInEx_UnityIL2CPP_x64_1504d80_6.0.0-be.546.zip) into the game folder.
- Unpack one of the archives into the game folder:
    - [FF1](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/FF1_v2022.02.12.zip)
    - [FF2](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/FF2_v2022.02.12.zip)
    - [FF3](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/FF3_v2022.02.12.zip)
    - [FF4](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/FF4_v2022.02.12.zip)
    - [FF5](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.02.12/FF5_v2022.02.12.zip)

If you are already using BepInEx to load other mods, use the most recent version of the loader.

## Deinstalation:
- To remove the mod - delete $GameFolder$\BepInEx\plugins\Memoria.FF*.dll
- To remove the mod launcer - delete $GameFolder$\winhttp.dll

## Features:

- You can increase game speed (Default Key: F1).
- You can disable encounters (Default Key: F2).
- You can use quick save and load (Default Keys: (Release) F5 and F9, (Current) Alt+F5, Alt+F9).
- You can [highlight interactive objects and hidden passages](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Highlighting) (Default key: Caps Lock [Toggle or Hold])
- You can [switch between ATB/Command battle systems](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Battle-System)
- [FF2: Color used words and items](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-FF2-Color-Secret-Words-and-Key-Items)
- You can export, edit and import graphics and text resources (like characteristics of enemies) (Export disabled by default)
- [Partial modification](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Mods) of CSV resources

## Configuration:

1. Start the game first.
2. Wait, the first launch will take some time.
3. Features that are enabled by default (for example, increasing the speed of the game) will already work.
4. Close the game and edit the configuration file `$GameFolder$\BepInEx\config\Memoria.FFPR\$Section$.cfg`

## Troubleshooting:

- Share mod logs: $GameFolder$\BepInEx\LogOutput.log
- Create an issue.

## Export:

@Eatitup86:
Just providing these in case you want to use them. I posted this on a FF Modding Discord that has been collaborating a lot with the new pixel remasters. Will need updated just a bit if you resolve that Export always on issue. :)

1. Download BepInEx Loader + Mod from: https://github.com/Albeoris/Memoria.FFPR
2. Extract to the root of your game install.
3. Run your game + close it.
4. Navigate to InstallDir\BepInEx\config and open Memoria.ffpr.cfg
5. Set ExportEnabled = true
6. Run your game again, wait a bit for extraction to complete then close it.
7. Navigate to: InstallDir\FINAL FANTASY_Data\StreamingAssets\Assets\GameAssets\Serial\Data to explore the files within and make any modifications you would like to try.
8. Launch the game again and the changes will be applied automatically.


![screen](https://i.imgur.com/1IrVylI.png)
