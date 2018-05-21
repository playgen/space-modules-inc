# Space Modules Inc. Setup
## 1. Rebuild Libraries
Run CreateLibJunctions.bat in /Tools/
This will update libraries for GameWork, SUGAR, Integrated Authoring Tool and other external tools that are being used.

## 2. Set Custom Args
To get the game running in editor, Navigate to Tools/SUGAR/Set Auto Log-in Values and add the following Custom Args:

    ingameq=true;lockafterq=true;feedback=2;forcelaunch=true

Each argument should be separated with a ;

### Custom Args Available
- **ingameq**: Should the in game questionnaire be shown
- **lockafterq**: Should the game be locked after the questions have been complete
- **feedback**: The feedback level (level 1: End game only, level 2: End game and conversation review, Level 3: End game, conversation review and in game feedback)
- **forcelaunch**: If the game should ignore time stamp
- **round**: which round of scenarios to play
