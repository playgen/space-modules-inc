# Must be run from ./Tools directory

function ConvertWav([string]$inputDir, [string]$outputDir)
{
    $oldVideos = Get-ChildItem -Include @("*.wav") -Path $inputDir -Recurse;

    foreach ($oldVideo in $oldVideos) {
        $newVideo = [io.path]::GetFileNameWithoutExtension($oldVideo.FullName);
        $newVideo = [io.path]::Combine($outputDir, $newVideo + ".ogg")

        # Declare the command line arguments for ffmpeg.exe
        $ArgumentList = '-i "{0}" "{1}"' -f $oldVideo, $newVideo

        Start-Process -FilePath "c:\Program Files\ffmpeg\bin\ffmpeg.exe" -ArgumentList $ArgumentList -Wait -NoNewWindow
	}
    
}

ConvertWav -inputDir "..\Audio\wav\Male" -outputDir "..\Audio\ogg\Male"
ConvertWav -inputDir "..\Audio\wav\Female" -outputDir "..\Audio\ogg\Female"