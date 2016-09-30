using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class GameObjectUtilities
{
    /// <summary>
    /// Breadth first search for game objects.
    /// 
    /// Root cannot be inactive.
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <returns></returns>
    public static Transform[] FindAll(string absolutePath)
    {
        var segments = absolutePath.Split('/');
        var level = 0;

        var childObject = GameObject.Find(segments[level]);

        if (childObject == null)
        {
            Debug.LogWarning("Couldn't find any object at path: " + absolutePath);
            return null;
        }

        var rootTransform = childObject.transform;

        var currentLevel = new List<Transform> { rootTransform };
        var nextLevel = new List<Transform>();

        List<Transform> matches = FindMatches(++level, segments, currentLevel, nextLevel);

        return matches.ToArray();
    }

    public static GameObject[] FindAllGameObjects(string absolutePath)
    {
        var results = FindAll(absolutePath);
        return results.Select(t => t.gameObject).ToArray();
    }

    /// <summary>
    /// Breadth first search for game objects.
    /// 
    /// Root cannot be inactive.
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <returns></returns>
    public static Transform Find(string absolutePath)
    {
        var results = FindAll(absolutePath);

        if (results.Length != 1)
        {
            if (results.Length == 0)
            {
                Debug.LogWarning(string.Format("Couldn't find any objects matching the path: \"{0}\"", absolutePath));
            }
            else
            {
                Debug.LogWarning(string.Format("Found {0} objects matching the path: \"{1}\"", results.Length, absolutePath));
            }

            return null;
        }

        return results[0];
    }

    public static GameObject FindGameObject(string absolutePath)
    {
        var result = Find(absolutePath);
        return result.gameObject;
    }

    public static GameObject[] FindAllChildren(string absolutePath)
    {
        var result = Find(absolutePath);

        var childCount = result.childCount;

        if (childCount < 1)
        {
            Debug.LogWarning(string.Format("Couldn't find any children of the object matching the path: \"{0}\"", absolutePath));
            return null;
        }

        var children = new List<Transform>();

        for (var i = 0; i < childCount; i++)
        {
            children.Add(result.GetChild(i));
        }

        return children.Select(t => t.gameObject).ToArray();
    }

    private static List<Transform> FindMatches(int level, string[] pathSegments, List<Transform> currentLevel, List<Transform> nextLevel)
    {
        if (level < pathSegments.Length)
        {
            foreach (var transform in currentLevel)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).name == pathSegments[level])
                    {
                        nextLevel.Add(transform.GetChild(i));
                    }
                }
            }

            currentLevel.Clear();
            level++;

            return FindMatches(level, pathSegments, nextLevel, currentLevel);
        }

        return currentLevel;
    }

}