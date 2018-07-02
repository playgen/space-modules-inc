# Developer Guide
Below are more details about getting started with making changes in Space Modules inc.

## DeepLinks
The game is configured to launch from DeepLink urls for both Android and iOS.

This url is the same as the launcher url except that where the launcher url starts with:  
`rage://?gameid=SMI`  
the deeplink url starts with:  
`rage://SMI?`  
After that the rest of the argument are the same e.g:  
`rage://smi?username=SMI_User1&class=Testing&feedback=2&forcelaunch=true&hash=A2AF8BA8AF05A16E0282A0D318EBF4FDF070636C`

## Unity Game
### Key Scene Structure
- **Background Camera**: This camera is used to draw a blank black background around the portrait-aligned gameplay in PC builds.
- **Main Camera**
- **Controller**: contains the ControllerBehaviour script.
- **EventSystem**
- **Canvas**
  - **BackgroundContainer**
  - **SplashContainer**
  - **MenuContainer**
  - **SettingsContainer**
    - **SettingsPanelContainer/SettingsPanel/Set Settings Panel**: prefab object. Using the SettingCreation script from the PlayGen Settings asset to create the settings menu at run-time.
  - **LevelContainer**
  - **CallContainer**
  - **GameContainer**
  - **ReviewContainer**
  - **ScoreContainer**
- **SUGAR**: prefab containing all components relating to SUGAR.
- **Tracker**: contains the Tracker script for the RAGE Analytics.

## Key Classes
- Behaviours/CharacterFaceController - Manages loading in facial sprites, changes spirtes based on the emotion of the character and animating talking and blinking.
- Behaviour/ControllerBehaviour - Starts controllers, fixes aspect ratio to be height/width when width is greater than height on PC.
- Controllers/ModulesController - controls loading and displaying module information.
- Controllers/ScenarioController - manages loading and selecting scenarios, loading, sending and receiving player and controller dialogue (including triggering audio to play) and calculates the end score of each scenario played.
- GameStates/Inputs/GameStateInput - handles UI whilst in the ‘Game’ state, including updating character expression and dialogue and triggering various commands.
- GameStates/GameStateControllerFactory - creates and sets up events for all states within the game.

## Setting up your game with SUGAR
For information on Setting up Space Modules Inc. using SUGAR, see [SUGAR Quick Start Guide](http://api.sugarengine.org/v1/unity-client/tutorials/quick-start.html). *make sure that Assets\StreamingAssets\SUGAR.config.json exists and the BaseUri value matches the Base Address in the SUGAR Prefab.* 

### Running SUGAR Locally
Using Space Modules inc. with a local version of SUGAR is as simple as changing the Base Address in the SUGAR Prefab, and the BaseUri value in *Assets\StreamingAssets\SUGAR.config.json*

## Level Selection
Space Modules inc. supports both an automatic level progression system, and allowing players to select a level they want to play. In the current version, the level selection screen is never shown. 

To change this, see [GameStateControllerFactory](Assets/Scripts/GameStates/GameStateControllerFactory.cs) for the CreateMenuState logic. This contains 2 variations of nextStateTransition. To change the next state after pressing play, change the EventTransition to be either *CallState* or *LevelState*.

## TODO Scenario Structure
TODO how are they defined, what information is required, current setup, loading scenarios, worked example

## TODO Character Emotions
TODO how are character emotions defined, what does the game currently support, how would you go about adding additional emotions/art assets

## TODO Game States
TODO overview of game state management, perhaps link to gamework docs

## Scoring
Player scoring is determined based on the 5 metrics that are tracked; closure, inquire, faq, empathy and polite. For each scenario, a maximum score is known, and is used to calculate how well players have done. The number of stars achieved is based on the players actual score and the maximum score. 

% of max points | Stars | Score Range
--- | --- |---
0 - 40 | 1 Star | 100 - 1000
40 - 80 | 2 Stars | 1001 - 5000
80 - 100 | 3 stars | 5001 - 15000

The characters mood is used to determine the players score using the score range for the number of stars achieved

Full Scoring details can be found in GetScoreData() in [ScenariosController.cs](Assets\Scripts\Controllers\ScenarioController.cs)

### Saving Scores
Star ratings for each level are saved to SUGAR at the end of each scenario, using the following:

``` c#
    // Send Game Data
    SUGARManager.GameData.Send(string name, long value);
    SUGARManager.GameData.Send(string name, bool value);
    SUGARManager.GameData.Send(string name, float value);
    SUGARManager.GameData.Send(string name, string value);


    // Get Game Data 
    SUGARManager.GameData.Get(Action<IEnumberableM<EvaluationDataResponse>> success, string[] keys = null)
```