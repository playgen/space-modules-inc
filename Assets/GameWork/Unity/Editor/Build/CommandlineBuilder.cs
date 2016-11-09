using System;
using System.Collections.Generic;
using UnityEditor;

namespace GameWork.Unity.Editor.Build
{
    public static class CommandlineBuilder
    {
        public static void Build()
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod();
            var fullMethodName = method.ReflectedType.FullName + "." + method.Name;
            
            var buildTargets = GetCommandlineBuildTargets(fullMethodName);
            
            Builder.Build(buildTargets);
        }

        public static BuildTarget[] GetCommandlineBuildTargets(string commandlineMethod)
        {
            var args = Environment.GetCommandLineArgs();
            
            var buildTargetStrings = new List<string>();
            for (var i = args.Length - 1; i >= 0; i--)
            {
                if (args[i] == commandlineMethod)
                {
                    break;
                }

                buildTargetStrings.Add(args[i]);
            }

            var buildTargets = new BuildTarget[buildTargetStrings.Count];

            for (var i = buildTargetStrings.Count - 1; i >= 0; i--)
            {
                buildTargets[i] = (BuildTarget) Enum.Parse(typeof(BuildTarget), buildTargetStrings[i]);
            }

            return buildTargets;
        }
    }
}
