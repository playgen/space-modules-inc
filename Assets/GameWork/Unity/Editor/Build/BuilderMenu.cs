using System;
using UnityEditor;

namespace GameWork.Unity.Editor.Build
{
    public static class BuilderMenu
    {
        [MenuItem("Tools/GameWork/Build/Active Target")]
        public static void BuildActiveTarget()
        {
            Builder.Build();
        }
    }
}