# Must be run from ./Tools directory

# Copy Gamework.Core into GameWork.unity
pushd ../GameWork.Unity/Tools
chmod u+x ./Copy_GameWorkCore.command
./Copy_GameWorkCore.command
popd

# Copy GameWork.Unity

function CopyGameworkComponent {

	sourceDir="$1"
	destDir="$2"

	rm -rf $destDir
	mkdir -p $destDir

	cp -r $sourceDir $(dirname $destDir)
}

CopyGameworkComponent ../GameWork.Unity/UnityProject/Assets/GameWork/Core ../Assets/GameWork/Core
CopyGameworkComponent ../GameWork.Unity/UnityProject/Assets/GameWork/Unity ../Assets/GameWork/Unity