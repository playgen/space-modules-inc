# Unity Project Structure

## Animations
- Animations used in the UI.
- **NOTE**: Character facial animations are handled through code.

## DeepLink iOS
- Setup for deep links on iOS, which allows for the game to be launched and set-up through a URL via SUGAR on iOS devices.

## DeepLink Plugin
- Setup for deep links on Android, which allows for the game to be launched and set-up through a URL via SUGAR on Android devices.

## Editor
- Editor tools and configurations. Editor folders will not be built when Unity builds the game for any platform.
- Sub-folders and files include: 
    - ExcelToJson
    - Localization
    - SUGAR
    - FAtiMAImporter

## EvaluationAsset
- Classes which add RAGE Evaluation Asset functionality.

## Fonts
- Fonts available in game.

## GamwWork
- Compiled GameWork files.

## Plugins
- Contains complied files for:
    - FAtiMA Toolkit (within the IntegratedAuthoringTool folder).
    - PlayGen Unity Utilities (within the PlayGen Utilities folder).
    - Deeplink for Android (within the Android folder).
- The Android folder also contains the AndroidManifest.

## Prefabs
- UI prefabs primarily for creating the Settings UI and for setting a standardised UI panel.

## RAGE Analytics
- Class which add RAGE Analytics functionality. 

## Resources
- Objects in the Resources folder are always built, so make sure to clean up any unused assets.
    - **Audio**: Audio files for characters, both male and female variants. Included in Resources to allow for compression and as a result a reduced build size.
    - **Localization**: Localization JSON files for UI and scenario dialogue.
    - **Prefabs**: Prefabs that are loaded by name and instantiated during runtime.
    - **ScenarioRelated**: Text versions of the files that are used within scenarios, which are loaded when required during runtime.
    - **Scenarios**: Text versions of the scenarios, which are loaded at the beginning of every game session.
    - **Sprites**: Character sprites and sprites for modules loaded at runtime. Contains all facial and body sprites for characters.

## ScenarioRelated & Scenario
- Scenario definitions, provided by SPL.
- If the game fails to find any scenarios or scenarios used appear to be older versions, reimport these folders to rebuild the text files found in Resources.

## Scenes
- Location of game scene, SMI.unity.
- The naming of GameObjects within the scene is used in StateInput classes to setup each state. As a result, if changes are made to the naming or ordering of objects in scene, check that the related StateInput remains in sync.

## Scripts
- Contains all scripts related to game logic (excluding SUGAR), including game behaviour, UI behaviour, state management and loading scenarios.

## Sprites
- Sprites that are used in prefabs and in scene and are not loaded by name at runtime.
- All sprites used have widths and heights divisible by 4 so that they can be crunch compressed, resulting in a smaller build size.

## StreamingAssets
- Assets within this folder are saved to the file system of the target machine and retrieved at runtime by filepath, which is useful for large files that should not be compressed and config file. This also means that config files can be changed on target machines in order to change game settings.
- Contains config files for SUGAR, scenario loading and available modules.

## SUGAR
- SUGAR assets, including complied files, template scripts and generic sprites and animations, pulled in from the [SUGAR Unity project](https://github.com/playgen/SUGAR-Unity) and redesigned to match the UI of Space Modules Inc.
- Also contains AccountUnityClientAdditions, which is used on top of the standard AccountUnityClient logic and handles signing into SUGAR after launching via a DeepLink URL on iOS or Android.