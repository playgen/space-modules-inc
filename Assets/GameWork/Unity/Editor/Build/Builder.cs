using System;
using System.IO;
using UnityEditor;

namespace GameWork.Unity.Editor.Build
{
    public static class Builder
    {
        public static string BuildPath { private get; set; }

        public static string TargetBuildExtension
        {
            get
            {
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneWindows:
                        return ".exe";

                    case BuildTarget.WebGL:
                    case BuildTarget.iOS:
                        return string.Empty;

                    case BuildTarget.Android:
                        return ".apk";

                    case BuildTarget.StandaloneLinux:
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneLinuxUniversal:
                    case BuildTarget.StandaloneOSXUniversal:
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
                    case BuildTarget.WSAPlayer:
                    case BuildTarget.Tizen:
                    case BuildTarget.PSP2:
                    case BuildTarget.PS4:
                    case BuildTarget.PSM:
                    case BuildTarget.XboxOne:
                    case BuildTarget.SamsungTV:
                    case BuildTarget.WiiU:
                    case BuildTarget.tvOS:
                        throw new NotImplementedException();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static string[] Build(BuildTarget[] buildTargets)
        {
            var buildPaths = new string[buildTargets.Length];

            for (var i = 0; i < buildTargets.Length; i++)
            {
                buildPaths[i] = Build(buildTargets[i]);
            }

            return buildPaths;
        }

        public static void Build()
        {
            Build(EditorUserBuildSettings.activeBuildTarget);
        }

        public static string Build(BuildTarget buildTarget)
        {
            SetPlatform(buildTarget);
            SetDefaults();

            var buildEventCache = new BuildEventCache();
            buildEventCache.Execute(EventType.Pre, buildTarget);

            var buildPath = BuildPath;
            CheckBuildPath(buildPath);
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, buildTarget, BuildOptions.None);

            buildEventCache.Execute(EventType.Post, buildTarget);

            return buildPath;
        }

        private static void SetPlatform(BuildTarget buildTarget)
        {
            if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
            }
        }

        private static void SetDefaults()
        {
            BuildPath = "Build/" + EditorUserBuildSettings.activeBuildTarget + TargetBuildExtension;
        }

        private static void CheckBuildPath(string buildPath)
        {
            if (Path.HasExtension(buildPath))
            {
                if (File.Exists(buildPath))
                {
                    File.Delete(buildPath);
                }
                else
                {
                    var buildDir = Path.GetDirectoryName(buildPath);
                    CheckBuildDir(buildDir);
                }
            }
            else
            {
                if (Directory.Exists(buildPath))
                {
                    Directory.Delete(buildPath, true);
                }
                else
                {
                    var buildDir = Directory.GetParent(buildPath).FullName;
                    CheckBuildDir(buildDir);
                }
            }
        }

        private static void CheckBuildDir(string buildDir)
        {
            if (!Directory.Exists(buildDir))
            {
                Directory.CreateDirectory(buildDir);
            }
        }
    }
}