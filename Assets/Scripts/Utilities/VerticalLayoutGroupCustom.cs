using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VerticalLayoutGroupCustom : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectPadding
    {
        public float left;
        public float right;
        public float bottom;
        public float top;
    }

    [System.Serializable]
    public struct ChildExpand
    {
        public bool Width;
        public bool Height;
    }

    public enum Orientation
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    //[HideInInspector]
    public ObjectPadding Padding;

    public float Spacing;

    //public Orientation ChildAlignment;

    public ChildExpand ChildForceExpand;

    private RectTransform _myRectTransform;

    

    private void Setup()
    {
        _myRectTransform = this.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        // Set the pivot of this object

        if (_myRectTransform == null)
        {
            Setup();
        }
        //Find all children
        var children = new List<RectTransform>();

        foreach (Transform t in transform)
        {
            children.Add(t.GetComponent<RectTransform>());
        }

        var offset = 0f;

        foreach (var child in children)
        {
            var height = 0.0f;

            child.pivot = _myRectTransform.pivot;
            
            // Update to the new height of the object, updated last frame
            if (child.GetComponent<ContentSizeFitter>() != null)
            {
                height = child.rect.height;
            }
            // Set the force width / height

            if (ChildForceExpand.Width)
            {
                child.sizeDelta = new Vector2(_myRectTransform.rect.width, child.sizeDelta.y);
            }
            if (ChildForceExpand.Height)
            {
                child.sizeDelta = new Vector2(child.sizeDelta.x, _myRectTransform.rect.height / children.Count);
                
                // update the height
                height = child.rect.height;
            }
            // Set the padding
            
            child.anchorMin = Vector2.zero;
            child.anchorMax = Vector2.one;
            child.offsetMin = new Vector2(Padding.left, Padding.bottom);
            child.offsetMax = new Vector2(Padding.right, Padding.top);
            if (height == 0f)
            {
                height = child.rect.height;
            }

            // Move the child into position
            child.localPosition = new Vector3(0, offset, 0f);



            // increment the offset and include the spacing
           offset -= (height + Spacing);

        }
    }
}
