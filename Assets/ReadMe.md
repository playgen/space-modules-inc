#Unity Project Structure

## Animations
- Animations used in the UI
- **NOTE**: character facial animations are handled through code    

## DeepLink iOS
- Setup for deep links on iOS, to allow for the game to be launched and setup through a url on iOS devices 

## DeepLink Plugin
- Setup for deep links on Android, to allow for the game to be launched and setup through a url on iOS devices 

## Editor
- Editor tools and configurations, Editor folders will not be built when unity builds the game for any platform. Tools include: 
    - ExcelToJson
    - Localization
    - SUGAR
    - FATiMAImporter

## EvaluationAsset
- RAGE Evaluation Asset Classes

## Fonts
- Fonts available in game, try to keep the count to 3 or less

## GamwWork
- Compiled GameWork files

## Plugins
- You can add plug-ins to your Project to extend Unity’s features. Plug-ins are native DLLs that are typically written in C/C++. They can access third-party code libraries, system calls and other Unity built-in functionality. Always place plug-ins in a folder called Plugins for them to be detected by Unity. Project contains the following plugins
    - RAGE Integrated Authoring tool 
    - PlayGen utitlites

## Prefabs
- UI Prefabs to be instantiated and loaded during run time

## RAGE Analytics
- RAGE Tracker Asset 

## Resources
- Objects in the Resources folder are always built, so make sure to clean up any unused assets.
    - **Audio/**: Audio files for characters, both male and female variants
        - These are included in Resources to allow for compression and reducing build size.
    - **Localization/**: Localization string files
    - **Prefabs/**: Prefabs that are loaded by name and instantiated during runtime
    - **Scenarios/**: Scenarios to be loaded at the beginning of each round
    - **Sprites/**: Character sprites and sprites for modules loaded at runtime. Contains all facial and body sprites for characters

## ScenarioRelated & Scenario
- Scenario definitions, provided by SPL, if game fails to find any scenarios on game load. reimport these folders

## Scenes
- Location of game scene, SMI.unity

## Scripts
- Game behaviours, managing state changes, loading of scenarios and UI transitions.

## Sprites
- Sprites that are set up in prefabs and in scene, not loaded by name at runtime, most sprites are Power of 2 resolution to allow for efficient compressing and packing of images to save file size.

## StreamingAssets
- Assets in streaming assets folder are saved to the filesystem of the target machine and retrieved at runtime by filepath, useful for large files that should not be compressed, and config files. Config files can be changed on target machines and then the game launched again with different settings.

## SUGAR
- SUGAR assets, pulled in from SUGAR unity project and redesigned to match space modules inc. UI.