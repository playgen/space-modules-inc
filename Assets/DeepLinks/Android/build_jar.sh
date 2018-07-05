#!/bin/sh

UNITY_LIBS="/Applications/Unity/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar"
export UNITY_LIBS

ANDROID_SDK_ROOT="C:/Users/Felix/AppData/Local/Android/sdk"
export ANDROID_SDK_ROOT

JDK_HOME=C:/Program Files/Java/jdk1.8.0_121
export JDK_HOME

BOOTCLASSPATH=$JDK_HOME/jre/lib
export BOOTCLASSPATH

CLASSPATH=$UNITY_LIBS:$ANDROID_SDK_ROOT/platforms/android-23/android.jar
export CLASSPATH

echo "Compiling ..."
echo $BOOTCLASSPATH
echo $CLASSPATH
$JDK_HOME/bin/javac *.java -bootclasspath $BOOTCLASSPATH -classpath $CLASSPATH -d .

echo "Manifest-Version: 1.0" > MANIFEST.MF

echo "Creating jar file..."
jar cvfM ../UnityDeeplinks.jar com/

