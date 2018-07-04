# Developer Guide
Below are more details about getting started when making changes in Space Modules Inc.

## DeepLinks
The game is configured to launch from a URL for Android, iOS (both via Deep Links) and PC (via the [Game Launcher](https://gitlab.com/playgen/game-launcher)).

Game Launcher URLs starts with: `rage://?gameid=SMI`  
Mobile URLs start with: `rage://SMI?`  
After that all URLs use the same system for passing arguments using key value pairs, such as: `username=SMI_User1&class=Testing&feedback=2&forcelaunch=true&hash=A2AF8BA8AF05A16E0282A0D318EBF4FDF070636C`

## Unity Game
### Key Scene Structure
- **Background Camera**: This camera is used to draw a blank black background around the portrait-aligned gameplay in PC builds.
- **Main Camera**: Camera used by the Canvas, meaning all UI and thus all gameplay is displayed on this camera. ForcePortraitCamera script ensures gameplay aspect ratio is always portrait.
- **Controller**: Contains the ControllerBehaviour and UnityDeeplinks scripts.
- **EventSystem**: StandaloneInputModule made as generic as possible to further limit ability to interact with UI using the keyboard.
- **Canvas**
  - **BackgroundContainer**
  - **SplashContainer**
  - **MenuContainer**
  - **SettingsContainer**
    - **SettingsPanelContainer/SettingsPanel/Set Settings Panel**: Prefab object. Using the SettingCreation script from the Settings asset in PlayGen Unity Utilities to create the settings menu at run-time.
  - **LevelContainer**
  - **CallContainer**
  - **GameContainer**
  - **ReviewContainer**
  - **ScoreContainer**
- **SUGAR**: Prefab containing all components relating to SUGAR, including the SUGAR Unity Manager, Response Handler and Unity Clients.
- **Tracker**: Contains the Tracker script for the RAGE Analytics.

## Key Classes
- **Behaviours/CharacterFaceController** - Manages changeing sprites based on the strongest emotion of the character and animating their talking and blinking.
- **Behaviour/ControllerBehaviour** - Creates the controllers for Audio, GameStates, Scenarios and Modules and puts the created GameStateControllerFactory into the initial game state.
- **Behaviour/ForcePortraitCamera** - Forces cameras with a landscape aspect ratio to instead use the portrait equivalent of that aspect ratio.
- **Controllers/ModulesController** - Controls loading and displaying module information.
- **Controllers/ScenarioController** - Manages everything to do with scenarios, including loading and selecting scenarios, sending and receiving player and NPC dialogue (which includes triggering audio to play) and calculating the end score of each scenario.
- **GameStates/Inputs/GameStateInput** - handles UI whilst in the ‘Game’ state, including updating the NPC's expression and dialogue and displaying dialogue options to the player.
- **GameStates/GameStateControllerFactory** - creates all of the states and sets up the transition events needed to move between them.

## Setting up your game with SUGAR
For information on setting up Space Modules Inc to use SUGAR, see the [SUGAR Quick Start Guide](http://api.sugarengine.org/v1/unity-client/tutorials/quick-start.html). **Make sure that *Assets\StreamingAssets\SUGAR.config.json* exists and the BaseUri value matches the Base Address in the SUGAR Prefab.** 

### Running SUGAR Locally
Using Space Modules Inc with a local version of SUGAR is as simple as changing the Base Address in the SUGAR Prefab, and the BaseUri value in *Assets\StreamingAssets\SUGAR.config.json*.

## Converting IntegratedAuthoringTool into current format
In order to convert an IntegratedAuthoringTool file which follows the older file format into the current format, the following steps should be followed:

1.
**Old Format**
```JSON
"Characters": [
      {
        "Name": "Positive",
        "Source": "Positive.rpc"
      },
      {
        "Name": "Neutral",
        "Source": "Neutral.rpc"
      },
      {
        "Name": "Negative",
        "Source": "Negative.rpc"
      }
    ],
```
**New Format**
```JSON
"CharacterSources": ["..\\ScenarioRelated\\Negative.rpc", "..\\ScenarioRelated\\Neutral.rpc", "..\\ScenarioRelated\\Positive.rpc"],
```
2.
**Old Format**
```JSON
"Meaning": [
```
**New Format**
```JSON
"Meaning": 
```
3.
**Old Format**
```JSON
],
        "Style": [
```
**New Format**
```JSON
,
				"Style": 
```
4.
**Old Format**
```JSON
],
        "FileName":
```
**New Format**
```JSON
,
				"FileName":
```
5.
**Old Format**
```JSON
": ,
```
**New Format**
```JSON
": "-",
```
6.
**Old Format** (also check for other values other than -1)
```JSON 
(-1)", "
```
**New Format**
```JSON
_
```
7. Open the file using the IntegratedAuthoringToolWF.exe found in *FAtiMA-Toolkit\AuthoringTools\IntegratedAuthoringToolWF\bin\Debug* and save.
## Level Selection
Space Modules Inc supports both an automatic level progression system and a level selection system. The current version defaults to automatic progression and as a result the level selection screen is never shown. 

To change this, go to the CreateMenuState method in the [GameStateControllerFactory](Assets/Scripts/GameStates/GameStateControllerFactory.cs) class. This contains 2 variations of nextStateTransition. To change the next state after pressing play, change the EventTransition to be either *CallState* (automatic progression) or *LevelState* (level progression).

## Scenario Structure
The base of a scenario is an [IntegratedAuthoringTool](https://gamecomponents.eu/content/201) file created using the [FAtiMA Toolkit](https://gitlab.com/playgen/FAtiMA-Toolkit). These files allow us to define player and NPC dialogue options and what NPCs ([RolePlayCharacter](https://www.gamecomponents.eu/content/196)) should be available. A RolePlayCharacter can in turn refer to other files which define their starting emotions and temperament ([EmotionalAppraisal](https://www.gamecomponents.eu/content/224)) and their decision making process ([EmotionalDecisionMaking](https://www.gamecomponents.eu/content/218)). Note that it is key that the PlayGen fork of FAtiMA is used, as this version contains additional features required for Space Modules Inc, most notably the 'File Name' value for each piece of dialogue which is used to find and play dialogue audio.

The scenarios to use are defined within *Assets\StreamingAssets\levelconfig.json*. A LevelConfig is made up of a collection of Rounds, which in turn is made up of a collection of ScenarioData called Levels. ScenarioData contains the following:
- **ID**: A unique identifier for the level.
- **Prefix**: A string which identifies the type of scenario to load.
- **Character**: Which character to use in the scenario.
- **MaxPoints**: The most points a player can earn from metrics playing one of these scenarios.

The collection of ScenarioData to use is determined using the RoundNumber value in ScenarioController.

A ScenarioData is used when the CurrentLevel in the ScenarioController matches the **ID**. Any scenario that contains the **Prefix** provided can be used, with the provided **Character** used. A player's star rating at the end of the level is then defined using the **MaxPoints** value provided. If no ScenarioData is provided for a Round or if no scenarios match the prefix provided in a ScenarioData, a random ScenarioData is created from all scenarios available.

The process of loading and setting up a scenario is performed within the NextLevel() method in ScenarioController. Further information on loading scenarios and characters can be found in the [FAtiMA Toolkit Cheat Sheet](https://gitlab.com/playgen/FAtiMA-Toolkit/blob/master/PlayGen-Cheat-Sheet.md).

## TODO Character Emotions
TODO how are character emotions defined, what does the game currently support, how would you go about adding additional emotions/art assets

## TODO Game States
TODO overview of game state management, perhaps link to gamework docs

## Scoring
Player scoring is determined based on the five metrics that are tracked; closure, inquire, faq, empathy and polite. For each scenario, a maximum score is known and is used to calculate how well players have done. The number of stars achieved is based on the player's actual score and the maximum score. 

% of max points | Stars | Score Range
--- | --- |---
0 - Less than 40 | 1 Star | 100 - 1000
40 - 80 | 2 Stars | 1001 - 5000
Greater than 80 - 100 | 3 Stars | 5001 - 15000

The character's mood is used to determine the players score within the score range for the number of stars earned.

The player's score, the number of stars earned and their score in each scoring metrics is sent to SUGAR At the conclusion of every scenario. In addition to this, 1 is sent for the 'plays' key, which records how many scenarios they have played, and the number of stars earned is also sent to a key which contains the Round and Level number, allowing for a record of their result in that scenario to be recorded and displayed on the level selection screen.

Full scoring details can be found in GetScoreData() in [ScenariosController.cs](Assets\Scripts\Controllers\ScenarioController.cs).