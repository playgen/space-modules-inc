using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace GameWork.Unity.Editor.Build
{
    public class BuildEventCache
    {
        private Dictionary<EventType, List<BuildEventMethodInfo>> _events;
        
        public BuildEventCache()
        {
            InitializeEventsDictionary();
            PopulateEventsDictionary();
            SortEventsDictionary();
        }

        public void Execute(EventType eventType, BuildTarget buildTarget)
        {
            foreach (var buildEventMethodInfo in _events[eventType])
            {
                if (buildEventMethodInfo.BuildTargets.Contains(buildTarget)
                    || buildEventMethodInfo.BuildTargets.Length == 0)
                {
                    buildEventMethodInfo.MethodInfo.Invoke(null, null);
                }
            }
        }

        private void InitializeEventsDictionary()
        {
            _events = new Dictionary<EventType, List<BuildEventMethodInfo>>();

            foreach (var eventType in (EventType[]) Enum.GetValues(typeof(EventType)))
            {
                _events[eventType] = new List<BuildEventMethodInfo>();
            }
        }

        private void PopulateEventsDictionary()
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var buildEvents = (BuildEvent[]) methodInfo.GetCustomAttributes(typeof(BuildEvent), true);
                    foreach (var buildEvent in buildEvents)
                    {
                        _events[buildEvent.EventType].Add(new BuildEventMethodInfo()
                        {
                            Order = buildEvent.Order,
                            BuildTargets = buildEvent.BuildTargets,
                            MethodInfo = methodInfo,
                        });
                    }
                }
            }
        }

        private void SortEventsDictionary()
        {
            foreach (var eventSpecificMethods in _events.Values)
            {
                eventSpecificMethods.Sort((a, b) => a.Order.CompareTo(b.Order));
            }
        }

        private class BuildEventMethodInfo
        {
            public int Order { get; set; }

            public BuildTarget[] BuildTargets { get; set; }

            public MethodInfo MethodInfo { get; set; }
        }
    }
}