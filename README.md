# Memoria FF PR
This is a small modification that makes gameplay of [Final Fantasy I (PR)](https://store.steampowered.com/app/1173770/FINAL_FANTASY/), [Final Fantasy II (PR)](https://store.steampowered.com/app/1173780/FINAL_FANTASY_II/), [Final Fantasy III (PR)](https://store.steampowered.com/app/1173790/FINAL_FANTASY_III/), [Final Fantasy IV (PR)](https://store.steampowered.com/app/1173800/FINAL_FANTASY_IV/), [Final Fantasy V (PR)](https://store.steampowered.com/app/1173810/FINAL_FANTASY_V/) and [Final Fantasy VI (PR)](https://store.steampowered.com/app/1173820/FINAL_FANTASY_VI/) more enjoyable. 

## Support
[Patreon](https://www.patreon.com/Albeoris?fan_landing=true)

## Installation:
- Unpack [BepInEx_UnityIL2CPP_x64_3a54f7e_6.0.0-be.571](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.07.16/BepInEx_UnityIL2CPP_x64_3a54f7e_6.0.0-be.571.zip) into the game folder.
- Unpack one of the archives into the game folder:
    - [FF1](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF1_v2022.08.07.zip)
    - [FF2](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF2_v2022.08.07.zip)
    - [FF3](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF3_v2022.08.07.zip)
    - [FF4](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF4_v2022.08.07.zip)
    - [FF5](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF5_v2022.08.07.zip)
    - [FF6](https://github.com/Albeoris/Memoria.FFPR/releases/download/v2022.08.07/FF6_v2022.08.07.zip)

If you are already using BepInEx to load other mods, use the most recent version of the loader.

If you playing on Steam Deck check [this page](https://github.com/Albeoris/Memoria.FFPR/wiki/Steam-Deck).

## Deinstalation:
- To remove the mod - delete $GameFolder$\BepInEx\plugins\Memoria.FF*.dll
- To remove the mod launcer - delete $GameFolder$\winhttp.dll

## Features:

- You can increase game speed (Default Key: F1).
- You can disable encounters (Default Key: F2).
- You can use quick save and load (Default Keys: (Release) F5 and F9, (Current) Alt+F5, Alt+F9).
- You can [highlight interactive objects and hidden passages](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Highlighting) (Default key: Caps Lock [Toggle or Hold])
- You can [switch to Turn-based ATB Combat](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Turn-based-ATB-Combat)
- [FF2: Color used words and items](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-FF2-Color-Secret-Words-and-Key-Items)
- [FF6: Gau Improvements](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-FF2-Gau-Improvements)
- [FF6: Easy Blitz](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-FF2-Easy-Blitz)
- [FF6: Modable Magicite.json](https://github.com/Albeoris/Memoria.FFPR/issues/27#issuecomment-1186271402)
- You can export, edit and import graphics and text resources (like characteristics of enemies) (Export disabled by default)
- [Partial modification](https://github.com/Albeoris/Memoria.FFPR/wiki/Features-Mods) of CSV and TXT resources

## Configuration:

1. Start the game first.
2. Wait, the first launch will take some time. (about 1 minute)
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
4. Navigate to InstallDir\BepInEx\config\Memoria.FFPR\ and open Assets.cfg
5. Set ExportEnabled = true
6. Run your game again, wait a bit for extraction to complete then close it.
7. Navigate to: InstallDir\FINAL FANTASY_Data\StreamingAssets\Assets\GameAssets\Serial\Data to explore the files within and make any modifications you would like to try.
8. Launch the game again and the changes will be applied automatically.


![screen](https://i.imgur.com/1IrVylI.png)
