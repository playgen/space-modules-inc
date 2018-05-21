== SPACE MODULES INC.

## 1. Rebuild Libraries
Run CreateLibJunctions.bat in /Tools/
This will update libraries for GameWork, SUGAR, Integrated Authoring Tool and other external tools that are being used.

## 2. Set Custom Args
To get the game running in editor, Navigate to Tools/SUGAR/Set Auto Log-in Values and add the following Custom Args:

    ingameq=true;lockafterq=true;feedback=2;forcelaunch=true