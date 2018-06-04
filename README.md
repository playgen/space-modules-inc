# Overview 
Space Modules Inc is a space ship parts call center service agent simulator game.

# Licensing
See the LICENSE file included in this project.

# Project Structure

**Unity Project**: *Project to be opened in the Unity Editor*  

> **Assets**
>> **Evaluation Asset**: *[third-pary](#Third-Party) asset that evaluates the pedagogical efficiency of the game*  
>> **RAGE Analytics**: *[third-pary](#Third-Party) asset to log to the RAGE analytics server*  
>> **StreamingAssets**: *configuration files*  

> **lib**: *precompiled [third-pary](#Third-Party) libraries*  
>> **ExcelToJsonConverter**: *Used to covnert excel localization files to json*  
>> **GameWork**: *Game Development Framework*  
>> **IntegratedAuthoringTool**: *used in by the emotional decisionmaking component*  
>> **PlayGenUtilities**: *A collection of simple Unity Utilities*  
>> **SUGAR**: *Social Gamification Backend*

> **space-modules-inc-setup**: *The installer project*  
> **Tools**: *setup scripts*  


# Development:
## Requirements:
- Windows OS
- Unity Editor

## Process:
1. Rebuild Libraries:  
Run CreateLibJunctions.bat in Tools/  
This will update libraries for GameWork, SUGAR, Integrated Authoring Tool and other external tools that are being used.

2. Set Custom Args:  
To get the game running in editor, Navigate to Tools/SUGAR/Set Auto Log-in Values and add the following Custom Args: 
`ingameq=true;lockafterq=true;feedback=2;forcelaunch=true`  
Each argument should be separated with a `;`

### Custom Args Available:
- **ingameq**: Should the in game questionnaire be shown
- **lockafterq**: Should the game be locked after the questions have been complete
- **feedback**: The feedback level:  
  - level 1: End game only  
  - level 2: End game and conversation review  
  - Level 3: End game, conversation review and in game feedback
- **forcelaunch**: If the game should ignore time stamp
- **round**: which round of scenarios to play

# Build:
## Requirements:
- Follow the requirements and steps listed in the [Development](#Development) section.

## Process:
Standalone, Android and iOS are currently supported using the default Unity build system.

# Third-Party:
- [SUGAR](http://www.sugarengine.org/) is a Social Gamification Backend. 
- [Evaluation Asset](https://gamecomponents.eu/content/338) asset that evaluates the pedagogical efficiency of the game.  
- [RAGE Analytics](https://gamecomponents.eu/content/232) asset to log to the RAGE analytics server.
- [IntegratedAuthoringTool](https://gamecomponents.eu/content/201): is used in by the emotional decisionmaking component.
- ExcelToJsonConverter: is used to convert Excel Localization files to jSON.
- PlayGen Utilities: is a collection of simple game utilities.
- [GameWork](https://github.com/Game-Work/GameWork.Unity) is a game development framework. 