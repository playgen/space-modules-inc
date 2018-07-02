# Overview 
Space Modules Inc is a space ship parts call center service agent simulator game.

It is part of the [RAGE project](http://rageproject.eu/).

# Licensing
See the [LICENCE](LICENCE.md) file included in this project.

# Key Project Structure

- **Unity Project**: *Project to be opened in the Unity Editor*  
  - **Assets**  
    - **Editor**
      - **FATiMAImporter**: *Utility to parse the scenario content files into a Unity readable format.* 
    - **DeepLink iOS**: *[Included-Assets](#Included-Assets)*  
    - **DeepLink Plugin**: *[Included-Assets](#Included-Assets)*  
    - **Evaluation Asset**: *[Included-Assets](#Included-Assets) asset that evaluates the pedagogical efficiency of the game*  
    - **RAGE Analytics**: *[Included-Assets](#Included-Assets) asset to log to the RAGE analytics server*  
  - **StreamingAssets**
    - **levelconfig.json**: *configuration file for levels*  
    - **modules-en.json**: *space modules data in English*  
    - **modules-nl.json**: *space modules data in Dutch*
  - **ScenariosRelated**: *Files used by the scenarios.*
  - **Scenarios**: *The scenarios in game.*
  - **lib**: *precompiled [Included-Assets](#Included-Assets) libraries*  
    - **ExcelToJsonConverter**: *Used to covnert excel localization files to json*  
    - **GameWork**: *Game Development Framework*  
    - **IntegratedAuthoringTool**: *used in by the emotional decisionmaking component*  
    - **PlayGenUtilities**: *A collection of simple Unity Utilities*  
    - **SUGAR**: *Social Gamification Backend*
  - **space-modules-inc-setup**: *installer project*  
  - **Tools**: *setup scripts*  
  - **user.keystore**: *keystore for Android builds*

# Included Assets:
- [Server-Side Dashboard and Analysis](https://www.gamecomponents.eu/content/195)
- [Role-Play Character](https://www.gamecomponents.eu/content/196)
- [SUGAR](https://gamecomponents.eu/content/200)
- [Integrated Authoring Tool](https://gamecomponents.eu/content/201): is used in by the emotional decisionmaking component.
- [Social Importance Dynamics](https://www.gamecomponents.eu/content/207)
- [Emotional Decision Making Asset](https://www.gamecomponents.eu/content/218)
- [Server-Side Interaction Storage and Analytics](https://www.gamecomponents.eu/content/220)
- [Emotional Appraisal Asset](https://www.gamecomponents.eu/content/224)
- [Client-Side Tracker](https://gamecomponents.eu/content/232)
- [Evaluation Component](https://gamecomponents.eu/content/338)
- [ExcelToJsonConverter](https://github.com/Benzino/ExcelToJsonConverter): is used to convert Excel Localization files to jSON.
- [PlayGen Unity Utilities](git@gitlab.com:playgen/unity-utilities.git): is a collection of simple game utilities.
- [GameWork](https://github.com/JaredGG/GameWork.Unity) is a game development framework. 
- [DeepLink iOS](https://github.com/TROPHiT/UnityDeeplinks)
- [DeepLink Plugin](https://assetstore.unity.com/packages/tools/integration/deeplink-plugin-30430): for android.

# Development:
## Requirements:
- Windows OS
- Unity Editor
- Powershell
- FFmpeg

## Process
1. Create ‘Male’ and ‘Female’ folders in Audio/ogg (there should already be metas) and  run ConvertWav.ps1 to create ogg versions of the dialogue audio files  
1.1. Note: FFMPEG will need to be installed and set-up to be available from the command line for this to work.

2. Run CreateLibJunctions.bat to create the required SymLinks.

3. Pull the RAGE Analytics Unity Tracker and place the required files into Assets\Plugins\unity-tracker. Note that code changes could be needed as a result of updates to this asset.

4. In Unity's Project View, select the ScenariosRelated and Scenarios and right-click/ReImport to generate the Resources versions.

5. To get the game running in editor, Navigate to Tools/SUGAR/Set Auto Log-in Values and add the following [Custom Args](#Custom-Args):
`ingameq=true;lockafterq=true;feedback=2;forcelaunch=true`    
Note: Each argument should be separated with a `;`  

### Custom Args: 
- `ingameq`: Should the in game questionnaire be shown
- `lockafterq`: Should the game be locked after the questions have been complete
- `feedback`: The feedback level:  
  - level 0: End game only  
  - level 1: End game and conversation review  
  - Level 2: End game, conversation review and in game feedback
- `forcelaunch` If the game should ignore time stamp
- `round`: which round of scenarios to play

## Updating:
### FAtiMA-Toolkit 
Build solution and place new DLLs (found in FAtiMA-Toolkit\Assets\IntegratedAuthoringTool\bin\Debug) into lib\IntegratedAuthoringTool folder. Note that code changes could be needed as a result of updates to this asset.

### GameWork.Unity ha
Build solution and place new DLLs (found in GameWork.Unity/bin) into lib\GameWork folder. Note that code changes could be needed as a result of updates to this asset.

### PlayGen Unity Utilities 
Build solution and place new DLLs (found in various folders in this project) into lib\PlayGenUtilities folder. Note that code changes could be needed as a result of updates to this asset. New prefabs may also need copying, although take care not to overwrite customised versions of previous prefabs.  

New DLLs should also be copied into the lib\PlayGen Utilities folders in the PlayGen Unity Settings and SUGAR Unity projects. 

### PlayGen Unity Settings
Build solution and place new DLLs (found in unity-settings\Assets\Plugins\Settings) into lib\PlayGenUtilities folder. Note that code changes could be needed as a result of updates to this asset. New prefabs may also need copying, although take care not to overwrite customised versions of previous prefabs.

### SUGAR Unity Asset
Update the DLLs found in the lib\SUGAR folder in stm-unity  

Note: It is advised that you do not remove the Prefabs for SUGAR, as these have been edited to match the styling of Space Modules Inc. Only replace these assets if they are no longer compatible with the latest version of the SUGAR Unity asset, and even then be aware that you will need to recreate the previous styling if this is done.

### SUGAR-Unity version
Commit hash: 7e634030d1c4a68c73f8d1e182a01288b6d505c1

# Build:
## Process:
Standalone, Android and iOS are currently supported using the default Unity build system.

# Installer:
[Wix](http://wixtoolset.org/) is used to create the Windows installer.

The game-launcher repository is used to launch the game from url.

## Requirements:
- Wix Toolset
- Visual Studio 2017
- Wix Visual Studio Extension
- [game-launcher](https://gitlab.com/playgen/game-launcher) project

## TODO Process:
1. Create a Unity PC Build at Build\WindowsSMI called “SMI”.
2. Once built, go to the project solution (space-modules-inc.sln) and build the space-modules-inc-setup project.
3. The resulting windows installer can be found at space-modules-inc-installer\bin\Debug\space-modules-inc_setup.msi.

TODO explain how this works with wix and how to create create an installer etc once you've made a standalone build

## Developer Guide
See the [Developer Guide](DeveloperGuide.md) for further details about the game.