# Overview 
Space Modules Inc is a spaceship parts call center service agent simulator.

It is part of the [RAGE project](http://rageproject.eu/).

# Licensing
See the [LICENCE](LICENCE.md) file included in this project.

# Cloning
- When the project is cloned you will need to run the CreateLibJunctions.bat (on Windows) or MakeSymLinks.sh (on MacOS) file in order to set up the three SymLinks required for the project structure to be set up correctly. The sym links are:

From | To
--- | ---
..\lib\GameWork | ..\Assets\GameWork
..\lib\IntegratedAuthoringTool | ..\Assets\Plugins\IntegratedAuthoringTool
..\lib\PlayGenUtilities | ..\Assets\Plugins\PlayGen Utilities

# Key Project Structure

- **Unity Project**: *Project to be opened in the Unity Editor.*  
  - **Assets**  
    - **Editor**
      - **ExcelToJson**: *[Included-Assets](#included-assets)*
      - **FATiMAImporter**: *Utility to parse the scenario content files into a Unity readable format.* 
    - **DeepLink iOS**: *[Included-Assets](#included-assets)*  
    - **DeepLink Plugin**: *[Included-Assets](#included-assets)*  
    - **Evaluation Asset**: *[Included-Assets](#included-assets) - an asset which helps evaluate the pedagogical efficiency of a game.*  
    - **RAGE Analytics**: *[Included-Assets](#included-assets) - an asset to log to the RAGE analytics server.*  
    - **StreamingAssets**
      - **levelconfig.json**: *Configuration file for levels.*  
      - **modules-en.json**: *Space modules data in English.*  
      - **modules-nl.json**: *Space modules data in Dutch.*
    - **ScenariosRelated**: *Files used by the scenarios.*
    - **Scenarios**: *The scenarios in game.*
  - **lib**: *Precompiled [included-assets](#included-assets) libraries.*
    - **GameWork**: *Game Development Framework.*
    - **IntegratedAuthoringTool**: *An asset collection related to NPC conversations and emotions.*  
    - **PlayGenUtilities**: *A collection of simple Unity Utilities.*  
  - **space-modules-inc-setup**: *installer project*  
  - **Tools**: *Setup scripts.*

# Included Assets:
- [Integrated Authoring Tool](https://gamecomponents.eu/content/201)
- [Role-Play Character](https://www.gamecomponents.eu/content/196)
- [Emotional Appraisal Asset](https://www.gamecomponents.eu/content/224)
- [Emotional Decision Making Asset](https://www.gamecomponents.eu/content/218)
- [Social Importance Dynamics](https://www.gamecomponents.eu/content/207)
- [Client-Side Tracker](https://gamecomponents.eu/content/232)
- [Server-Side Interaction Storage and Analytics](https://www.gamecomponents.eu/content/220)
- [Server-Side Dashboard and Analysis](https://www.gamecomponents.eu/content/195)
- [Evaluation Component](https://gamecomponents.eu/content/338)
- [SUGAR](https://gamecomponents.eu/content/200)
- [PlayGen FAtiMA Toolkit Fork](https://gitlab.com/playgen/FAtiMA-Toolkit)
- [PlayGen Unity Utilities](https://github.com/playgen/unity-utilities) - a collection of simple Unity utilities.
- [GameWork](https://github.com/JaredGG/GameWork.Unity) - a game development framework.
- [ExcelToJsonConverter](https://github.com/Benzino/ExcelToJsonConverter) - used to convert Excel Localization files to JSON.
- [DeepLink iOS](https://github.com/TROPHiT/UnityDeeplinks)
- [DeepLink Plugin](https://assetstore.unity.com/packages/tools/integration/deeplink-plugin-30430) - for android.

# Development:
## Requirements:
- Windows
- Git LFS
- Visual Studio 2017
- Unity Editor

## Process
1. Run CreateLibJunctions.bat/MakeSymLinks.sh to create the required SymLinks.

2. In Unity's Project View, select the ScenarioRelated and Scenarios folders, right-click and select Reimport. This will cause the FATiMAImporter to generate the .txt versions and place them into the Resources folder.

    a. Note that while this process will overwrite existing files and add new files, it does not currently delete files when their counterpart in the Scenarios or ScenarioRelated folder is removed from the project.

3. To run the game with SUGAR functionality, please refer to the [SUGAR Unity Documentation](http://api.sugarengine.org/v1/unity-client/tutorials/index.html).

4. To run the game with RAGE Analytics functionality, please refer to the [RAGE Analytics Documentation](Assets/RAGE%20Analytics/ReadMe.md).

5. To run the game with Evaluation Asset functionality, please refer to the [Evaluation Asset Documentation](Assets/Evaluation%20Asset/ReadMe.md).

### SUGAR Custom Args:
Key | Potential Values | Description
-- | -- | --
round | 1, 2, 3... | Which round of scenarios to play.
feedback | 0, 1, 2 | The level of feedback provided to the player, ranging from end game only to throughout live conversations.
ingameq | true, false | Should the in game questionnaire be shown.
lockafterq | true, false |  Should the game be locked after all scenarios have been completed.
tstamp | yyyy-MM-ddTHH:mm | Timestamp related to when the game should be accessible.
forcelaunch |  | For debug purposes only. If the game should launch regardless of the validity of the timestamp. Any value will enable this functionality.
wipeprogress |  | For debug purposes only. Wipes the current progress in a round when playing over different sessions.

## Updating:
### FAtiMA Toolkit 
Using PlayGen's forked version of the FAtiMA Toolkit, build solution and place new DLLs (found in FAtiMA-Toolkit\Assets\IntegratedAuthoringTool\bin\Debug) into lib\IntegratedAuthoringTool folder. Note that code changes could be needed as a result of updates to this asset.

**Commit hash: 66bc9cfba528f8cbdeee2bb4c67b4ee77afc4b6a**

### GameWork.Unity
Build solution and place new DLLs (found in GameWork.Unity\bin) into lib\GameWork folder. Note that code changes could be needed as a result of updates to this asset.

**Commit hash: 37b623daf815667d6f523c38ab47304bfac82b22**

### PlayGen Unity Utilities 
Build solution and place new DLLs (found in various folders in this project) into lib\PlayGenUtilities folder. Note that code changes could be needed as a result of updates to this asset. New prefabs may also need copying, although take care not to overwrite customised versions of previous prefabs.  

New DLLs should also be copied into the lib\PlayGen Utilities folder in the SUGAR Unity project.

**Commit hash: 99d0daaa429430b36807bc5c28e567a61fc75e7d**

### SUGAR Unity Asset
Build solution and place new DLLs (found in SUGAR-Unity\Assets\SUGAR\Plugins) into Assets\SUGAR\Plugins folder. Note that code changes could be needed as a result of updates to this asset. It is advised that you do not remove the prefabs for SUGAR, as these have been edited to match the styling of Space Modules Inc. Only replace these assets if they are no longer compatible with the latest version of the SUGAR Unity asset, and even then be aware that you will need to recreate the previous styling.

**Commit hash: 51bbdcd3658af28823471a09f3be89dff0b641f9**

### RAGE Analytics
Follow the instructions provided in the [RAGE Analytics Documentation](Assets/RAGE%20Analytics/ReadMe.md).

**Commit hash: 652a562c11d3b2ddb85bae509a719d30ed6ecd0c**

### Evaluation Asset
Follow the instructions provided in the [Evaluation Asset Documentation](Assets/Evaluation%20Asset/ReadMe.md).

**Commit hash: 6c4551df61ac1a1829ed0cbf7b9788362ee1342a**

# Build:
## Process:
Standalone, Android and iOS are currently supported using the default Unity build system.

# Installer:
[Wix](http://wixtoolset.org/) is used to create the Windows installer.

Using the game-launcher repository, games can be launched using a url.

## Requirements:
- Wix Toolset
- Visual Studio 2017
- Wix Visual Studio Extension
- [Game Launcher](https://gitlab.com/playgen/game-launcher) project

## Process:
The process for setting up a game installer is detailed within the [Game Launcher documentation](https://gitlab.com/playgen/game-launcher/blob/master/ReadMe.md#game-installer).

## Developer Guide
See the [Developer Guide](DeveloperGuide.md) for further details about the game.