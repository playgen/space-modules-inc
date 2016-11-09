using System;
using UnityEditor;

namespace GameWork.Unity.Editor.Build
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BuildEvent : Attribute
    {
        public EventType EventType { get; set; }
        public int Order { get; set; }
        public BuildTarget[] BuildTargets { get; set; }
        
        public BuildEvent(EventType eventType, int order = 0, params BuildTarget[] buildTargets)
        {
            EventType = eventType;
            Order = order;
            BuildTargets = buildTargets;
        }
    }
}